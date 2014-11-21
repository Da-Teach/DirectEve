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
    public class DirectLocation : DirectObject
    {
        public DirectLocation(DirectEve directEve) : base(directEve)
        {
        }

        public long LocationId { get; private set; }
        public string Name { get; private set; }

        public long? RegionId { get; private set; }
        public long? ConstellationId { get; private set; }
        public long? SolarSystemId { get; private set; }
        public long? ItemId { get; private set; }

        public bool IsValid { get; private set; }

        /// <summary>
        ///     Get a location name
        /// </summary>
        /// <param name="directEve"></param>
        /// <param name="locationId"></param>
        /// <returns></returns>
        internal static string GetLocationName(DirectEve directEve, long locationId)
        {
            return (string) directEve.PySharp.Import("__builtin__").Attribute("cfg").Attribute("evelocations").Call("GetIfExists", locationId).Attribute("name");
        }

        /// <summary>
        ///     Get a location based on locationId
        /// </summary>
        /// <param name="directEve"></param>
        /// <param name="locationId"></param>
        /// <returns></returns>
        public static DirectLocation GetLocation(DirectEve directEve, long locationId)
        {
            var isValid = false;
            string name = null;
            DirectRegion region = null;
            DirectConstellation constellation = null;
            DirectSolarSystem solarSystem = null;
            DirectStation station = null;

            if (directEve.Regions.TryGetValue(locationId, out region))
            {
                isValid = true;
                name = region.Name;
            }
            else if (directEve.Constellations.TryGetValue(locationId, out constellation))
            {
                isValid = true;
                name = constellation.Name;

                region = constellation.Region;
            }
            else if (directEve.SolarSystems.TryGetValue((int) locationId, out solarSystem))
            {
                isValid = true;
                name = solarSystem.Name;

                constellation = solarSystem.Constellation;
                region = constellation.Region;
            }
            else if (directEve.Stations.TryGetValue((int) locationId, out station))
            {
                isValid = true;
                name = station.Name;

                solarSystem = station.SolarSystem;
                constellation = solarSystem.Constellation;
                region = constellation.Region;
            }

            var result = new DirectLocation(directEve);
            result.IsValid = isValid;
            result.Name = name;
            result.LocationId = locationId;
            result.RegionId = region != null ? region.Id : (long?) null;
            result.ConstellationId = constellation != null ? constellation.Id : (long?) null;
            result.SolarSystemId = solarSystem != null ? solarSystem.Id : (long?) null;
            result.ItemId = station != null ? station.Id : (long?) null;
            return result;
        }

        /// <summary>
        ///     Set location as destination
        /// </summary>
        /// <returns></returns>
        public bool SetDestination()
        {
            return AddWaypoint(true, true);
        }

        /// <summary>
        ///     Add a waypoint
        /// </summary>
        /// <returns></returns>
        public bool AddWaypoint()
        {
            return AddWaypoint(false, false);
        }

        /// <summary>
        ///     Add a waypoint
        /// </summary>
        /// <param name="clearOtherWaypoints"></param>
        /// <param name="firstWaypoint"></param>
        /// <returns></returns>
        public bool AddWaypoint(bool clearOtherWaypoints, bool firstWaypoint)
        {
            if (SolarSystemId == null)
                return false;

            return DirectEve.ThreadedLocalSvcCall("starmap", "SetWaypoint", LocationId, clearOtherWaypoints, firstWaypoint);
        }
    }
}