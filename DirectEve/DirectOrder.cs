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
    using global::DirectEve.PySharp;

    public class DirectOrder : DirectObject
    {
        internal PyObject PyOrder;

        internal DirectOrder(DirectEve directEve, PyObject pyOrder) : base(directEve)
        {
            PyOrder = pyOrder;
            Price = (double) pyOrder.Attribute("price");
            VolumeRemaining = (int) pyOrder.Attribute("volRemaining");
            TypeId = (int) pyOrder.Attribute("typeID");
            if ((int) pyOrder.Attribute("range") == (int) DirectEve.Const.RangeSolarSystem)
                Range = DirectOrderRange.SolarSystem;
            else if ((int) pyOrder.Attribute("range") == (int) DirectEve.Const.RangeConstellation)
                Range = DirectOrderRange.Constellation;
            else if ((int) pyOrder.Attribute("range") == (int) DirectEve.Const.RangeRegion)
                Range = DirectOrderRange.Region;
            else
                Range = DirectOrderRange.Station;
            OrderId = (long) pyOrder.Attribute("orderID");
            VolumeEntered = (int) pyOrder.Attribute("volEntered");
            MinimumVolume = (int) pyOrder.Attribute("minVolume");
            IsBid = (bool) pyOrder.Attribute("bid");
            IssuedOn = (DateTime) pyOrder.Attribute("issued");
            Duration = (int) pyOrder.Attribute("duration");
            StationId = (int) pyOrder.Attribute("stationID");
            RegionId = (int) pyOrder.Attribute("regionID");
            SolarSystemId = (int) pyOrder.Attribute("solarSystemID");
            Jumps = (int) pyOrder.Attribute("jumps");
        }

        public int Jumps { get; set; }
        public int SolarSystemId { get; set; }
        public int RegionId { get; set; }
        public int StationId { get; set; }
        public int Duration { get; set; }
        public DateTime IssuedOn { get; set; }
        public bool IsBid { get; set; }
        public int MinimumVolume { get; set; }
        public int VolumeEntered { get; set; }
        public long OrderId { get; set; }
        public DirectOrderRange Range { get; set; }
        public int TypeId { get; set; }
        public int VolumeRemaining { get; set; }
        public double Price { get; set; }

        private PyObject GetRange(DirectOrderRange range)
        {
            switch (range)
            {
                case DirectOrderRange.SolarSystem:
                    return DirectEve.Const.RangeSolarSystem;
                case DirectOrderRange.Constellation:
                    return DirectEve.Const.RangeConstellation;
                case DirectOrderRange.Region:
                    return DirectEve.Const.RangeRegion;
                default:
                    return DirectEve.Const.RangeStation;
            }
        }

        public bool Sell(DirectItem item, int quantity, DirectOrderRange range)
        {
            if (!item.PyItem.IsValid)
                return false;

            var pyRange = GetRange(range);
            return DirectEve.ThreadedLocalSvcCall("marketQuote", "SellStuff", StationId, TypeId, item.PyItem, quantity, pyRange);
        }

        public bool Buy(int quantity, DirectOrderRange range)
        {
            var pyRange = GetRange(range);
            return DirectEve.ThreadedLocalSvcCall("marketQuote", "BuyStuff", StationId, TypeId, Price, quantity, pyRange);
        }
    }
}