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

    public class DirectSession : DirectObject
    {
        /// <summary>
        ///   Now cache
        /// </summary>
        private DateTime? _now;

        internal DirectSession(DirectEve directEve) : base(directEve)
        {
        }

        private PyObject Session
        {
            get { return PySharp.Import("__builtin__").Attribute("eve").Attribute("session"); }
        }

        public long? CharacterId
        {
            get { return (long?) Session.Attribute("charid"); }
        }

        public DirectOwner Character
        {
            get { return DirectEve.GetOwner(CharacterId ?? -1); }
        }

        public long? CorporationId
        {
            get { return (long?) Session.Attribute("corpid"); }
        }

        public long? AllianceId
        {
            get { return (long?) Session.Attribute("allianceid"); }
        }

        public long? ShipId
        {
            get { return (long?) DirectEve.PySharp.Import("util").Call("GetActiveShip"); }
        }

        public long? FleetId
        {
            get { return (long?) Session.Attribute("fleetid"); }
        }

        public long? StationId
        {
            get { return (long?) Session.Attribute("stationid"); }
        }

        public long? LocationId
        {
            get { return (long?) Session.Attribute("locationid"); }
        }

        public long? SolarSystemId
        {
            get { return (long?) Session.Attribute("solarsystemid2"); }
        }

        public long? ConstellationId
        {
            get { return (long?) Session.Attribute("constellationid"); }
        }

        public long? RegionId
        {
            get { return (long?) Session.Attribute("regionid"); }
        }

        public DateTime Now
        {
            get
            {
                if (!_now.HasValue)
                    _now = (DateTime) PySharp.Import("blue").Attribute("os").Call("GetSimTime");

                return _now.Value;
            }
        }

        public bool IsInSpace
        {
            get { return LocationId.HasValue && LocationId == SolarSystemId; }
        }

        public bool IsInStation
        {
            get { return LocationId.HasValue && LocationId == StationId; }
        }

        public bool IsReady
        {
            get
            {
                if (ShipId == null)
                    return false;

                if (!Session.Attribute("locationid").IsValid)
                    return false;

                if (!Session.Attribute("solarsystemid2").IsValid)
                    return false;

                if (Session.Attribute("changing").IsValid)
                    return false;

                if ((bool) Session.Attribute("mutating"))
                    return false;

                if (Session.Attribute("nextSessionChange").IsValid)
                {
                    // Wait 10 seconds after a session change
                    var nextSessionChange = (DateTime) Session.Attribute("nextSessionChange");
                    nextSessionChange = nextSessionChange.AddSeconds(-20);
                    if (nextSessionChange >= Now)
                        return false;
                }

                var station = DirectEve.GetLocalSvc("station");
                if (station.IsValid)
                {
                    if ((bool) station.Attribute("activatingShip"))
                        return false;

                    if ((bool) station.Attribute("exitingstation"))
                        return false;

                    if ((bool) station.Attribute("dockaborted"))
                        return false;

                    if ((bool) station.Attribute("loading"))
                        return false;

                    if ((bool) station.Attribute("leavingShip"))
                        return false;
                }

                var loading = (bool) DirectEve.PySharp.Import("__builtin__").Attribute("uicore").Attribute("layer").Attribute("loading").Attribute("display");
                if (loading)
                    return false;

                return true;
            }
        }
    }
}