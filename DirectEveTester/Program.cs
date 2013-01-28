// ------------------------------------------------------------------------------
//   <copyright from='2010' to='2015' company='THEHACKERWITHIN.COM'>
//     Copyright (c) TheHackerWithin.COM. All Rights Reserved.
// 
//     Please look in the accompanying license.htm file for the license that 
//     applies to this source code. (a copy can also be found at: 
//     http://www.thehackerwithin.com/license.htm)
//   </copyright>
// -------------------------------------------------------------------------------
namespace DirectEveTester
{
    using System;
    using System.Linq;
    using System.Threading;
    using DirectEve;
    using InnerSpaceAPI;

    internal static class Program
    {
        private static bool _done;
        private static DirectEve _directEve;

        private static long _frameCount = 0;

        /// <summary>
        ///   The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Log("Starting test...");
            _directEve = new DirectEve();
            _directEve.OnFrame += OnFrame;

            // Sleep until we're done
            while (!_done)
                Thread.Sleep(50);

            _directEve.Dispose();
            Log("Test finished.");
        }

        private static void Log(string format, params object[] parms)
        {
            InnerSpace.Echo(string.Format("{0:D} {1:HH:mm:ss} {2}", _frameCount, DateTime.Now, string.Format(format, parms)));
        }

        private static void OnFrame(object sender, EventArgs eventArgs)
        {
            _frameCount++;

            if (_done)
                return;

            try
            {
                _directEve.Log("This is a message from DirectEve.Log()");
                System.Diagnostics.Debugger.Launch();


                var scanwindow = _directEve.Windows.OfType<DirectScannerWindow>().FirstOrDefault();
                //scanwindow.GetProbes().FirstOrDefault().SetLocation(0, 0, 0);
                var value = scanwindow.SystemScanResults.FirstOrDefault(i => i.Id == "EOO-800").IsPointResult;
                //var distance = probe.Distance / 149000000000;
               // var dev = probe.Deviation / 149000000000;
                
                //var window2 = _directEve.Windows[1];
                //window2.AnswerModal("Ok");
                //window.Add(items.FirstOrDefault());
                _done = true;
            }
            catch(Exception e)
            {
                Log("Caught exception!! {0}", e);
                _done = true;
            }
        }

    }
}