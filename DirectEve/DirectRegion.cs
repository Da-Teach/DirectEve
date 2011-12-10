namespace DirectEve
{
    using System.Collections.Generic;
    using System.Linq;
    using global::DirectEve.PySharp;

    public class DirectRegion : DirectObject
    {
        private List<DirectConstellation> _constellations;

        internal DirectRegion(DirectEve directEve, PyObject pyo)
            : base(directEve)
        {
            Id = (long)pyo.Attribute("regionID");
            Name = (string)pyo.Attribute("regionName");
            Description = (string)pyo.Attribute("description");
            FactionId = (long?)pyo.Attribute("factionID");
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

            var pyDict = directEve.PySharp.Import("__builtin__").Attribute("cfg").Attribute("regions").Attribute("data").ToDictionary<long>();
            foreach (var pair in pyDict)
                result[pair.Key] = new DirectRegion(directEve, pair.Value);

            return result;
        }
    }
}
