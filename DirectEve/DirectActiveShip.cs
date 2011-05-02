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

    public class DirectActiveShip : DirectItem
    {
        /// <summary>
        ///   Entity cache
        /// </summary>
        private DirectEntity _entity;

        internal DirectActiveShip(DirectEve directEve)
            : base(directEve)
        {
            PyItem = directEve.GetLocalSvc("godma").Attribute("stateManager").Attribute("invitems").DictionaryItem(directEve.Session.ShipId ?? -1);
        }

        /// <summary>
        ///   Maximum locked targets
        /// </summary>
        /// <remarks>
        ///   Skills may cause you to lock less targets!
        /// </remarks>
        public int MaxLockedTargets
        {
            get { return (int) Attributes.TryGet<double>("maxLockedTargets"); }
        }

        /// <summary>
        ///   The maximum target range
        /// </summary>
        public double MaxTargetRange
        {
            get { return Attributes.TryGet<double>("maxTargetRange"); }
        }

        /// <summary>
        ///   Your current amount of shields
        /// </summary>
        public double Shield
        {
            get { return Attributes.TryGet<double>("shieldCharge"); }
        }

        /// <summary>
        ///   The maxmimum amount of shields
        /// </summary>
        public double MaxShield
        {
            get { return Attributes.TryGet<double>("shieldCapacity"); }
        }

        /// <summary>
        ///   Shield percentage
        /// </summary>
        public double ShieldPercentage
        {
            get { return (Shield/MaxShield)*100; }
        }

        /// <summary>
        ///   Your current amount of armor
        /// </summary>
        public double Armor
        {
            get { return MaxArmor - Attributes.TryGet<double>("armorDamage"); }
        }

        /// <summary>
        ///   The maximum amount of armor
        /// </summary>
        public double MaxArmor
        {
            get { return Attributes.TryGet<double>("armorHP"); }
        }

        /// <summary>
        ///   Armor percentage
        /// </summary>
        public double ArmorPercentage
        {
            get { return (Armor/MaxArmor)*100; }
        }

        /// <summary>
        ///   Your current amount of structure
        /// </summary>
        public double Structure
        {
            get { return MaxStructure - Attributes.TryGet<double>("damage"); }
        }

        /// <summary>
        ///   The maximum amount of structure
        /// </summary>
        public double MaxStructure
        {
            get { return Attributes.TryGet<double>("hp"); }
        }

        /// <summary>
        ///   Structure percentage
        /// </summary>
        public double StructurePercentage
        {
            get { return (Structure/MaxStructure)*100; }
        }

        /// <summary>
        ///   Your current amount of capacitor
        /// </summary>
        public double Capacitor
        {
            get { return Attributes.TryGet<double>("charge"); }
        }

        /// <summary>
        ///   The maximum amount of capacitor
        /// </summary>
        public double MaxCapacitor
        {
            get { return Attributes.TryGet<double>("capacitorCapacity"); }
        }

        /// <summary>
        ///   Capacitor percentage
        /// </summary>
        public double CapacitorPercentage
        {
            get { return (Capacitor/MaxCapacitor)*100; }
        }

        /// <summary>
        ///   Maximum velocity
        /// </summary>
        public double MaxVelocity
        {
            get { return Attributes.TryGet<double>("maxVelocity"); }
        }

        /// <summary>
        ///   The entity associated with your ship
        /// </summary>
        /// <remarks>
        ///   Only works in space, return's null if no entity can be found
        /// </remarks>
        public DirectEntity Entity
        {
            get { return _entity ?? (_entity = DirectEve.GetEntityById(DirectEve.Session.ShipId ?? -1)); }
        }

        /// <summary>
        ///   Launch all drones
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        ///   Only works in space
        /// </remarks>
        public bool LaunchAllDrones()
        {
            var droneBay = DirectEve.GetShipsDroneBay();
            if (!droneBay.IsReady && !DirectEve.Windows.Any(w => w.Type == "form.DroneView"))
                return false;

            return LaunchDrones(droneBay.Items);
        }

        /// <summary>
        ///   Launch a specific list of drones
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        ///   Only works in space
        /// </remarks>
        public bool LaunchDrones(IEnumerable<DirectItem> drones)
        {
            var invItems = drones.Where(d => d.PyItem.IsValid).Select(d => d.PyItem);
            return DirectEve.ThreadedLocalSvcCall("menu", "LaunchDrones", invItems);
        }
    }
}