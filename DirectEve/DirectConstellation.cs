namespace DirectEve
{
    using System.Collections.Generic;
    using System.Linq;
    using global::DirectEve.PySharp;

    public class DirectConstellation : DirectObject
    {
        private DirectRegion _region;
        private List<DirectSolarSystem> _solarSystems;

        internal DirectConstellation(DirectEve directEve, PyObject pyo)
            : base(directEve)
        {
            Id = (long)pyo.Attribute("constellationID");
            Name = (string)pyo.Attribute("constellationName");
            Description = (string)pyo.Attribute("description");
            RegionId = (long)pyo.Attribute("regionID");
            FactionId = (long?)pyo.Attribute("factionID");
        }

        public long Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        
        public long RegionId { get; private set; }
        public DirectRegion Region
        {
            get
            {
                DirectEve.Regions.TryGetValue(RegionId, out _region);
                return _region;
            }
        }

        public long? FactionId { get; private set; }

        /// <summary>
        ///   List all solar systems within this constellation
        /// </summary>
        public List<DirectSolarSystem> SolarSystems
        {
            get { return _solarSystems ?? (_solarSystems = DirectEve.SolarSystems.Values.Where(s => s.ConstellationId == Id).ToList()); }
        }

        public static Dictionary<long, DirectConstellation> GetConstellations(DirectEve directEve)
        {
            var result = new Dictionary<long, DirectConstellation>();

            var pyDict = directEve.PySharp.Import("__builtin__").Attribute("cfg").Attribute("constellations").Attribute("data").ToDictionary<long>();
            foreach (var pair in pyDict)
                result[pair.Key] = new DirectConstellation(directEve, pair.Value);

            return result;
        }
    }
}
