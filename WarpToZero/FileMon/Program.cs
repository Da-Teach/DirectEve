// ------------------------------------------------------------------------------
//   <copyright from='2010' to='2015' company='THEHACKERWITHIN.COM'>
//     Copyright (c) TheHackerWithin.COM. All Rights Reserved.
// 
//     Please look in the accompanying license.htm file for the license that 
//     applies to this source code. (a copy can also be found at: 
//     http://www.thehackerwithin.com/license.htm)
//   </copyright>
// -------------------------------------------------------------------------------

namespace Aphack
{
    using System;
    using System.Diagnostics;
    using System.Runtime.Remoting;
    using System.Windows.Forms;
    using EasyHook;

    public class AphackInterface : MarshalByRefObject
    {
        public void IsInstalled(Int32 InClientPID)
        {
            Console.WriteLine("aphack has been installed in target {0}.\r\n", InClientPID);
        }

        public void ReportException(Exception InInfo)
        {
            Console.WriteLine("The target process has reported an error:\r\n" + InInfo.ToString());
        }

        public void Ping()
        {
        }
    }

    internal class Program
    {
        private static String ChannelName = null;

        private static void Main(string[] args)
        {
            var TargetPID = 0;
            //TargetPID = System.Diagnostics.Process.GetProcessesByName("exefile")[0].Id;
            foreach (var exefile in Process.GetProcessesByName("exefile"))
            {
                ChannelName = null;
                TargetPID = exefile.Id;

                try
                {
                    try
                    {
                        Config.Register(
                            "A Aphack like demo application.",
                            "Aphack.exe",
                            "AphackInject.dll");
                    }
                    catch (ApplicationException)
                    {
                        MessageBox.Show("This is an administrative task!", "Permission denied...", MessageBoxButtons.OK);

                        Process.GetCurrentProcess().Kill();
                    }

                    RemoteHooking.IpcCreateServer<AphackInterface>(ref ChannelName, WellKnownObjectMode.SingleCall);

                    RemoteHooking.Inject(
                        TargetPID,
                        "AphackInject.dll",
                        "AphackInject.dll",
                        ChannelName);
                }
                catch (Exception ExtInfo)
                {
                    Console.WriteLine("There was an error while connecting to target:\r\n{0}", ExtInfo.ToString());
                }
            }
            Console.ReadLine();
        }
    }
}