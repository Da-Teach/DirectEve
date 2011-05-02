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

    public class DirectModule : DirectItem
    {
        private List<DirectItem> _matchingAmmo;
        private PyObject _pyModule;

        internal DirectModule(DirectEve directEve, PyObject pyModule) : base(directEve)
        {
            _pyModule = pyModule;
        }

        public bool IsOnline { get; internal set; }
        public bool IsGoingOnline { get; internal set; }

        public bool IsActivatable { get; internal set; }
        public bool IsActive { get; internal set; }
        public bool IsDeactivating { get; internal set; }

        public bool IsReloadingAmmo { get; internal set; }
        public bool IsChangingAmmo { get; internal set; }

        public DirectItem Charge { get; internal set; }

        public int CurrentCharges
        {
            get { return Charge != null ? Charge.Stacksize : 0; }
        }

        public int MaxCharges
        {
            get
            {
                if (Capacity == 0)
                    return 0;

                if (Charge != null && Charge.Volume > 0)
                    return (int) (Capacity/Charge.Volume);

                if (MatchingAmmo.Count > 0)
                    return (int) (Capacity/MatchingAmmo[0].Volume);

                return 0;
            }
        }

        public long? TargetId { get; internal set; }

        public double? OptimalRange
        {
            get { return Attributes.TryGet<double>("maxRange"); }
        }

        public List<DirectItem> MatchingAmmo
        {
            get
            {
                if (_matchingAmmo == null)
                {
                    _matchingAmmo = new List<DirectItem>();

                    var pyCharges = _pyModule.Call("GetMatchingAmmo", TypeId).ToList();
                    foreach (var pyCharge in pyCharges)
                    {
                        var charge = new DirectItem(DirectEve);
                        charge.PyItem = pyCharge;
                        _matchingAmmo.Add(charge);
                    }
                }

                return _matchingAmmo;
            }
        }

        internal static List<DirectModule> GetModules(DirectEve directEve)
        {
            var modules = new List<DirectModule>();

            var pySharp = directEve.PySharp;
            var builtin = pySharp.Import("__builtin__");

            var pyModules = builtin.Attribute("uicore").Attribute("layer").Attribute("shipui").Attribute("sr").Attribute("modules").ToDictionary<long>();
            foreach (var pyModule in pyModules)
            {
                var module = new DirectModule(directEve, pyModule.Value);
                module.PyItem = pyModule.Value.Attribute("sr").Attribute("moduleInfo");
                module.ItemId = pyModule.Key;
                module.IsOnline = (bool) pyModule.Value.Attribute("online");
                module.IsGoingOnline = (bool) pyModule.Value.Attribute("goingOnline");
                module.IsReloadingAmmo = (bool) pyModule.Value.Attribute("reloadingAmmo");
                module.IsChangingAmmo = (bool) pyModule.Value.Attribute("changingAmmo");

                var effect = pyModule.Value.Attribute("def_effect");
                module.IsActivatable = effect.IsValid;
                module.IsActive = (bool) effect.Attribute("isActive");
                module.IsDeactivating = (bool) effect.Attribute("isDeactivating");
                module.TargetId = (long?) effect.Attribute("targetID");

                var pyCharge = pyModule.Value.Attribute("charge");
                if (pyCharge.IsValid)
                {
                    module.Charge = new DirectItem(directEve);
                    module.Charge.PyItem = pyCharge;
                }

                modules.Add(module);
            }

            return modules;
        }

        public bool Click()
        {
            return DirectEve.ThreadedCall(_pyModule.Attribute("Click"));
        }

        public bool ChangeAmmo(DirectItem charge)
        {
            if (charge.ItemId <= 0)
                return false;

            if (charge.TypeId <= 0)
                return false;

            if (charge.Stacksize <= 0)
                return false;

            return DirectEve.ThreadedCall(_pyModule.Attribute("ChangeAmmoType"), charge.TypeId, charge.Stacksize);
        }

        public bool ReloadAmmo(DirectItem charge)
        {
            if (charge.ItemId <= 0)
                return false;

            return DirectEve.ThreadedCall(_pyModule.Attribute("ReloadAmmo"), charge.ItemId, 1, charge.IsSingleton);
        }

        public bool Activate()
        {
            return DirectEve.ThreadedCall(_pyModule.Attribute("ActivateEffect"), _pyModule.Attribute("def_effect"));
        }

        public bool Activate(long targetId)
        {
            return DirectEve.ThreadedCall(_pyModule.Attribute("ActivateEffect"), _pyModule.Attribute("def_effect"), targetId);
        }

        public bool Deactivate()
        {
            return DirectEve.ThreadedCall(_pyModule.Attribute("DeactivateEffect"), _pyModule.Attribute("def_effect"));
        }
    }
}