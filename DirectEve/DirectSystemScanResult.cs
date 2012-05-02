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
    using global::DirectEve.PySharp;

    public class DirectSystemScanResult : DirectObject
    {
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
    }
}
