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
    using global::DirectEve.PySharp;

    public class DirectScanResult : DirectObject
    {
        private PyObject _ball;
        private PyObject _celestialRec;
        private PyObject _slimItem;

        internal DirectScanResult(DirectEve directEve, PyObject slimItem, PyObject ball, PyObject celestialRec)
            : base(directEve)
        {
            _slimItem = slimItem;
            _ball = ball;
            _celestialRec = celestialRec;
        }

        // return these as strings for now
        public string SlimItem { get { return _slimItem.GetPyType().ToString(); } }
        public string Ball { get { return _ball.GetPyType().ToString(); } }
        public string Celestial { get { return _celestialRec.GetPyType().ToString(); } }
    }
}