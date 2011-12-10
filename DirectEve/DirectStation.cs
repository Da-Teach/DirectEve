namespace DirectEve
{
    using System.Linq;
    using System.Collections.Generic;
    using global::DirectEve.PySharp;

    public class DirectStation : DirectInvType
    {
        private DirectSolarSystem _solarSystem;

        internal DirectStation(DirectEve directEve, PyObject pyo) : base(directEve)
        {
            Id = (long)pyo.Attribute("stationID");
            Name = (string)pyo.Attribute("stationName");
            X = (double)pyo.Attribute("x");
            Y = (double)pyo.Attribute("y");
            Z = (double)pyo.Attribute("z");
            TypeId = (int)pyo.Attribute("stationTypeID");
            SolarSystemId = (long)pyo.Attribute("solarSystemID");
        }

        public long Id { get; private set; }
        public string Name { get; private set; }
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }

        public long SolarSystemId { get; private set; }
        public DirectSolarSystem SolarSystem 
        {
            get
            {
                DirectEve.SolarSystems.TryGetValue(SolarSystemId, out _solarSystem);
                return _solarSystem;
            }
        }

        public static Dictionary<long, DirectStation> GetStations(DirectEve directEve)
        {
            var result = new Dictionary<long, DirectStation>();

            var pyDict = directEve.PySharp.Import("__builtin__").Attribute("cfg").Attribute("stations").Attribute("data").ToDictionary<long>();
            foreach(var pair in pyDict)
                result[pair.Key] = new DirectStation(directEve, pair.Value);

            return result;
        }
    }
}
