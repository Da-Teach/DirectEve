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
    using PySharp;

    public class DirectSolarSystem : DirectObject
    {
        private DirectConstellation _constellation;
        private List<DirectStation> _stations;

        internal DirectSolarSystem(DirectEve directEve, PyObject pyo)
            : base(directEve)
        {
            Id = (int) pyo.Attribute("solarSystemID");
            Name = (string) DirectEve.PySharp.Import("__builtin__").Attribute("cfg").Attribute("evelocations").Call("Get", Id).Attribute("name");
            ConstellationId = (long) pyo.Attribute("constellationID");
            FactionId = (long?) pyo.Attribute("factionID");
            Security = (double) pyo.Attribute("securityStatus");
            IsWormholeSystem = ((long) directEve.Const.MapWormholeSystemMin < Id && Id < (long) directEve.Const.MapWormholeSystemMax);
        }

        public int Id { get; private set; }
        public string Name { get; private set; }

        public long ConstellationId { get; private set; }

        public DirectConstellation Constellation
        {
            get
            {
                DirectEve.Constellations.TryGetValue(ConstellationId, out _constellation);
                return _constellation;
            }
        }

        public int GetClassOfWormhole()
        {
            var regionId = DirectEve.Constellations[ConstellationId].RegionId;
            return (int) DirectEve.PySharp.Import("__builtin__").Attribute("cfg").Call("GetLocationWormholeClass", Id);
        }

        public long? FactionId { get; private set; }
        public double Security { get; private set; }
        public bool IsWormholeSystem { get; private set; }
        public int WormholeClass { get; private set; }

        /// <summary>
        ///     List all stations within this solar system
        /// </summary>
        public List<DirectStation> Stations
        {
            get { return _stations ?? (_stations = DirectEve.Stations.Values.Where(s => s.SolarSystemId == Id).ToList()); }
        }

        internal static Dictionary<int, DirectSolarSystem> GetSolarSystems(DirectEve directEve)
        {
            var result = new Dictionary<int, DirectSolarSystem>();

            var pyDict = directEve.PySharp.Import("__builtin__").Attribute("cfg").Attribute("mapSystemCache").ToDictionary<int>();
            foreach (var pair in pyDict)
                result[pair.Key] = new DirectSolarSystem(directEve, pair.Value);

            return result;
        }

        internal static int GetDistanceBetweenSolarsystems(int solarsystem1, int solarsystem2, DirectEve directEve)
        {
            return (int) directEve.GetLocalSvc("clientPathfinderService").Call("GetAutopilotJumpCount", solarsystem1, solarsystem2);
        }
    }
}