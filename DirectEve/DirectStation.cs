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
    using PySharp;

    public class DirectStation : DirectInvType
    {
        private DirectSolarSystem _solarSystem;

        internal DirectStation(DirectEve directEve, PyObject pyo) : base(directEve)
        {
            Id = (int) pyo.Attribute("stationID");
            Name = (string) pyo.Attribute("stationName");
            X = (double) pyo.Attribute("x");
            Y = (double) pyo.Attribute("y");
            Z = (double) pyo.Attribute("z");
            TypeId = (int) pyo.Attribute("stationTypeID");
            SolarSystemId = (int) pyo.Attribute("solarSystemID");
        }

        public int Id { get; private set; }
        public string Name { get; private set; }
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }

        public int SolarSystemId { get; private set; }

        public DirectSolarSystem SolarSystem
        {
            get
            {
                DirectEve.SolarSystems.TryGetValue(SolarSystemId, out _solarSystem);
                return _solarSystem;
            }
        }

        public static Dictionary<int, DirectStation> GetStations(DirectEve directEve)
        {
            var result = new Dictionary<int, DirectStation>();

            var pyDict = directEve.PySharp.Import("__builtin__").Attribute("cfg").Attribute("stations").Attribute("data").ToDictionary<int>();
            foreach (var pair in pyDict)
                result[pair.Key] = new DirectStation(directEve, pair.Value);

            return result;
        }
    }
}