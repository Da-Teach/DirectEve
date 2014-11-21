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

    public class DirectNavigation : DirectObject
    {
        internal DirectNavigation(DirectEve directEve) : base(directEve)
        {
        }

        /// <summary>
        ///     Returns location name based on locationId
        /// </summary>
        /// <param name="locationId"></param>
        /// <returns></returns>
        public string GetLocationName(long locationId)
        {
            return DirectLocation.GetLocationName(DirectEve, locationId);
        }

        /// <summary>
        ///     Returns a location based on locationId
        /// </summary>
        /// <param name="locationId"></param>
        /// <returns></returns>
        public DirectLocation GetLocation(long locationId)
        {
            return DirectLocation.GetLocation(DirectEve, locationId);
        }

        /// <summary>
        ///     Set destination to locationId
        /// </summary>
        /// <param name="locationId"></param>
        /// <returns></returns>
        /// <remarks>
        ///     GetLocation is used to find the actual solar system
        /// </remarks>
        public bool SetDestination(long locationId)
        {
            return GetLocation(locationId).SetDestination();
        }

        /// <summary>
        ///     Set destination to locationId without actually retrieving the directLocation ~ CPU Intensive
        /// </summary>
        /// <param name="locationId2"></param>
        /// <param name="directEve"></param>
        /// <returns></returns>
        internal static bool SetDestination(long locationId, DirectEve directEve)
        {
            return directEve.ThreadedLocalSvcCall("starmap", "SetWaypoint", locationId, true, true);
        }

        /// <summary>
        ///     Return destination path (locationId's only)
        /// </summary>
        /// <returns></returns>
        public List<int> GetDestinationPath()
        {
            return DirectEve.GetLocalSvc("starmap").Attribute("destinationPath").ToList<int>();
        }
    }
}