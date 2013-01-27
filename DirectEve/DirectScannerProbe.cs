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
    using System.Collections.Generic;
    using global::DirectEve.PySharp;
    using System.Linq;

    public class DirectScannerProbe : DirectObject
    {

        internal PyObject PyProbe;

        internal DirectScannerProbe(DirectEve directEve, PyObject pyProbe)
            : base(directEve)
        {
            PyProbe = pyProbe;
            TypeId = (int)pyProbe.Attribute("typeID");
            ProbeId = (long)pyProbe.Attribute("probeID");
            X = (double)pyProbe.Attribute("pos").Attribute("x");
            Y = (double)pyProbe.Attribute("pos").Attribute("y");
            Z = (double)pyProbe.Attribute("pos").Attribute("z");
            Expiry = new TimeSpan((long)pyProbe.Attribute("expire"));
            RangeAu = (double)pyProbe.Attribute("scanRange") / (double)directEve.Const.AU;
            AllRangesAu = DirectEve.GetLocalSvc("scanSvc").Call("GetScanRangeStepsByTypeID", TypeId).ToList<double>().Select(i => i / (double)directEve.Const.AU).ToList();
        }

        public int TypeId { get; internal set; }
        public long ProbeId { get; internal set; }
        public double X { get; internal set; }
        public double Y { get; internal set; }
        public double Z { get; internal set; }
        public TimeSpan Expiry { get; internal set; }
        public double RangeAu { get; internal set; }
        public List<double> AllRangesAu { get; internal set; }
        
        public void SetLocation(double x, double y, double z)
        {
            PyProbe.Attribute("destination").SetAttribute("x", x);
            PyProbe.Attribute("destination").SetAttribute("y", y);
            PyProbe.Attribute("destination").SetAttribute("z", z);
        }

        public bool SetProbeRangeAu(double range)
        {
            if (!AllRangesAu.Any(i => i == range))
                return false;
            var stepNumber = AllRangesAu.FindIndex(i => i == range) + 1;

            return DirectEve.ThreadedCall(DirectEve.GetLocalSvc("scanSvc").Attribute("SetProbeRangeStep"), ProbeId, stepNumber);            
        }

        public bool RecoverProbe()
        {
            return DirectEve.ThreadedCall(DirectEve.GetLocalSvc("scanSvc").Attribute("RecoverProbe"), ProbeId);
        }

        public bool DestroyProbe()
        {
            return DirectEve.ThreadedCall(DirectEve.GetLocalSvc("scanSvc").Attribute("DestroyProbe"), ProbeId);
        }
       
    }
}