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

    public class DirectMarketWindow : DirectWindow
    {
        private List<DirectOrder> _buyOrders;
        private List<DirectOrder> _sellOrders;

        internal DirectMarketWindow(DirectEve directEve, PyObject pyWindow) : base(directEve, pyWindow)
        {
            IsReady = !(bool) pyWindow.Attribute("sr").Attribute("market").Attribute("loadingType");
            DetailTypeId = (int?) pyWindow.Attribute("sr").Attribute("market").Attribute("sr").Attribute("detailTypeID");
        }

        public bool IsReady { get; internal set; }
        public int? DetailTypeId { get; internal set; }

        public List<DirectOrder> SellOrders
        {
            get
            {
                if (_sellOrders == null)
                    _sellOrders = DetailTypeId.HasValue ? GetOrders(true) : new List<DirectOrder>();

                return _sellOrders;
            }
        }

        public List<DirectOrder> BuyOrders
        {
            get
            {
                if (_buyOrders == null)
                    _buyOrders = DetailTypeId.HasValue ? GetOrders(false) : new List<DirectOrder>();

                return _buyOrders;
            }
        }

        private List<DirectOrder> GetOrders(bool sellOrders)
        {
            var orders = DirectEve.GetLocalSvc("marketQuote").Attribute("orderCache").DictionaryItem(DetailTypeId.Value).Item(sellOrders ? 0 : 1).ToList();
            return orders.Select(order => new DirectOrder(DirectEve, order)).ToList();
        }

        public bool LoadTypeId(int typeId)
        {
            return DirectEve.ThreadedCall(PyWindow.Attribute("LoadTypeID_Ext"), typeId);
        }
    }
}