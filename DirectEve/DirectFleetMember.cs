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
    using System.Linq;
    using global::DirectEve.PySharp;

    public class DirectFleetMember : DirectObject
    {
        internal DirectFleetMember(DirectEve directEve, PyObject memberObject)
            : base(directEve)
        {
            CharacterId = (int)memberObject.Attribute("charID");
            SquadID = (long)memberObject.Attribute("squadID");
            WingID = (long)memberObject.Attribute("wingID");
            Skills = new List<int> { (int)memberObject.Attribute("skills").ToList()[0],
                (int)memberObject.Attribute("skills").ToList()[1],
                (int)memberObject.Attribute("skills").ToList()[2] };

            if ((int)memberObject.Attribute("job") == (int)directEve.Const.FleetJobCreator)
                Job = DirectFleetMember.JobRole.Boss;
            else
                Job = DirectFleetMember.JobRole.RegularMember;

            if ((int)memberObject.Attribute("role") == (int)directEve.Const.FleetRoleLeader)
                Role = DirectFleetMember.FleetRole.FleetCommander;
            else if ((int)memberObject.Attribute("role") == (int)directEve.Const.FleetRoleWingCmdr)
                Role = DirectFleetMember.FleetRole.WingCommander;
            else if ((int)memberObject.Attribute("role") == (int)directEve.Const.FleetRoleSquadCmdr)
                Role = DirectFleetMember.FleetRole.SquadCommander;
            else if ((int)memberObject.Attribute("role") == (int)directEve.Const.FleetRoleMember)
                Role = DirectFleetMember.FleetRole.Member;

            ShipTypeID = (int?)memberObject.Attribute("shipTypeID");
            SolarSystemID = (int)memberObject.Attribute("solarSystemID");
        }

        public int CharacterId { get; internal set; }
        public long SquadID { get; internal set; }
        public long WingID { get; internal set; }
        public List<int> Skills { get; internal set; }
        public JobRole Job { get; internal set; }
        public FleetRole Role { get; internal set; }
        public int? ShipTypeID { get; internal set; }
        public long SolarSystemID { get; internal set; }

        public string Name
        {
            get { return DirectEve.GetOwner(CharacterId).Name; }
        }

        public bool WarpToMember(double distance = 0)
        {
            return DirectEve.ThreadedLocalSvcCall("menu", "WarpToMember", this.CharacterId, distance);
        }

        public enum FleetRole
        {
            FleetCommander,
            WingCommander,
            SquadCommander,
            Member
        }

        public enum JobRole
        {
            Boss,
            RegularMember
        }


    }
}