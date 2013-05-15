using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Text;
using System.IO;
using EasyHook;
using System.Windows.Forms;

namespace Aphack
{
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

    class Program
    {
        static String ChannelName = null;

        static void Main(string[] args)
        {
            Int32 TargetPID = 0;
            //TargetPID = System.Diagnostics.Process.GetProcessesByName("exefile")[0].Id;
            foreach (var exefile in System.Diagnostics.Process.GetProcessesByName("exefile"))
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

                        System.Diagnostics.Process.GetCurrentProcess().Kill();
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