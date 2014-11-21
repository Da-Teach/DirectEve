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

    public class DirectInvType : DirectObject
    {
        private double? _basePrice;
        private double? _capacity;
        private int? _categoryId;
        private string _categoryName;
        private double? _chanceOfDuplicating;
        private int? _dataId;
        private string _description;
        private int? _graphicId;
        private int? _groupId;
        private string _groupName;
        private int? _iconId;
        private int? _marketGroupId;
        private double? _mass;
        private int? _portionSize;
        private bool? _published;
        private PyObject _pyInvCategory;
        private PyObject _pyInvGroup;
        private PyObject _pyInvType;
        private int? _raceId;
        private double? _radius;
        private int? _soundId;
        private string _typeName;
        private double? _volume;

        private double? _shield;
        private double? _armor;
        private double? _structure;

        private double? _shieldResistanceEM;
        private double? _shieldResistanceKinetic;
        private double? _shieldResistanceExplosion;
        private double? _shieldResistanceThermal;

        private double? _armorResistanceEM;
        private double? _armorResistanceKinetic;
        private double? _armorResistanceExplosion;
        private double? _armorResistanceThermal;

        private double? _signatureRadius;

        internal DirectInvType(DirectEve directEve)
            : base(directEve)
        {
        }

        internal DirectInvType(DirectEve directEve, int typeId)
            : base(directEve)
        {
            TypeId = typeId;
        }

        internal PyObject PyInvType
        {
            get { return _pyInvType ?? (_pyInvType = PySharp.Import("__builtin__").Attribute("cfg").Attribute("invtypes").Call("GetIfExists", TypeId)); }
        }

        internal PyObject PyInvGroup
        {
            get { return _pyInvGroup ?? (_pyInvGroup = PySharp.Import("__builtin__").Attribute("cfg").Attribute("invgroups").Call("GetIfExists", GroupId)); }
        }

        internal PyObject PyInvCategory
        {
            get { return _pyInvCategory ?? (_pyInvCategory = PySharp.Import("__builtin__").Attribute("cfg").Attribute("invcategories").Call("GetIfExists", CategoryId)); }
        }

        public int TypeId { get; internal set; }

        public int GroupId
        {
            get
            {
                if (!_groupId.HasValue)
                    _groupId = (int) PyInvType.Attribute("groupID");

                return _groupId.Value;
            }
        }

        public string GroupName
        {
            get
            {
                if (string.IsNullOrEmpty(_groupName))
                    _groupName = (string) PyInvGroup.Attribute("groupName");

                return _groupName;
            }
        }

        public string CategoryName
        {
            get
            {
                if (string.IsNullOrEmpty(_categoryName))
                    _categoryName = (string) PyInvCategory.Attribute("categoryName");

                return _categoryName;
            }
        }

        public string TypeName
        {
            get
            {
                if (string.IsNullOrEmpty(_typeName))
                    _typeName = (string) PyInvType.Attribute("typeName");

                return _typeName;
            }
        }

        public string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                    _description = (string) PyInvType.Attribute("description");

                return _description;
            }
        }

        public int GraphicId
        {
            get
            {
                if (!_graphicId.HasValue)
                    _graphicId = (int) PyInvType.Attribute("graphicID");

                return _graphicId.Value;
            }
        }

        public double Radius
        {
            get
            {
                if (!_radius.HasValue)
                    _radius = (double) PyInvType.Attribute("radius");

                return _radius.Value;
            }
        }

        public double Mass
        {
            get
            {
                if (!_mass.HasValue)
                    _mass = (double) PyInvType.Attribute("mass");

                return _mass.Value;
            }
        }

        public double Volume
        {
            get
            {
                if (!_volume.HasValue)
                    _volume = (double) PyInvType.Attribute("volume");

                return _volume.Value;
            }
        }

        public double Capacity
        {
            get
            {
                if (!_capacity.HasValue)
                    _capacity = (double) PyInvType.Attribute("capacity");

                return _capacity.Value;
            }
        }

        public int PortionSize
        {
            get
            {
                if (!_portionSize.HasValue)
                    _portionSize = (int) PyInvType.Attribute("portionSize");

                return _portionSize.Value;
            }
        }

        public int RaceId
        {
            get
            {
                if (!_raceId.HasValue)
                    _raceId = (int) PyInvType.Attribute("raceID");

                return _raceId.Value;
            }
        }

        public double BasePrice
        {
            get
            {
                if (!_basePrice.HasValue)
                    _basePrice = (double) PyInvType.Attribute("basePrice");

                return _basePrice.Value;
            }
        }

        public bool Published
        {
            get
            {
                if (!_published.HasValue)
                    _published = (bool) PyInvType.Attribute("published");

                return _published.Value;
            }
        }

        public int MarketGroupId
        {
            get
            {
                if (!_marketGroupId.HasValue)
                    _marketGroupId = (int) PyInvType.Attribute("marketGroupID");

                return _marketGroupId.Value;
            }
        }

        public double ChanceOfDuplicating
        {
            get
            {
                if (!_chanceOfDuplicating.HasValue)
                    _chanceOfDuplicating = (double) PyInvType.Attribute("chanceOfDuplicating");

                return _chanceOfDuplicating.Value;
            }
        }

        public int SoundId
        {
            get
            {
                if (!_soundId.HasValue)
                    _soundId = (int) PyInvType.Attribute("soundID");

                return _soundId.Value;
            }
        }

        public int CategoryId
        {
            get
            {
                if (!_categoryId.HasValue)
                    _categoryId = (int) PyInvType.Attribute("categoryID");

                return _categoryId.Value;
            }
        }

        public int IconId
        {
            get
            {
                if (!_iconId.HasValue)
                    _iconId = (int) PyInvType.Attribute("iconID");

                return _iconId.Value;
            }
        }

        public int DataId
        {
            get
            {
                if (!_dataId.HasValue)
                    _dataId = (int) PyInvType.Attribute("dataID");

                return _dataId.Value;
            }
        }

        public double? Shield
        {
            get
            {
                if (!_shield.HasValue)
                {
                    var dmgAttributes = DirectEve.PySharp.Import("__builtin__").Attribute("cfg").Attribute("dgmtypeattribs").Call("get", TypeId);
                    if (dmgAttributes.IsValid)
                    {
                        foreach (var row in dmgAttributes.ToList())
                        {
                            if ((int) row.Attribute("attributeID") == (int) DirectEve.Const.AttributeShieldCapacity)
                            {
                                _shield = (double) row.Attribute("value");
                                break;
                            }
                        }
                    }
                }
                return _shield;
            }
        }

        public double? Armor
        {
            get
            {
                if (!_armor.HasValue)
                {
                    var dmgAttributes = DirectEve.PySharp.Import("__builtin__").Attribute("cfg").Attribute("dgmtypeattribs").Call("get", TypeId);
                    if (dmgAttributes.IsValid)
                    {
                        foreach (var row in dmgAttributes.ToList())
                        {
                            if ((int) row.Attribute("attributeID") == (int) DirectEve.Const.AttributeArmorHP)
                            {
                                _armor = (double) row.Attribute("value");
                                break;
                            }
                        }
                    }
                }
                return _armor;
            }
        }

        public double? Structure
        {
            get
            {
                if (!_structure.HasValue)
                {
                    var dmgAttributes = DirectEve.PySharp.Import("__builtin__").Attribute("cfg").Attribute("dgmtypeattribs").Call("get", TypeId);
                    if (dmgAttributes.IsValid)
                    {
                        foreach (var row in dmgAttributes.ToList())
                        {
                            if ((int) row.Attribute("attributeID") == (int) DirectEve.Const.AttributeHullHP)
                            {
                                _structure = (double) row.Attribute("value");
                                break;
                            }
                        }
                    }
                }
                return _structure;
            }
        }

        public double? ShieldResistanceEM
        {
            get
            {
                if (!_shieldResistanceEM.HasValue)
                {
                    var dmgAttributes = DirectEve.PySharp.Import("__builtin__").Attribute("cfg").Attribute("dgmtypeattribs").Call("get", TypeId);
                    if (dmgAttributes.IsValid)
                    {
                        foreach (var row in dmgAttributes.ToList())
                        {
                            if ((int) row.Attribute("attributeID") == (int) DirectEve.Const.AttributeShieldEmDamageResonance)
                            {
                                _shieldResistanceEM = (double) row.Attribute("value");
                                break;
                            }
                        }
                    }
                }
                return _shieldResistanceEM;
            }
        }

        public double? ShieldResistanceKinetic
        {
            get
            {
                if (!_shieldResistanceKinetic.HasValue)
                {
                    var dmgAttributes = DirectEve.PySharp.Import("__builtin__").Attribute("cfg").Attribute("dgmtypeattribs").Call("get", TypeId);
                    if (dmgAttributes.IsValid)
                    {
                        foreach (var row in dmgAttributes.ToList())
                        {
                            if ((int) row.Attribute("attributeID") == (int) DirectEve.Const.AttributeShieldKineticDamageResonance)
                            {
                                _shieldResistanceKinetic = (double) row.Attribute("value");
                                break;
                            }
                        }
                    }
                }
                return _shieldResistanceKinetic;
            }
        }

        public double? ShieldResistanceExplosion
        {
            get
            {
                if (!_shieldResistanceExplosion.HasValue)
                {
                    var dmgAttributes = DirectEve.PySharp.Import("__builtin__").Attribute("cfg").Attribute("dgmtypeattribs").Call("get", TypeId);
                    if (dmgAttributes.IsValid)
                    {
                        foreach (var row in dmgAttributes.ToList())
                        {
                            if ((int) row.Attribute("attributeID") == (int) DirectEve.Const.AttributeShieldExplosiveDamageResonance)
                            {
                                _shieldResistanceExplosion = (double) row.Attribute("value");
                                break;
                            }
                        }
                    }
                }
                return _shieldResistanceExplosion;
            }
        }

        public double? ShieldResistanceThermal
        {
            get
            {
                if (!_shieldResistanceThermal.HasValue)
                {
                    var dmgAttributes = DirectEve.PySharp.Import("__builtin__").Attribute("cfg").Attribute("dgmtypeattribs").Call("get", TypeId);
                    if (dmgAttributes.IsValid)
                    {
                        foreach (var row in dmgAttributes.ToList())
                        {
                            if ((int) row.Attribute("attributeID") == (int) DirectEve.Const.AttributeShieldThermalDamageResonance)
                            {
                                _shieldResistanceThermal = (double) row.Attribute("value");
                                break;
                            }
                        }
                    }
                }
                return _shieldResistanceThermal;
            }
        }

        public double? ArmorResistanceEM
        {
            get
            {
                if (!_armorResistanceEM.HasValue)
                {
                    var dmgAttributes = DirectEve.PySharp.Import("__builtin__").Attribute("cfg").Attribute("dgmtypeattribs").Call("get", TypeId);
                    if (dmgAttributes.IsValid)
                    {
                        foreach (var row in dmgAttributes.ToList())
                        {
                            if ((int) row.Attribute("attributeID") == (int) DirectEve.Const.AttributeArmorEmDamageResonance)
                            {
                                _armorResistanceEM = (double) row.Attribute("value");
                                break;
                            }
                        }
                    }
                }
                return _armorResistanceEM;
            }
        }

        public double? ArmorResistanceKinetic
        {
            get
            {
                if (!_armorResistanceKinetic.HasValue)
                {
                    var dmgAttributes = DirectEve.PySharp.Import("__builtin__").Attribute("cfg").Attribute("dgmtypeattribs").Call("get", TypeId);
                    if (dmgAttributes.IsValid)
                    {
                        foreach (var row in dmgAttributes.ToList())
                        {
                            if ((int) row.Attribute("attributeID") == (int) DirectEve.Const.AttributeArmorKineticDamageResonance)
                            {
                                _armorResistanceKinetic = (double) row.Attribute("value");
                                break;
                            }
                        }
                    }
                }
                return _armorResistanceKinetic;
            }
        }

        public double? ArmorResistanceExplosion
        {
            get
            {
                if (!_armorResistanceExplosion.HasValue)
                {
                    var dmgAttributes = DirectEve.PySharp.Import("__builtin__").Attribute("cfg").Attribute("dgmtypeattribs").Call("get", TypeId);
                    if (dmgAttributes.IsValid)
                    {
                        foreach (var row in dmgAttributes.ToList())
                        {
                            if ((int) row.Attribute("attributeID") == (int) DirectEve.Const.AttributeArmorExplosiveDamageResonance)
                            {
                                _armorResistanceExplosion = (double) row.Attribute("value");
                                break;
                            }
                        }
                    }
                }
                return _armorResistanceExplosion;
            }
        }

        public double? ArmorResistanceThermal
        {
            get
            {
                if (!_armorResistanceThermal.HasValue)
                {
                    var dmgAttributes = DirectEve.PySharp.Import("__builtin__").Attribute("cfg").Attribute("dgmtypeattribs").Call("get", TypeId);
                    if (dmgAttributes.IsValid)
                    {
                        foreach (var row in dmgAttributes.ToList())
                        {
                            if ((int) row.Attribute("attributeID") == (int) DirectEve.Const.AttributeArmorThermalDamageResonance)
                            {
                                _armorResistanceThermal = (double) row.Attribute("value");
                                break;
                            }
                        }
                    }
                }
                return _armorResistanceThermal;
            }
        }

        public double? SignatureRadius
        {
            get
            {
                if (!_signatureRadius.HasValue)
                {
                    var dmgAttributes = DirectEve.PySharp.Import("__builtin__").Attribute("cfg").Attribute("dgmtypeattribs").Call("get", TypeId);
                    if (dmgAttributes.IsValid)
                    {
                        foreach (var row in dmgAttributes.ToList())
                        {
                            if ((int) row.Attribute("attributeID") == (int) DirectEve.Const.AttributeSignatureRadius)
                            {
                                _signatureRadius = (double) row.Attribute("value");
                                break;
                            }
                        }
                    }
                }
                return _signatureRadius;
            }
        }

        internal static Dictionary<int, DirectInvType> GetInvtypes(DirectEve directEve)
        {
            var result = new Dictionary<int, DirectInvType>();

            var pyDict = directEve.PySharp.Import("__builtin__").Attribute("cfg").Attribute("invtypes").Attribute("data").ToDictionary<int>();
            foreach (var pair in pyDict)
                result[pair.Key] = new DirectInvType(directEve, pair.Key);

            return result;
        }
    }
}