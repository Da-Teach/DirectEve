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
    using System.Linq;
    using global::DirectEve.PySharp;

    public class DirectRegion : DirectObject
    {
        private List<DirectConstellation> _constellations;

        internal DirectRegion(DirectEve directEve, dynamic pyo)
            : base(directEve)
        {
            Id = (long) pyo.regionID;
            Name = (string) pyo.regionName;
            Description = (string) pyo.description;
            FactionId = (long?) pyo.factionID;
        }

        public long Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public long? FactionId { get; private set; }

        /// <summary>
        ///   List all constellations within this region
        /// </summary>
        public List<DirectConstellation> Constellations
        {
            get { return _constellations ?? (_constellations = DirectEve.Constellations.Values.Where(c => c.RegionId == Id).ToList()); }
        }

        public static Dictionary<long, DirectRegion> GetRegions(DirectEve directEve)
        {
            var result = new Dictionary<long, DirectRegion>();

            dynamic ps = directEve.PySharp;
            Dictionary<long,PyObject> pyDict = ps.__builtin__.cfg.regions.data.ToDictionary<long>();
            foreach (var pair in pyDict)
                result[pair.Key] = new DirectRegion(directEve, pair.Value);

            return result;
        }
    }
}