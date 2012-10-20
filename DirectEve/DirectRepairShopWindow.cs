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
                return (string)PyWindow.Attribute("sr").Attribute("avgDamage").Attribute("text");
            }
            catch (Exception e)
            {
                return "";
            }

        }



    }
}