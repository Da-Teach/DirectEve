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
            OrderId = -1;
            Price = (double) pyOrder.Attribute("price");
            VolumeRemaining = (int) pyOrder.Attribute("volRemaining");
            TypeId = (int) pyOrder.Attribute("typeID");
            if ((int)pyOrder.Attribute("range") == (int)DirectEve.Const.RangeSolarSystem)
                Range = DirectOrderRange.SolarSystem;
            else if ((int)pyOrder.Attribute("range") == (int)DirectEve.Const.RangeConstellation)
                Range = DirectOrderRange.Constellation;
            else if ((int)pyOrder.Attribute("range") == (int)DirectEve.Const.RangeRegion)
                Range = DirectOrderRange.Region;
            else if ((int)pyOrder.Attribute("range") == (int)DirectEve.Const.RangeStation)
                Range = DirectOrderRange.Station;
            else
                RangeAbsolute = (int)pyOrder.Attribute("range");

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
        public int RangeAbsolute { get; set; }
        public int TypeId { get; set; }
        public int VolumeRemaining { get; set; }
        public double Price { get; set; }


        public bool Buy(int quantity, DirectOrderRange range)
        {
            var pyRange = DirectEve.GetRange(range);
            return DirectEve.ThreadedLocalSvcCall("marketQuote", "BuyStuff", StationId, TypeId, Price, quantity, pyRange);
        }
        
        
        // def CancelOrder(self, orderID, regionID):
        public bool CancelOrder()
        {
            if (!DirectEve.HasSupportInstances())
            {
                DirectEve.Log("DirectEve: Error: This method requires a support instance.");
                return false;
            }
            if (OrderId == -1 || !PyOrder.IsValid)
            {
                DirectEve.Log("Trying to cancel a invalid order");
                return false;
            }
            return DirectEve.ThreadedLocalSvcCall("marketQuote", "CancelOrder", OrderId, RegionId);
        }

        //def ModifyOrder(self, order, newPrice):
        public bool ModifyOrder(double newPrice)
        {
            if (!DirectEve.HasSupportInstances())
            {
                DirectEve.Log("DirectEve: Error: This method requires a support instance.");
                return false;
            }

            if (OrderId == -1 || !PyOrder.IsValid)
            {
                DirectEve.Log("Trying to modify a invalid order");
                return false;
            }
            
            return DirectEve.ThreadedLocalSvcCall("marketQuote", "ModifyOrder", PyOrder, newPrice);
        }
    }
}