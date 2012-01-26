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
    using global::DirectEve.PySharp;

    public class DirectEntity : DirectInvType
    {
        private int? _allianceId;
        private double? _armorPct;
        private PyObject _ball;
        private PyObject _ballpark;
        private int? _charId;
        private int? _corpId;
        private double? _distance;
        private long? _followId;
        private bool? _hasExploded;
        private bool? _hasReleased;
        private bool? _isCloaked;
        private bool? _isEmpty;
        private int? _mode;
        private string _name;
        private string _givenName;
        private int? _ownerId;
        private double? _shieldPct;
        private PyObject _slimItem;
        private double? _structurePct;
        private double? _velocity;
        private double? _x;
        private double? _y;
        private double? _z;

        internal DirectEntity(DirectEve directEve, PyObject ballpark, PyObject ball, PyObject slimItem, long id)
            : base(directEve)
        {
            _ballpark = ballpark;
            _ball = ball;
            _slimItem = slimItem;

            Id = id;
            TypeId = (int) slimItem.Attribute("typeID");

            Attacks = new List<string>();
            ElectronicWarfare = new List<string>();
        }

        public long Id { get; internal set; }

        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                    _name = (string) PySharp.Import("uix").Call("GetSlimItemName", _slimItem);

                return _name;
            }
        }

        public string GivenName
        {
            get
            {
                if (_givenName == null)
                    _givenName = DirectEve.GetLocationName(Id);

                return _givenName;
            }
        }

        public int OwnerId
        {
            get
            {
                if (!_ownerId.HasValue)
                    _ownerId = (int) _slimItem.Attribute("ownerID");

                return _ownerId.Value;
            }
        }

        public int CharId
        {
            get
            {
                if (!_charId.HasValue)
                    _charId = (int) _slimItem.Attribute("charID");

                return _charId.Value;
            }
        }

        public int CorpId
        {
            get
            {
                if (!_corpId.HasValue)
                    _corpId = (int) _slimItem.Attribute("corpID");

                return _corpId.Value;
            }
        }

        public int AllianceId
        {
            get
            {
                if (!_allianceId.HasValue)
                    _allianceId = (int) _ball.Attribute("allianceID");

                return _allianceId.Value;
            }
        }

        public long FollowId
        {
            get
            {
                if (!_followId.HasValue)
                    _followId = (long) _ball.Attribute("followId");

                return _followId.Value;
            }
        }

        public int Mode
        {
            get
            {
                if (!_mode.HasValue)
                    _mode = (int) _ball.Attribute("mode");

                return _mode.Value;
            }
        }

        public bool IsWarping
        {
            get { return Mode == 3; }
        }

        public bool IsNpc
        {
            get { return (bool?) PySharp.Import("util").Call("IsNPC", OwnerId) ?? false; }
        }

        public bool IsPc
        {
            get { return CharId > 0; }
        }

        public bool IsEmpty
        {
            get
            {
                if (!_isEmpty.HasValue)
                    _isEmpty = (bool?) _slimItem.Attribute("isEmpty") ?? true;

                return _isEmpty.Value;
            }
        }

        public bool HasExploded
        {
            get
            {
                if (!_hasExploded.HasValue)
                    _hasExploded = (bool) _ball.Attribute("exploded");

                return _hasExploded.Value;
            }
        }

        public bool IsCloaked
        {
            get
            {
                if (!_isCloaked.HasValue)
                    _isCloaked = (int) _ball.Attribute("isCloaked") != 0;

                return _isCloaked.Value;
            }
        }

        public bool HasReleased
        {
            get
            {
                if (!_hasReleased.HasValue)
                    _hasReleased = (bool) _ball.Attribute("released");

                return _hasReleased.Value;
            }
        }

        public double Distance
        {
            get
            {
                if (!_distance.HasValue)
                    _distance = (double) _ball.Attribute("surfaceDist");

                return _distance.Value;
            }
        }

        public double X
        {
            get
            {
                if (!_x.HasValue)
                    _x = (double) _ball.Attribute("x");

                return _x.Value;
            }
        }

        public double Y
        {
            get
            {
                if (!_y.HasValue)
                    _y = (double) _ball.Attribute("y");

                return _y.Value;
            }
        }

        public double Z
        {
            get
            {
                if (!_z.HasValue)
                    _z = (double) _ball.Attribute("z");

                return _z.Value;
            }
        }

        public double Velocity
        {
            get
            {
                if (_velocity == null)
                    _velocity = (double)_ball.Call("GetVectorDotAt", PySharp.Import("blue").Attribute("os").Call("GetSimTime")).Call("Length");

                return _velocity.Value;
            }
        }

        public bool IsTarget { get; internal set; }
        public bool IsActiveTarget { get; internal set; }
        public bool IsTargeting { get; internal set; }
        public bool IsTargetedBy { get; internal set; }
        public bool IsAttacking { get; internal set; }

        public List<string> Attacks { get; private set; }
        public List<string> ElectronicWarfare { get; private set; }

        public bool IsWarpScramblingMe { get; private set; }
        public bool IsWebbingMe { get; private set; }
        public bool IsNeutralizingMe { get; private set; }
        public bool IsJammingMe { get; private set; }
        public bool IsSensorDampeningMe { get; private set; }
        public bool IsTargetPaintingMe { get; private set; }
        public bool IsTrackingDisruptingMe { get; private set; }

        public double ShieldPct
        {
            get
            {
                if (!_shieldPct.HasValue)
                    GetDamageState();

                return _shieldPct ?? 0;
            }
        }

        public double ArmorPct
        {
            get
            {
                if (!_armorPct.HasValue)
                    GetDamageState();

                return _armorPct ?? 0;
            }
        }

        public double StructurePct
        {
            get
            {
                if (!_structurePct.HasValue)
                    GetDamageState();

                return _structurePct ?? 0;
            }
        }

        /// <summary>
        ///   Is it a valid entity?
        /// </summary>
        public bool IsValid
        {
            get { return _ball.IsValid && _slimItem.IsValid && Id > 0 && !string.IsNullOrEmpty(Name) && Name != "entity" && !HasExploded && !HasReleased; }
        }

        internal void GetDamageState()
        {
            _shieldPct = 0;
            _armorPct = 0;
            _structurePct = 0;

            // Get damage state properties
            var damageState = _ballpark.Call("GetDamageState", Id).ToList();
            if (damageState.Count == 3)
            {
                _shieldPct = (double) damageState[0];
                _armorPct = (double) damageState[1];
                _structurePct = (double) damageState[2];
            }
        }

        internal static Dictionary<long, DirectEntity> GetEntities(DirectEve directEve)
        {
            var pySharp = directEve.PySharp;
            var entitiesById = new Dictionary<long, DirectEntity>();

            // Used by various loops, etc
            DirectEntity entity;

            var uix = pySharp.Import("uix");
            var ballpark = directEve.GetLocalSvc("michelle").Call("GetBallpark");
            var balls = ballpark.Attribute("balls").Call("keys").ToList<long>();
            foreach (var id in balls)
            {
                if (id < 0)
                    continue;

                // Get slim item
                var slimItem = ballpark.Call("GetInvItem", id);

                // Get ball
                var ball = ballpark.Call("GetBall", id);

                // Create the entity
                entitiesById[id] = new DirectEntity(directEve, ballpark, ball, slimItem, id);
                ;
            }

            // Mark active target
            var activeTarget = pySharp.Import("state").Attribute("activeTarget");
            var activeTargetId = (long) directEve.GetLocalSvc("state").Call("GetExclState", activeTarget);
            if (entitiesById.TryGetValue(activeTargetId, out entity))
                entity.IsActiveTarget = true;

            var target = directEve.GetLocalSvc("target");
            var targets = target.Attribute("targets").ToList<long>();
            foreach (var targetId in targets)
            {
                if (!entitiesById.TryGetValue(targetId, out entity))
                    continue;

                entity.IsTarget = true;
            }

            var targeting = target.Attribute("targeting").ToDictionary<long>().Keys;
            foreach (var targetId in targeting)
            {
                if (!entitiesById.TryGetValue(targetId, out entity))
                    continue;

                entity.IsTargeting = true;
            }

            var targetedBy = target.Attribute("targetedBy").ToList<long>();
            foreach (var targetId in targetedBy)
            {
                if (!entitiesById.TryGetValue(targetId, out entity))
                    continue;

                entity.IsTargetedBy = true;
            }

            var attackers = directEve.GetLocalSvc("tactical").Attribute("attackers").ToDictionary<long>();
            foreach (var attacker in attackers)
            {
                if (!entitiesById.TryGetValue(attacker.Key, out entity))
                    continue;

                entity.IsAttacking = true;

                var attacks = attacker.Value.ToList();
                foreach (var attack in attacks.Select(a => (string) a.Item(1)))
                {
                    entity.IsWarpScramblingMe |= attack == "effects.WarpScramble";
                    entity.IsWebbingMe |= attack == "effects.ModifyTargetSpeed";
                    entity.Attacks.Add(attack);
                }
            }

            var jammers = directEve.GetLocalSvc("tactical").Attribute("jammers").ToDictionary<long>();
            foreach (var jammer in jammers)
            {
                if (!entitiesById.TryGetValue(jammer.Key, out entity))
                    continue;

                var ews = jammer.Value.ToDictionary<string>().Keys;
                foreach (var ew in ews)
                {
                    entity.IsNeutralizingMe |= ew == "ewEnergyNeut";
                    entity.IsJammingMe |= ew == "electronic";
                    entity.IsSensorDampeningMe |= ew == "ewRemoteSensorDamp";
                    entity.IsTargetPaintingMe |= ew == "ewTargetPaint";
                    entity.IsTrackingDisruptingMe |= ew == "ewTrackingDisrupt";
                    entity.ElectronicWarfare.Add(ew);
                }
            }

            return entitiesById;
        }

        /// <summary>
        ///   Lock target
        /// </summary>
        /// <returns></returns>
        public bool LockTarget()
        {
            // It's already a target!
            if (IsTarget || IsTargeting)
                return false;

            // We can't target this entity yet!
            if (!DirectEve.CanTarget(Id))
                return false;

            // Set target timer
            DirectEve.SetTargetTimer(Id);

            return DirectEve.ThreadedLocalSvcCall("menu", "LockTarget", Id);
        }

        /// <summary>
        ///   Unlock target
        /// </summary>
        /// <returns></returns>
        public bool UnlockTarget()
        {
            // Its not a target, why are you unlocking?!?!
            if (!IsTarget)
                return false;

            // Clear target information
            DirectEve.ClearTargetTimer(Id);

            return DirectEve.ThreadedLocalSvcCall("menu", "UnlockTarget", Id);
        }

        /// <summary>
        ///   Approach target at 50m
        /// </summary>
        /// <returns></returns>
        public bool Approach()
        {
            return Approach(50);
        }

        /// <summary>
        ///   Approach target
        /// </summary>
        /// <param name = "range"></param>
        /// <returns></returns>
        public bool Approach(int range)
        {
            return DirectEve.ThreadedLocalSvcCall("menu", "Approach", Id, range);
        }

        /// <summary>
        ///   Orbit target at 5000m
        /// </summary>
        /// <returns></returns>
        public bool Orbit()
        {
            return Orbit(5000);
        }

        /// <summary>
        ///   Orbit target
        /// </summary>
        /// <param name = "range"></param>
        /// <returns></returns>
        public bool Orbit(int range)
        {
            return DirectEve.ThreadedLocalSvcCall("menu", "Orbit", Id, range);
        }

        /// <summary>
        ///   Warp to target
        /// </summary>
        /// <returns></returns>
        public bool WarpTo()
        {
            return DirectEve.ThreadedLocalSvcCall("menu", "WarpToItem", Id);
        }


        /// <summary>
        ///   Warp to target and dock
        /// </summary>
        /// <returns></returns>
        public bool WarpToAndDock()
        {
            return DirectEve.ThreadedLocalSvcCall("menu", "DockOrJumpOrActivateGate", Id);
        }

        /// <summary>
        ///   Warp to target and dock
        /// </summary>
        /// <returns></returns>
        public bool Dock()
        {
            return DirectEve.ThreadedLocalSvcCall("menu", "DockOrJumpOrActivateGate", Id);
        }

        

        /// <summary>
        ///   Warp to target
        /// </summary>
        /// <returns></returns>
        public bool AlignTo()
        {
            return DirectEve.ThreadedLocalSvcCall("menu", "AlignTo", Id);
        }

        /// <summary>
        ///   Open cargo window (only valid on containers in space, or own ship)
        /// </summary>
        /// <returns></returns>
        public bool OpenCargo()
        {
            return DirectEve.ThreadedLocalSvcCall("menu", "OpenCargo", Id);
        }

        /// <summary>
        ///   Jump (Stargates only)
        /// </summary>
        /// <returns></returns>
        public bool Jump()
        {
            var toCelestialId = _slimItem.Attribute("jumps").Item(0).Attribute("toCelestialID");
            var locationId = _slimItem.Attribute("jumps").Item(0).Attribute("locationID");
            return DirectEve.ThreadedLocalSvcCall("menu", "StargateJump", Id, toCelestialId, locationId);
        }

        /// <summary>
        ///   Activate (Acceleration Gates only)
        /// </summary>
        /// <returns></returns>
        public bool Activate()
        {
            return DirectEve.ThreadedLocalSvcCall("menu", "ActivateAccelerationGate", Id);
        }

        /// <summary>
        ///   Make this your active target
        /// </summary>
        /// <returns></returns>
        public bool MakeActiveTarget()
        {
            if (!IsTarget)
                return false;

            if (IsActiveTarget)
                return true;

            // Even though we uthread the thing, expect it to be instant
            var currentActiveTarget = DirectEve.Entities.FirstOrDefault(t => t.IsActiveTarget);
            if (currentActiveTarget != null)
                currentActiveTarget.IsActiveTarget = false;

            // Switch active targets
            var activeTarget = PySharp.Import("state").Attribute("activeTarget");
            return IsActiveTarget = DirectEve.ThreadedLocalSvcCall("state", "SetState", Id, activeTarget, 1);
        }
    }
}