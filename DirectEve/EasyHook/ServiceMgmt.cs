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
    using System.IO;
    using System.Threading;

    internal class ServiceMgmt
    {
        private static Mutex m_TermMutex = null;
        private static HelperServiceInterface m_Interface = null;
        private static Object ThreadSafe = new Object();

        private static void Install()
        {
            lock (ThreadSafe)
            {
                if (m_Interface == null)
                {
                    // create sync objects
                    var ChannelName = RemoteHooking.GenerateName();
                    var Listening = new EventWaitHandle(
                        false,
                        EventResetMode.ManualReset,
                        "Global\\Event_" + ChannelName);
                    var TermMutex = new Mutex(true, "Global\\Mutex_" + ChannelName);

                    using (TermMutex)
                    {
                        // install and start service
                        NativeAPI.RtlInstallService(
                            "EasyHook" + (NativeAPI.Is64Bit ? "64" : "32") + "Svc",
                            Path.GetFullPath(Config.GetDependantSvcExecutableName()),
                            ChannelName);

                        if (!Listening.WaitOne(5000, true))
                            throw new ApplicationException("Unable to wait for service startup.");

                        var Interface = RemoteHooking.IpcConnectClient<HelperServiceInterface>(ChannelName);

                        Interface.Ping();

                        // now we can be sure that all things are fine...
                        m_Interface = Interface;
                        m_TermMutex = TermMutex;
                    }
                }
            }
        }

        public static void Inject(
            Int32 InHostPID,
            Int32 InTargetPID,
            Int32 InWakeUpTID,
            Int32 InNativeOptions,
            String InLibraryPath_x86,
            String InLibraryPath_x64,
            Boolean InRequireStrongName,
            params Object[] InPassThruArgs)
        {
            Install();

            m_Interface.InjectEx(
                InHostPID,
                InTargetPID,
                InWakeUpTID,
                InNativeOptions,
                InLibraryPath_x86,
                InLibraryPath_x64,
                false,
                false,
                InRequireStrongName,
                InPassThruArgs);
        }

        public static Object ExecuteAsService<TClass>(
            String InMethodName,
            params Object[] InParams)
        {
            Install();

            return m_Interface.ExecuteAsService<TClass>(InMethodName, InParams);
        }
    }
}