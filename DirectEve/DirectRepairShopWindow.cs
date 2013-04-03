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
    using System.Collections.Generic;
    using System.Linq;
    using PySharp;

    public class DirectRepairShopWindow : DirectWindow
    {
        internal DirectRepairShopWindow(DirectEve directEve, PyObject pyWindow)
            : base(directEve, pyWindow)
        {
        }

        public bool RepairItems(List<DirectItem> items)
        {
            if (!DirectEve.HasSupportInstances())
            {
                DirectEve.Log("DirectEve: Error: This method requires a support instance.");
                return false;
            }
            var PyItems = items.Select(i => i.PyItem);
            return DirectEve.ThreadedCall(PyWindow.Attribute("DisplayRepairQuote"), PyItems);
        }

        public bool RepairAll()
        {
            if (!DirectEve.HasSupportInstances())
            {
                DirectEve.Log("DirectEve: Error: This method requires a support instance.");
                return false;
            }
            return DirectEve.ThreadedCall(PyWindow.Attribute("RepairAll"));
        }

        public string AvgDamage()
        {
            if (!DirectEve.HasSupportInstances())
            {
                DirectEve.Log("DirectEve: Error: This method requires a support instance.");
                return null;
            }

            try
            {
                return (string) PyWindow.Attribute("sr").Attribute("avgDamage").Attribute("text");
            }
            catch
            {
                return "";
            }
        }
    }
}