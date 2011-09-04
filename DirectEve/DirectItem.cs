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

    public class DirectItem : DirectInvType
    {
        private DirectItemAttributes _attributes;

        private int? _flagId;
        private string _givenName;
        private bool? _isSingleton;
        private long? _itemId;
        private long? _locationId;
        private List<DirectItem> _materials;
        private int? _ownerId;
        private PyObject _pyItem;
        private int? _quantity;
        private int? _stacksize;

        public DirectItem(DirectEve directEve) : base(directEve)
        {
            PyItem = global::DirectEve.PySharp.PySharp.PyZero;
        }

        internal PyObject PyItem
        {
            get { return _pyItem; }
            set
            {
                _pyItem = value;

                if (_pyItem != null && _pyItem.IsValid)
                    TypeId = (int) _pyItem.Attribute("typeID");
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
        ///   Fit this item to your ship
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        ///   Fails if the selected item is not of CategoryModule
        /// </remarks>
        public bool FitToActiveShip()
        {
            if (CategoryId != (int) DirectEve.Const.CategoryModule)
                return false;

            var data = new List<PyObject>();
            data.Add(PyItem);

            return DirectEve.ThreadedLocalSvcCall("menu", "TryFit", data);
        }
    }
}