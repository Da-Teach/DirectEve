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
    using PySharp;

    public class DirectLoyaltyPointStoreWindow : DirectWindow
    {
        private List<DirectLoyaltyPointOffer> _offers;

        internal DirectLoyaltyPointStoreWindow(DirectEve directEve, PyObject pyWindow) :
            base(directEve, pyWindow)
        {
        }

        public long LoyaltyPoints
        {
            get { return (long) DirectEve.GetLocalSvc("lpstore").Attribute("cache").Attribute("lps"); }
        }

        public List<DirectLoyaltyPointOffer> Offers
        {
            get
            {
                if (_offers == null)
                {
                    _offers = new List<DirectLoyaltyPointOffer>();
                    foreach (var offer in DirectEve.GetLocalSvc("lpstore").Attribute("cache").Attribute("offers").ToList())
                        _offers.Add(new DirectLoyaltyPointOffer(DirectEve, offer));
                }

                return _offers;
            }
        }

        public bool RefreshLoyaltyPoints()
        {
            // Delete saved LPs
            DirectEve.GetLocalSvc("lpstore").Attribute("cache").SetAttribute("lps", global::DirectEve.PySharp.PySharp.PyNone);
            return DirectEve.ThreadedLocalSvcCall("lpstore", "GetMyLPs");
        }
    }
}