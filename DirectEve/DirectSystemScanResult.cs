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
    using System;
    using PySharp;

    public class DirectSystemScanResult : DirectObject
    {
        internal PyObject PyResult;

        internal DirectSystemScanResult(DirectEve directEve, PyObject pyResult)
            : base(directEve)
        {
            PyResult = pyResult;
            Id = (string) pyResult.Attribute("id");
            ScanGroupName = (string) pyResult.Attribute("scanGroupName").ToUnicodeString();
            GroupName = (string) pyResult.Attribute("groupName").ToUnicodeString();
            TypeName = (string) pyResult.Attribute("typeName").ToUnicodeString();
            SignalStrength = (double) pyResult.Attribute("certainty");
            Deviation = (double) pyResult.Attribute("deviation");
            IsPointResult = (string) PyResult.Attribute("data").Attribute("__class__").Attribute("__name__") == "Vector3";
            IsSpereResult = (string) PyResult.Attribute("data").Attribute("__class__").Attribute("__name__") == "float";
            IsCircleResult = (!IsPointResult && !IsSpereResult);
            if (IsPointResult)
            {
                X = (double?) pyResult.Attribute("data").Attribute("x");
                Y = (double?) pyResult.Attribute("data").Attribute("y");
                Z = (double?) pyResult.Attribute("data").Attribute("z");
            }
            else if (IsCircleResult)
            {
                X = (double?) pyResult.Attribute("data").Attribute("point").Attribute("x");
                Y = (double?) pyResult.Attribute("data").Attribute("point").Attribute("y");
                Z = (double?) pyResult.Attribute("data").Attribute("point").Attribute("z");
            }

            // If SphereResult: X,Y,Z is probe location

            if (X.HasValue && Y.HasValue && Z.HasValue)
            {
                var myship = directEve.ActiveShip.Entity;
                Distance = Math.Sqrt((X.Value - myship.X)*(X.Value - myship.X) + (Y.Value - myship.Y)*(Y.Value - myship.Y) + (Z.Value - myship.Z)*(Z.Value - myship.Z));
            }
        }

        public string Id { get; internal set; }
        public string ScanGroupName { get; internal set; }
        public string GroupName { get; internal set; }
        public double SignalStrength { get; internal set; }
        public string TypeName { get; internal set; }
        public double? Distance { get; internal set; }
        public double? X { get; internal set; }
        public double? Y { get; internal set; }
        public double? Z { get; internal set; }
        public double Deviation { get; internal set; }
        public bool IsPointResult { get; internal set; }
        public bool IsSpereResult { get; internal set; }
        public bool IsCircleResult { get; internal set; }

        public bool BookmarkScanResult(string title, string comment = "", bool corp = false)
        {
            if (corp)
                return DirectEve.ThreadedLocalSvcCall("bookmarkSvc", "BookmarkScanResult", DirectEve.Session.SolarSystemId.Value, title, comment, Id, DirectEve.Session.CorporationId);
            else
                return DirectEve.ThreadedLocalSvcCall("bookmarkSvc", "BookmarkScanResult", DirectEve.Session.SolarSystemId.Value, title, comment, Id, DirectEve.Session.CharacterId);
        }

        public bool WarpTo()
        {
            if (SignalStrength == 1)
            {
                return DirectEve.ThreadedLocalSvcCall("menu", "WarpToScanResult", Id);
            }
            return false;
        }

        // I don't see any reason why we should read out the UI instead of the cached probeData.
        // The X,Y,Z values are not available in the UI, but i need them for scan probing to work.
        // If it's an issue using probeData instead of the current UI reading, let me know asap and we revert it ~ Ferox
        // 
        /*
        private Dictionary<string, PyObject> _node;
        private string _id;
        private string _scanGroup;
        private string _group;
        private string _type;
        private float? _signalStrength;
        private float? _distance;

        internal DirectSystemScanResult(DirectEve directEve, PyObject node)
            : base(directEve)
        {
            _node = node.ToDictionary<string>();
        }

        public string ID
        {
            get
            {
                if (string.IsNullOrEmpty(_id))
                {
                    if (_node.ContainsKey("sortData"))
                    {
                        _id = (string)_node["sortData"].ToList()[0];
                    }
                }
                return _id;
            }
        }

        public string ScanGroup
        {
            get
            {
                if (string.IsNullOrEmpty(_scanGroup))
                {
                    if (_node.ContainsKey("sortData"))
                    {
                        _scanGroup = (string)_node["sortData"].ToList()[1];
                    }
                }
                return _scanGroup;
            }
        }
        public string Group
        {
            get
            {
                if (string.IsNullOrEmpty(_group))
                {
                    if (_node.ContainsKey("sortData"))
                    {
                        _group = (string)_node["sortData"].ToList()[2];
                    }
                }
                return _group;
            }
        }
        public string Type
        {
            get
            {
                if (string.IsNullOrEmpty(_type))
                {
                    if (_node.ContainsKey("sortData"))
                    {
                        _type = (string)_node["sortData"].ToList()[3];
                    }
                }
                return _type;
            }
        }
        public float SignalStrength
        {
            get
            {
                if (!_signalStrength.HasValue) 
                {
                    if (_node.ContainsKey("sortData"))
                    {
                        _signalStrength = (float?)_node["sortData"].ToList()[4];
                    }
                    else
                    {
                        _signalStrength = (float?)0.0;
                    }
                }
                return _signalStrength.Value;
            }
        }
        public float Distance
        {
            get
            {
                if (!_distance.HasValue)
                {
                    if (_node.ContainsKey("sortData"))
                    {
                        _distance = (float?)_node["sortData"].ToList()[5];
                    }
                    else
                    {
                        _distance = (float?)0.0;
                    }
                }
                return _distance.Value;
            }
        }
         
        public bool WarpTo()
        {
            if( _node.ContainsKey("result") )
            {
                string id = (string)_node["result"].Attribute("id");
                return DirectEve.ThreadedLocalSvcCall("menu", "WarpToScanResult", id);
            }
            return false;
        }

        // A node item is a KeyVal object (see carbon\common\lib\util.py)
        // A scan result node will have a item with key=result
        // The texts item contains a list of strings exactly as shown in game

        public string DumpData()
        {
            string str = "";

            try
            {
                str += string.Join(", ",_node["texts"].ToList<string>().ToArray()) + "\n";
                var result = _node["result"];
                str += "warpable = " + (bool)PySharp.Import("scanner").Call("IsResultWithinWarpDistance", result) + "\n";
                str += "result.id = " + (string)result.Attribute("id") + "\n";
                foreach (var x in result.Attributes())
                {
                    str += "( " + string.Join(", ", new string[] { x.Key, (string)x.Value.GetPyType().ToString() }) + " )\n";
                }
            }
            catch
            {
                str += "exception";
            }

            return str;
        }

        */
    }
}