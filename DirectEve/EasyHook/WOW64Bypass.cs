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
    using System.Diagnostics;
    using System.Threading;

    internal class WOW64Bypass
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
                    var ChannelName = RemoteHooking.GenerateName();
                    var SvcExecutablePath = (Config.DependencyPath.Length > 0 ? Config.DependencyPath : Config.GetProcessPath()) + Config.GetWOW64BypassExecutableName();

                    var Proc = new Process();
                    var StartInfo = new ProcessStartInfo(
                        SvcExecutablePath, "\"" + ChannelName + "\"");

                    // create sync objects
                    var Listening = new EventWaitHandle(
                        false,
                        EventResetMode.ManualReset,
                        "Global\\Event_" + ChannelName);

                    m_TermMutex = new Mutex(true, "Global\\Mutex_" + ChannelName);

                    // start and connect program
                    StartInfo.CreateNoWindow = true;
                    StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                    Proc.StartInfo = StartInfo;

                    Proc.Start();

                    if (!Listening.WaitOne(5000, true))
                        throw new ApplicationException("Unable to wait for service application due to timeout.");

                    var Interface = RemoteHooking.IpcConnectClient<HelperServiceInterface>(ChannelName);

                    Interface.Ping();

                    m_Interface = Interface;
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
                true,
                InRequireStrongName,
                InPassThruArgs);
        }
    }
}