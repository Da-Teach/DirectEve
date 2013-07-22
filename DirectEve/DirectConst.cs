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
    using global::DirectEve.PySharp;

    internal class DirectConst : DirectObject
    {
        internal DirectConst(DirectEve directEve) : base(directEve)
        {
        }

        private PyObject Const
        {
            get { return PySharp.Import("__builtin__").Attribute("const"); }
        }

        public PyObject this[string flag]
        {
            get { return Const.Attribute(flag); }
        }

        public PyObject ContainerHangar
        {
            get { return Const.Attribute("containerHangar"); }
        }

        public PyObject FlagHangar
        {
            get { return Const.Attribute("flagHangar"); }
        }

        public PyObject FlagCorpSAG2
        {
            get { return Const.Attribute("flagCorpSAG2"); }
        }

        public PyObject FlagCorpSAG3
        {
            get { return Const.Attribute("flagCorpSAG3"); }
        }

        public PyObject FlagCorpSAG4
        {
            get { return Const.Attribute("flagCorpSAG4"); }
        }

        public PyObject FlagCorpSAG5
        {
            get { return Const.Attribute("flagCorpSAG5"); }
        }

        public PyObject FlagCorpSAG6
        {
            get { return Const.Attribute("flagCorpSAG6"); }
        }

        public PyObject FlagCorpSAG7
        {
            get { return Const.Attribute("flagCorpSAG7"); }
        }

        public PyObject FlagCargo
        {
            get { return Const.Attribute("flagCargo"); }
        }

        public PyObject FlagOreHold
        {
            get { return Const.Attribute("flagSpecializedOreHold"); }
        }

        public PyObject ContainerGlobal
        {
            get { return Const.Attribute("containerGlobal"); }
        }

        public PyObject FlagDroneBay
        {
            get { return Const.Attribute("flagDroneBay"); }
        }

        public int FlagSkillInTraining
        {
            get { return (int) Const.Attribute("flagSkillInTraining"); }
        }

        public PyObject FlagUnlocked
        {
            get { return Const.Attribute("flagUnlocked"); }
        }

        public PyObject CategoryShip
        {
            get { return Const.Attribute("categoryShip"); }
        }

        public PyObject CategoryModule
        {
            get { return Const.Attribute("categoryModule"); }
        }

        public PyObject CategorySkill
        {
            get { return Const.Attribute("categorySkill"); }
        }

        public PyObject CategoryStructure
        {
            get { return Const.Attribute("categoryStructure"); }
        }

        public PyObject GroupAuditLogSecureContainer
        {
            get { return Const.Attribute("groupAuditLogSecureContainer"); }
        }

        public PyObject RangeRegion
        {
            get { return Const.Attribute("rangeRegion"); }
        }

        public PyObject RangeConstellation
        {
            get { return Const.Attribute("rangeConstellation"); }
        }

        public PyObject RangeSolarSystem
        {
            get { return Const.Attribute("rangeSolarSystem"); }
        }

        public PyObject RangeStation
        {
            get { return Const.Attribute("rangeStation"); }
        }

        public PyObject FleetJobCreator
        {
            get { return Const.Attribute("fleetJobCreator"); }
        }

        public PyObject FleetRoleLeader
        {
            get { return Const.Attribute("fleetRoleLeader"); }
        }

        public PyObject FleetRoleWingCmdr
        {
            get { return Const.Attribute("fleetRoleWingCmdr"); }
        }

        public PyObject FleetRoleSquadCmdr
        {
            get { return Const.Attribute("fleetRoleSquadCmdr"); }
        }

        public PyObject FleetRoleMember
        {
            get { return Const.Attribute("fleetRoleMember"); }
        }

        public PyObject AU
        {
            get { return Const.Attribute("AU"); }
        }

        public PyObject GroupWreck
        {
            get { return Const.Attribute("groupWreck"); }
        }

        public PyObject MapWormholeSystemMin
        {
            get { return Const.Attribute("mapWormholeSystemMin"); }
        }
        public PyObject MapWormholeSystemMax
        {
            get { return Const.Attribute("mapWormholeSystemMax"); }
        }

        public PyObject AttributeEntityKillBounty
        {
            get { return Const.Attribute("attributeEntityKillBounty"); }
        }
    }
}