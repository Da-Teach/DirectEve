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
            _directEve = new DirectEve();
            _directEve.LogEvent += OnLog;
            _directEve.OnFrame += OnFrame;
            Log("Starting test...");

            // Sleep until we're done
            while (!_done)
                Thread.Sleep(50);

            Log("Test finished.");
            _directEve.Dispose();
        }

        private static void Log(string format, params object[] parms)
        {
            _directEve.Log(string.Format("{0:D} {1:HH:mm:ss} {2}", _frameCount, DateTime.Now, string.Format(format, parms)));
        }

        private static void OnLog(object sender, LogEventArgs eventArgs)
        {
            InnerSpace.Echo(eventArgs.Message);
        }

        private static void OnFrame(object sender, EventArgs eventArgs)
        {
            _frameCount++;

            if (_done)
                return;

            try
            {
                Log("AtLogin = {0}", _directEve.Login.AtLogin); 
                //var items = _directEve.GetItemHangar().Items;
                //foreach(var item in items)
                //{
                //    if (item.TypeName != "Small Tractor Beam II")
                //        continue;
                    
                //    Log("{0} {1} {2}", item.TypeName, item.TypeId, item.GroupId);
                //}

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