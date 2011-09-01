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

    public class DirectItem : DirectObject
    {
        private DirectItemAttributes _attributes;
        private double? _basePrice;
        private double? _capacity;
        private int? _categoryId;
        private double? _chanceOfDuplicating;
        private int? _dataId;
        private string _description;
        private int? _flagId;
        private string _givenName;
        private int? _graphicId;
        private int? _groupId;
        private int? _iconId;
        private bool? _isSingleton;
        private long? _itemId;
        private long? _locationId;
        private int? _marketGroupId;
        private double? _mass;
        private int? _ownerId;
        private int? _portionSize;
        private bool? _published;
        private PyObject _pyInvType;
        private int? _quantity;
        private int? _raceId;
        private double? _radius;
        private int? _soundId;
        private int? _stacksize;
        private int? _typeId;
        private string _typeName;
        private double? _volume;
        private List<DirectItem> _materials;

        public DirectItem(DirectEve directEve) : base(directEve)
        {
            PyItem = global::DirectEve.PySharp.PySharp.PyZero;
        }

        internal PyObject PyItem { get; set; }

        internal PyObject PyInvType
        {
            get
            {
                if (_pyInvType == null)
                    _pyInvType = PySharp.Import("__builtin__").Attribute("cfg").Attribute("invtypes").Call("GetIfExists", TypeId);

                return _pyInvType;
            }
        }

        public List<DirectItem> Materials
        {
            get
            {
                if (_materials == null)
                {
                    _materials = new List<DirectItem>();
                    foreach (var pyMaterial in PySharp.Import("__builtin__").Attribute("cfg").Attribute("invtypematerials").DictionaryItem(TypeId).ToList())
                    {
                        var material = new DirectItem(DirectEve);
                        material.ItemId = -1;
                        material.Stacksize = -1;
                        material.OwnerId = -1;
                        material.LocationId = -1;
                        material.FlagId = 0;
                        material.IsSingleton = false;
                        material.TypeId = (int) pyMaterial.Attribute("materialTypeID");
                        material.Quantity = (int) pyMaterial.Attribute("quantity");
                        _materials.Add(material);
                    }
                }

                return _materials;
            }
        }

        public long ItemId
        {
            get
            {
                if (!_itemId.HasValue)
                    _itemId = (long) PyItem.Attribute("itemID");

                return _itemId.Value;
            }
            internal set { _itemId = value; }
        }

        public int TypeId
        {
            get
            {
                if (!_typeId.HasValue)
                    _typeId = (int) PyItem.Attribute("typeID");

                return _typeId.Value;
            }
            internal set { _typeId = value; }
        }

        public int OwnerId
        {
            get
            {
                if (!_ownerId.HasValue)
                    _ownerId = (int) PyItem.Attribute("ownerID");

                return _ownerId.Value;
            }
            internal set { _ownerId = value; }
        }

        public long LocationId
        {
            get
            {
                if (!_locationId.HasValue)
                    _locationId = (long) PyItem.Attribute("locationID");

                return _locationId.Value;
            }
            internal set { _locationId = value; }
        }

        public int FlagId
        {
            get
            {
                if (!_flagId.HasValue)
                    _flagId = (int) PyItem.Attribute("flagID");

                return _flagId.Value;
            }
            internal set { _flagId = value; }
        }

        public int Quantity
        {
            get
            {
                if (!_quantity.HasValue)
                    _quantity = (int) PyItem.Attribute("quantity");

                return _quantity.Value;
            }
            internal set { _quantity = value; }
        }

        public int Stacksize
        {
            get
            {
                if (!_stacksize.HasValue)
                    _stacksize = (int) PyItem.Attribute("stacksize");

                return _stacksize.Value;
            }
            internal set { _stacksize = value; }
        }

        public bool IsSingleton
        {
            get
            {
                if (!_isSingleton.HasValue)
                    _isSingleton = (bool) PyItem.Attribute("singleton");

                return _isSingleton.Value;
            }
            internal set { _isSingleton = value; }
        }

        public int GroupId
        {
            get
            {
                if (!_groupId.HasValue)
                    _groupId = (int) PyInvType.Attribute("groupID");

                return _groupId.Value;
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

        public string GivenName
        {
            get
            {
                if (_givenName == null)
                    _givenName = DirectEve.GetLocationName(ItemId);

                return _givenName;
            }
        }

        public DirectItemAttributes Attributes
        {
            get
            {
                if (_attributes == null && PyItem.IsValid)
                {
                    var pyItemId = PyItem.Attribute("itemID");
                    if (pyItemId.IsValid)
                        _attributes = new DirectItemAttributes(DirectEve, pyItemId);
                }

                _attributes = _attributes ?? new DirectItemAttributes(DirectEve, ItemId);
                return _attributes;
            }
        }

        internal static bool RefreshItems(DirectEve directEve, PyObject inventory, PyObject flag)
        {
            var list = inventory.Attribute("List");
            return flag.IsValid ? directEve.ThreadedCall(list, flag) : directEve.ThreadedCall(list);
        }

        internal static List<DirectItem> GetItems(DirectEve directEve, PyObject inventory, PyObject flag)
        {
            var list = inventory.Attribute("List");
            var items = new List<DirectItem>();

            var pyItems = (flag.IsValid ? list.CallThis() : list.CallThis(flag)).ToList();
            foreach (var pyItem in pyItems)
            {
                var item = new DirectItem(directEve);
                item.PyItem = pyItem;

                // Do not add the item if the flags do not coincide
                if (flag.IsValid && (int) flag != item.FlagId)
                    continue;

                items.Add(item);
            }

            return items;
        }

        /// <summary>
        ///   Drop items into People and Places
        /// </summary>
        /// <param name = "directEve"></param>
        /// <param name = "bookmarks"></param>
        /// <returns></returns>
        internal static bool DropInPlaces(DirectEve directEve, IEnumerable<DirectItem> bookmarks)
        {
            var data = new List<PyObject>();
            foreach (var bookmark in bookmarks)
                data.Add(directEve.PySharp.Import("uix").Call("GetItemData", bookmark.PyItem, "list"));

            return directEve.ThreadedLocalSvcCall("addressbook", "DropInPlaces", data);
        }

        /// <summary>
        ///   Open up the quick-sell window to sell this item
        /// </summary>
        /// <returns></returns>
        public bool QuickSell()
        {
            return DirectEve.ThreadedLocalSvcCall("marketutils", "Sell", TypeId, PyItem);
        }

        /// <summary>
        ///   Open up the quick-buy window to buy more of this item
        /// </summary>
        /// <returns></returns>
        public bool QuickBuy()
        {
            return DirectEve.ThreadedLocalSvcCall("marketutils", "Buy", TypeId, PyItem);
        }

        /// <summary>
        ///   Activate this ship
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        ///   Fails if the current location is not the same as the current station and if its not a CategoryShip
        /// </remarks>
        public bool ActivateShip()
        {
            if (LocationId != DirectEve.Session.StationId)
                return false;

            if (CategoryId != (int) DirectEve.Const.CategoryShip)
                return false;

            return DirectEve.ThreadedLocalSvcCall("station", "TryActivateShip", PyItem);
        }

        /// <summary>
        ///   Leave this ship
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        ///   Fails if the current location is not the same as the current station and if its not a CategoryShip
        /// </remarks>
        public bool LeaveShip()
        {
            if (ItemId != DirectEve.Session.ShipId)
                return false;

            if (LocationId != DirectEve.Session.StationId)
                return false;

            if (CategoryId != (int) DirectEve.Const.CategoryShip)
                return false;

            return DirectEve.ThreadedLocalSvcCall("station", "TryLeaveShip", PyItem);
        }
        
        /// <summary>
        /// Fit this item to your ship
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        ///   Fails if the selected item is not of CategoryModule
        /// </remarks>
        public bool FitToActiveShip()
        {
            if (CategoryId != (int)DirectEve.Const.CategoryModule)
                return false;

            var data = new List<PyObject>();
            data.Add(PyItem);

            return DirectEve.ThreadedLocalSvcCall("menu", "TryFit", data);
        }
    }
}