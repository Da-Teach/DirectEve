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

    public class DirectLoyaltyPointOfferRequiredItem : DirectInvType
    {
        internal DirectLoyaltyPointOfferRequiredItem(DirectEve directEve, PyObject item) : base(directEve)
        {
            TypeId = (int) item.Item(0);
            Quantity = (long) item.Item(1);
        }

        public long Quantity { get; private set; }
    }
}