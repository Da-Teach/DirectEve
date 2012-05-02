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

    public class DirectSolarSystem : DirectObject
    {
        private DirectConstellation _constellation;
        private List<DirectStation> _stations;

        internal DirectSolarSystem(DirectEve directEve, PyObject pyo)
            : base(directEve)
        {
            Id = (long) pyo.Attribute("solarSystemID");
            Name = (string) pyo.Attribute("solarSystemName");
            Description = (string) pyo.Attribute("description");
            ConstellationId = (long) pyo.Attribute("constellationID");
            FactionId = (long?) pyo.Attribute("factionID");
            Security = (double) pyo.Attribute("security");
        }

        public long Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }

        public long ConstellationId { get; private set; }

        public DirectConstellation Constellation
        {
            get
            {
                DirectEve.Constellations.TryGetValue(ConstellationId, out _constellation);
                return _constellation;
            }
        }

        public long? FactionId { get; private set; }
        public double Security { get; private set; }

        /// <summary>
        ///   List all stations within this solar system
        /// </summary>
        public List<DirectStation> Stations
        {
            get { return _stations ?? (_stations = DirectEve.Stations.Values.Where(s => s.SolarSystemId == Id).ToList()); }
        }

        public static Dictionary<long, DirectSolarSystem> GetSolarSystems(DirectEve directEve)
        {
            var result = new Dictionary<long, DirectSolarSystem>();

            var pyDict = directEve.PySharp.Import("__builtin__").Attribute("cfg").Attribute("solarsystems").Attribute("data").ToDictionary<long>();
            foreach (var pair in pyDict)
                result[pair.Key] = new DirectSolarSystem(directEve, pair.Value);

            return result;
        }
    }
}