using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirectEve
{
    using global::DirectEve.PySharp;

    public class DirectRepairShopWindow : DirectWindow
    {
        internal DirectRepairShopWindow(DirectEve directEve, PyObject pyWindow)
            : base(directEve, pyWindow)
        {
        }

        public bool RepairItems(List<DirectItem> items)
        {
            var PyItems = items.Select(i => i.PyItem);
            return DirectEve.ThreadedCall(PyWindow.Attribute("DisplayRepairQuote"), PyItems);
        }

        public bool RepairAll()
        {
            return DirectEve.ThreadedCall(PyWindow.Attribute("RepairAll"));
        }

        public string AvgDamage()
        {
            try
            {
                return (string)PyWindow.Attribute("sr").Attribute("avgDamage").Attribute("text");
            }
            catch (Exception e)
            {
                return "";
            }

        }



    }
}