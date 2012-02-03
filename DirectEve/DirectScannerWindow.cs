// ------------------------------------------------------------------------------
//   <copyright from='2010' to='2015' company='THEHACKERWITHIN.COM'>
//     Copyright (c) TheHackerWithin.COM. All Rights Reserved.
// 
//     Please look in the accompanying license.htm file for the license that 
//     applies to this source code. (a copy can also be found at: 
//     http://www.thehackerwithin.com/license.htm)
//   </copyright>
// -------------------------------------------------------------------------------
namespace DirectEve
{
    using System;
    using System.Collections.Generic;
    using global::DirectEve.PySharp;
    using InnerSpaceAPI;

    public class DirectScannerWindow : DirectWindow
    {
        private List<DirectDirectionalScanResult> _scanResults;

        internal DirectScannerWindow(DirectEve directEve, PyObject pyWindow)
            : base(directEve, pyWindow)
        {
            var charId = DirectEve.Session.CharacterId;
            var obj = PyWindow.Attribute("busy");
            //Log("obj type = " + obj.GetPyType().ToString());
            //Log("obj value = " + ((bool) obj).ToString());
            IsReady = charId != null && obj.IsValid && (bool) obj == false;
        }

        public bool IsReady { get; internal set; }

        public int Range
        {
            get { return (int) PyWindow.Attribute("dir_rangeinput").Call("GetValue"); }
            set { PyWindow.Attribute("dir_rangeinput").Call("SetValue", value.ToString()); }
        }

        /// <summary>
        ///   List all the scan results
        /// </summary>
        /// <remarks>
        /// </remarks>
        public List<DirectDirectionalScanResult> ScanResults
        {
            get
            {
                var charId = DirectEve.Session.CharacterId;
                if (_scanResults == null && charId != null)
                {
                    _scanResults = new List<DirectDirectionalScanResult>();
                    foreach (var result in PyWindow.Attribute("scanresult").ToList())
                    {
                        // scan result is a list of tuples
                        var resultAsList = result.ToList();
                        _scanResults.Add(new DirectDirectionalScanResult(DirectEve, resultAsList[0],
                                                                         resultAsList[1], resultAsList[2]));
                    }
                }

                return _scanResults;
            }
        }

        private static void Log(string line)
        {
            InnerSpace.Echo(string.Format("{0:D} {1:HH:mm:ss} {2}", -1, DateTime.Now, line));
        }

        /// <summary>
        ///   Selects the next tab
        /// </summary>
        /// <returns>true if sucessfull, false otherwise</returns>
        public bool NextTab()
        {
            return DirectEve.ThreadedCall(PyWindow.Attribute("sr").Attribute("tabs").Attribute("SelectNext"));
        }

        /// <summary>
        ///   Selects the previous tab
        /// </summary>
        /// <returns>true if sucessfull, false otherwise</returns>
        public bool PrevTab()
        {
            return DirectEve.ThreadedCall(PyWindow.Attribute("sr").Attribute("tabs").Attribute("SelectPrev"));
        }

        /// <summary>
        ///   Selects a tab by index
        /// </summary>
        /// <returns>true if sucessfull, false otherwise</returns>
        public bool SelectByIdx(int tab)
        {
            return DirectEve.ThreadedCall(PyWindow.Attribute("sr").Attribute("tabs").Attribute("SelectByIdx"), tab);
        }

        /// <summary>
        ///   Returns the selected tab index
        /// </summary>
        /// <returns>the tab index</returns>
        public int GetSelectedIdx()
        {
            return (int) PyWindow.Attribute("sr").Attribute("tabs").Call("GetSelectedIdx");
        }

        /// <summary>
        ///   Performs a directional scan
        /// </summary>
        /// <returns>true if sucessfull, false otherwise</returns>
        public bool DirectionSearch()
        {
            _scanResults = null; // free old results
            return DirectEve.ThreadedCall(PyWindow.Attribute("DirectionSearch"));
        }
    }
}