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
    using System.Linq;
    using PySharp;

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

        /// <summary>
        ///     Find a child object (usually button)
        /// </summary>
        /// <param name="container"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        internal new static PyObject FindChild(PyObject container, string name)
        {
            var childs = container.Attribute("children").Attribute("_childrenObjects").ToList();
            return childs.Find(c => String.Compare((string) c.Attribute("name"), name) == 0) ?? global::DirectEve.PySharp.PySharp.PyZero;
        }

        /// <summary>
        ///     Find a child object (using the supplied path)
        /// </summary>
        /// <param name="container"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        internal new static PyObject FindChildWithPath(PyObject container, IEnumerable<string> path)
        {
            return path.Aggregate(container, FindChild);
        }
    }
}