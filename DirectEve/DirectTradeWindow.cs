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
    using global::DirectEve.PySharp;

    public class DirectTradeWindow : DirectWindow
    {

        internal DirectTradeWindow(DirectEve directEve, PyObject pyWindow)
            : base(directEve, pyWindow)
        {   
        }

        public List<DirectItem> MyTradeItems
        {
            get
            {
                List<DirectItem> items = new List<DirectItem>();
                var pyItems = PyWindow.Attribute("sr").Attribute("my").Attribute("items").ToList();
                foreach (var pyItem in pyItems)
                {
                    if (!pyItem.IsValid) //the list ends with 2x None
                        continue;

                    var item = new DirectItem(DirectEve);
                    item.PyItem = pyItem;
                    items.Add(item);
                }
                return items;
            }
        }

        public List<DirectItem> HerTradeItems
        {
            get
            {
                List<DirectItem> items = new List<DirectItem>();
                var pyItems = PyWindow.Attribute("sr").Attribute("her").Attribute("items").ToList();
                foreach (var pyItem in pyItems)
                {
                    if (!pyItem.IsValid) //the list ends with 2x None
                        continue;

                    var item = new DirectItem(DirectEve);
                    item.PyItem = pyItem;
                    items.Add(item);
                }
                return items;
            }
        }

        public bool Add(DirectItem item)
        {
            if (item.LocationId == -1)
                return false;

            //This method instead of _AddItem to prevent quantity popup
            return DirectEve.ThreadedCall(PyWindow.Attribute("sr").Attribute("my").Attribute("invController").Attribute("_BaseInvContainer__AddItem"), item.ItemId, item.LocationId, item.Quantity);
        }

        public bool OfferMoney(double amount)
        {
            return DirectEve.ThreadedCall(PyWindow.Attribute("tradeSession").Attribute("OfferMoney"), amount);
        }

        public bool ToggleAccept()
        {
            return DirectEve.ThreadedCall(PyWindow.Attribute("OnClickAccept"));
        }

        public bool MyAccepted
        {
            get
            {
                return (int)PyWindow.Attribute("sr").Attribute("myAccept").Attribute("state") == 1;
            }
        }

        public bool HerAccepted
        {
            get
            {
                return (int)PyWindow.Attribute("sr").Attribute("herAccept").Attribute("state") == 1;
            }
        }

        public int HerCharacterId
        {
            get
            {
                return (int)PyWindow.Attribute("sr").Attribute("herinfo").Attribute("ownerID");
            }
        }

        public string HerName
        {
            get
            {
                return (string)PyWindow.Attribute("sr").Attribute("herinfo").Attribute("ownerName").ToUnicodeString();
            }
        }

        public string MyOfferedIsk
        {
            get
            {
                return (string)PyWindow.Attribute("sr").Attribute("myMoney").Attribute("text").ToUnicodeString();
            }
        }

        public string HerOfferedIsk
        {
            get
            {
                return (string)PyWindow.Attribute("sr").Attribute("herMoney").Attribute("text").ToUnicodeString();
            }
        }

    }
}