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

    public class DirectContainerWindow : DirectWindow
    {
        internal DirectContainerWindow(DirectEve directEve, PyObject pyWindow) : base(directEve, pyWindow)
        {
            IsReady = (bool) pyWindow.Attribute("startedUp");
            IsOneWay = (bool) pyWindow.Attribute("oneWay");
            ItemId = (long) pyWindow.Attribute("itemID");
            LocationFlag = (int) pyWindow.Attribute("locationFlag");
            HasCapacity = (bool) pyWindow.Attribute("hasCapacity");
        }

        public bool IsReady { get; private set; }
        public bool IsOneWay { get; private set; }

        public long ItemId { get; private set; }
        public int LocationFlag { get; private set; }
        public bool HasCapacity { get; private set; }
    }
}