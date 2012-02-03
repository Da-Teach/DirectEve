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

    public class DirectTelecomWindow : DirectWindow
    {
        internal DirectTelecomWindow(DirectEve directEve, PyObject pyWindow) : base(directEve, pyWindow)
        {
        }

        public override bool Close()
        {
            //try to find the close Button
            string[] closeButtonPath = {"__maincontainer", "bottom", "btnsmainparent", "btns", "Close_Btn"};
            var btn = FindChildWithPath(PyWindow, closeButtonPath);
            if (btn != null)
                return DirectEve.ThreadedCall(btn.Attribute("OnClick"));
            else
            {
                return false;
            }

            //return DirectEve.ThreadedCall(PyWindow.Attribute("SelfDestruct"));
        }
    }
}