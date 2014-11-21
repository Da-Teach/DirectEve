// ------------------------------------------------------------------------------
//   <copyright from='2010' to='2015' company='THEHACKERWITHIN.COM'>
//     Copyright (c) TheHackerWithin.COM. All Rights Reserved.
// 
//     Please look in the accompanying license.htm file for the license that 
//     applies to this source code. (a copy can also be found at: 
//     http://www.thehackerwithin.com/license.htm)
//   </copyright>
// -------------------------------------------------------------------------------

namespace EasyHook
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;

#pragma warning disable 1591

    public class HelperServiceInterface : MarshalByRefObject
    {
        public void InjectEx(
            Int32 InHostPID,
            Int32 InTargetPID,
            Int32 InWakeUpTID,
            Int32 InNativeOptions,
            String InLibraryPath_x86,
            String InLibraryPath_x64,
            Boolean InCanBypassWOW64,
            Boolean InCanCreateService,
            Boolean InRequireStrongName,
            params Object[] InPassThruArgs)
        {
            RemoteHooking.InjectEx(
                InHostPID,
                InTargetPID,
                InWakeUpTID,
                InNativeOptions,
                InLibraryPath_x86,
                InLibraryPath_x64,
                InCanBypassWOW64,
                InCanCreateService,
                InRequireStrongName,
                InPassThruArgs);
        }

        public Object ExecuteAsService<TClass>(
            String InMethodName,
            Object[] InParams)
        {
            return typeof (TClass).InvokeMember(InMethodName, BindingFlags.InvokeMethod | BindingFlags.Public |
                                                              BindingFlags.Static, null, null, InParams);
        }

        private class InjectionWait
        {
            public Mutex ThreadLock = new Mutex(false);
            public ManualResetEvent Completion = new ManualResetEvent(false);
            public Exception Error = null;
        }

        private static SortedList<Int32, InjectionWait> InjectionList = new SortedList<Int32, InjectionWait>();

        public static void BeginInjection(Int32 InTargetPID)
        {
            InjectionWait WaitInfo;

            lock (InjectionList)
            {
                if (!InjectionList.TryGetValue(InTargetPID, out WaitInfo))
                {
                    WaitInfo = new InjectionWait();

                    InjectionList.Add(InTargetPID, WaitInfo);
                }
            }

            WaitInfo.ThreadLock.WaitOne();
            WaitInfo.Error = null;
            WaitInfo.Completion.Reset();

            lock (InjectionList)
            {
                if (!InjectionList.ContainsKey(InTargetPID))
                    InjectionList.Add(InTargetPID, WaitInfo);
            }
        }

        public static void EndInjection(Int32 InTargetPID)
        {
            lock (InjectionList)
            {
                InjectionList[InTargetPID].ThreadLock.ReleaseMutex();

                InjectionList.Remove(InTargetPID);
            }
        }

        public static void WaitForInjection(Int32 InTargetPID)
        {
            InjectionWait WaitInfo;

            lock (InjectionList)
            {
                WaitInfo = InjectionList[InTargetPID];
            }

            if (!WaitInfo.Completion.WaitOne(20000, false))
                throw new TimeoutException("Unable to wait for injection completion.");

            if (WaitInfo.Error != null)
                throw WaitInfo.Error;
        }

        public void InjectionException(
            Int32 InClientPID,
            Exception e)
        {
            InjectionWait WaitInfo;

            lock (InjectionList)
            {
                WaitInfo = InjectionList[InClientPID];
            }

            WaitInfo.Error = e;
            WaitInfo.Completion.Set();
        }

        public void InjectionCompleted(Int32 InClientPID)
        {
            InjectionWait WaitInfo;

            lock (InjectionList)
            {
                WaitInfo = InjectionList[InClientPID];
            }

            WaitInfo.Error = null;
            WaitInfo.Completion.Set();
        }

        public void Ping()
        {
        }
    }
}