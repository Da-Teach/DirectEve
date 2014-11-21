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
    using PySharp;

    public class DirectMarketActionWindow : DirectWindow
    {
        internal DirectMarketActionWindow(DirectEve directEve, PyObject pyWindow) : base(directEve, pyWindow)
        {
            IsReady = (bool) pyWindow.Attribute("ready");
            IsBuyAction = Name == "marketbuyaction";
            IsSellAction = Name == "marketsellaction";

            Item = new DirectItem(directEve);
            Item.PyItem = pyWindow.Attribute("sr").Attribute("sellItem");

            var order = pyWindow.Attribute("sr").Attribute("currentOrder");
            Price = (double?) order.Attribute("price");
            RemainingVolume = (double?) order.Attribute("volRemaining");
            Range = (int?) order.Attribute("range");
            OrderId = (long?) order.Attribute("orderID");
            EnteredVolume = (int?) order.Attribute("volEntered");
            MinimumVolume = (int?) order.Attribute("minVolume");
            IsBid = (bool?) order.Attribute("bid");
            Issued = (DateTime?) order.Attribute("issued");
            Duration = (int?) order.Attribute("duration");
            StationId = (long?) order.Attribute("stationID");
            RegionId = (long?) order.Attribute("regionID");
            SolarSystemId = (long?) order.Attribute("solarSystemID");
            Jumps = (int?) order.Attribute("jumps");
        }

        public bool IsReady { get; private set; }
        public bool IsBuyAction { get; private set; }
        public bool IsSellAction { get; private set; }

        public DirectItem Item { get; private set; }

        public double? Price { get; private set; }
        public double? RemainingVolume { get; private set; }
        public int? Range { get; private set; }
        public long? OrderId { get; private set; }
        public int? EnteredVolume { get; private set; }
        public int? MinimumVolume { get; private set; }
        public bool? IsBid { get; private set; }
        public DateTime? Issued { get; private set; }
        public int? Duration { get; private set; }
        public long? StationId { get; private set; }
        public long? RegionId { get; private set; }
        public long? SolarSystemId { get; private set; }
        public int? Jumps { get; private set; }

        /// <summary>
        ///     Accept the action
        /// </summary>
        /// <returns></returns>
        public bool Accept()
        {
            var call = IsBuyAction ? "Buy" : "Sell";
            return DirectEve.ThreadedCall(PyWindow.Attribute(call));
        }

        /// <summary>
        ///     Cancel the action
        /// </summary>
        /// <returns></returns>
        public bool Cancel()
        {
            return DirectEve.ThreadedCall(PyWindow.Attribute("Cancel"));
        }
    }
}