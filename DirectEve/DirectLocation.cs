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
        ///   Get a location name
        /// </summary>
        /// <param name = "directEve"></param>
        /// <param name = "locationId"></param>
        /// <returns></returns>
        internal static string GetLocationName(DirectEve directEve, long locationId)
        {
            return (string) directEve.PySharp.Import("__builtin__").Attribute("cfg").Attribute("evelocations").Call("GetIfExists", locationId).Attribute("name");
        }

        /// <summary>
        ///   Get a location based on locationId
        /// </summary>
        /// <param name = "directEve"></param>
        /// <param name = "locationId"></param>
        /// <returns></returns>
        public static DirectLocation GetLocation(DirectEve directEve, long locationId)
        {
            long? itemId = null;

            var items = directEve.GetLocalSvc("map").Attribute("mapcache").DictionaryItem("items");
            var location = items.DictionaryItem(locationId);
            if (!location.IsValid)
            {
                var station = directEve.GetLocalSvc("ui").Attribute("stationsdata").Attribute("items").DictionaryItem(locationId);

                // Not a station and not a item/solarsystem/constellation/region? return an invalid location
                if (!station.IsValid)
                    return new DirectLocation(directEve) {LocationId = locationId};

                // Item 1 == solarSystemId
                var solarSystemId = station.Item(1);
                itemId = (long?) station.Item(0);

                // Get the new location
                location = items.DictionaryItem(solarSystemId);
            }

            // Get the hierarchy data
            var hierarchy = location.Attribute("hierarchy");

            // Build a new DirectLocation with the info that we found
            var result = new DirectLocation(directEve);
            result.IsValid = location.IsValid;
            result.LocationId = locationId;
            result.RegionId = (long?) hierarchy.Item(0);
            result.ConstellationId = (long?) hierarchy.Item(1);
            result.SolarSystemId = (long?) hierarchy.Item(2);
            result.ItemId = (long?) hierarchy.Item(3) ?? itemId;
            result.Name = GetLocationName(directEve, locationId);
            return result;
        }

        /// <summary>
        ///   Set location as destination
        /// </summary>
        /// <returns></returns>
        public bool SetDestination()
        {
            return AddWaypoint(true, true);
        }

        /// <summary>
        ///   Add a waypoint
        /// </summary>
        /// <returns></returns>
        public bool AddWaypoint()
        {
            return AddWaypoint(false, false);
        }

        /// <summary>
        ///   Add a waypoint
        /// </summary>
        /// <param name = "clearOtherWaypoints"></param>
        /// <param name = "firstWaypoint"></param>
        /// <returns></returns>
        public bool AddWaypoint(bool clearOtherWaypoints, bool firstWaypoint)
        {
            if (SolarSystemId == null)
                return false;

            return DirectEve.ThreadedLocalSvcCall("starmap", "SetWaypoint", SolarSystemId.Value, clearOtherWaypoints, firstWaypoint);
        }
    }
}