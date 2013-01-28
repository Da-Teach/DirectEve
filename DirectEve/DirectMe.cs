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

        public bool IsTrialAccount
        {
            get
            {
                return DirectEve.Session.UserType == 23;
            }
        }

        /// <summary>
        /// Retrieves days left on account after login
        /// </summary>
        public int DaysLeftOnAccount
        {
            get
            {
                long? charid = DirectEve.Session.CharacterId;
                if (!charid.HasValue)
                    return -1;

                var daysLeft = (int?)PySharp.Import("__builtin__").Attribute("uicore").Attribute("layer").Attribute("charsel").Attribute("details").ToDictionary<long>()[charid.Value].Attribute("daysLeft");

                if (daysLeft.HasValue)
                    return daysLeft.Value;
                else
                    return -1;
            }
        }

        /// <summary>
        /// Are we in an active war?
        /// </summary>
        /// <returns></returns>
        public bool IsAtWar
        {
            get
            {
                long? id = DirectEve.Session.AllianceId;
                if (id == null)
                    id = DirectEve.Session.CorporationId;

                var atWar = (int)DirectEve.GetLocalSvc("war").Attribute("wars").Call("AreInAnyHostileWarStates", id);
                if (atWar == 1)
                    return true;
                else
                    return false;
            }
        }
    }
}