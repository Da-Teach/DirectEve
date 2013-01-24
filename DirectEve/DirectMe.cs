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
    public class DirectMe : DirectObject
    {
        /// <summary>
        ///   Attribute cache
        /// </summary>
        private DirectItemAttributes _attributes;

        internal DirectMe(DirectEve directEve) : base(directEve)
        {
            _attributes = new DirectItemAttributes(directEve, directEve.Session.CharacterId ?? -1);
        }

        public string Name
        {
            get { return DirectEve.GetOwner(DirectEve.Session.CharacterId ?? -1).Name; }
        }

        public int MaxLockedTargets
        {
            get { return _attributes.TryGet<int>("maxLockedTargets"); }
        }

        public int MaxActiveDrones
        {
            get { return _attributes.TryGet<int>("maxActiveDrones"); }
        }

        public double Wealth
        {
            get { return (double) DirectEve.GetLocalSvc("wallet").Attribute("wealth"); }
        }
    }
}