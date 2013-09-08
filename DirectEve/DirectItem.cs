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

        internal DirectItem(DirectEve directEve) : base(directEve)
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

        public double AveragePrice()
        {
            if (!DirectEve.HasSupportInstances())
            {
                DirectEve.Log("DirectEve: Error: This method requires a support instance.");
                return -1;
            }
            
            return (double)PySharp.Import("util").Call("GetAveragePrice", PyItem);
        }

        internal static bool RefreshItems(DirectEve directEve, PyObject inventory, PyObject flag)
        {            
            return directEve.ThreadedCall(inventory.Attribute("InvalidateCache"));
        }

        internal static List<DirectItem> GetItems(DirectEve directEve, PyObject inventory, PyObject flag)
        {            
            var items = new List<DirectItem>();
            var cachedItems = inventory.Attribute("cachedItems").ToDictionary();
            var pyItems = cachedItems.Values;
            
            foreach (var pyItem in pyItems)
            {
                var item = new DirectItem(directEve);
                item.PyItem = pyItem;

                // Do not add the item if the flags do not coincide
                if (flag.IsValid && (int)flag != item.FlagId)
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

            return directEve.ThreadedLocalSvcCall("addressbook", "DropInPlaces", global::DirectEve.PySharp.PySharp.PyNone, data);
        }

        /// <summary>
        ///   Open up the quick-sell window to sell this item
        /// </summary>
        /// <returns></returns>
        public bool QuickSell()
        {
            if (!DirectEve.HasSupportInstances())
            {
                DirectEve.Log("DirectEve: Error: This method requires a support instance.");
                return false;
            }
            return DirectEve.ThreadedLocalSvcCall("marketutils", "Sell", TypeId, PyItem);
        }

        /// <summary>
        ///   Open up the quick-buy window to buy more of this item
        /// </summary>
        /// <returns></returns>
        public bool QuickBuy()
        {
            if (!DirectEve.HasSupportInstances())
            {
                DirectEve.Log("DirectEve: Error: This method requires a support instance.");
                return false;
            }
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
        ///   Assembles this ship
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        ///   Fails if the current location is not the same as the current station and if its not a CategoryShip and is not allready assembled
        /// </remarks>
        /// 
        public bool AssembleShip()
        {
            if (LocationId != DirectEve.Session.StationId)
                return false;

            if (CategoryId != (int)DirectEve.Const.CategoryShip)
                return false;

            if (IsSingleton)
                return false;

			PyObject AssembleShip = PySharp.Import("eve.client.script.ui.services.menuSvcExtras.invItemFunctions").Attribute("AssembleShip");
            return DirectEve.ThreadedCall(AssembleShip, new List<PyObject>() { this.PyItem });
        }

        /// <summary>
        /// Board this ship from a ship maintanance bay!
        /// </summary>
        /// <returns>false if entity is player or out of range</returns>
        public bool BoardShipFromShipMaintBay()
        {
            if (CategoryId != (int)DirectEve.Const.CategoryShip)
                return false;

            if (IsSingleton)
                return false;

			PyObject Board = PySharp.Import("eve.client.script.ui.services.menuSvcExtras.menuFunctions").Attribute("Board");
			return DirectEve.ThreadedCall(Board, ItemId);
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

        /// <summary>
        ///   Inject the skill into your brain
        /// </summary>
        /// <returns></returns>
        public bool InjectSkill()
        {
            if (CategoryId != (int) DirectEve.Const.CategorySkill)
                return false;

            if (!DirectEve.Session.StationId.HasValue || LocationId != DirectEve.Session.StationId)
                return false;

            if (ItemId == 0 || !PyItem.IsValid)
                return false;

			PyObject InjectSkillIntoBrain = PySharp.Import("eve.client.script.ui.services.menuSvcExtras.invItemFunctions").Attribute("InjectSkillIntoBrain");
			return DirectEve.ThreadedCall(InjectSkillIntoBrain, new List<PyObject> { PyItem });
        }

        /// <summary>
        /// Set the name of an item.  Be sure to call DirectEve.ScatterEvent("OnItemNameChange") shortly after calling this function.  Do not call ScatterEvent from the same frame!!
        /// </summary>
        /// <remarks>See menuSvc.SetName</remarks>
        /// <param name="name">The new name for this item.</param>
        /// <returns>true if successful.  false if not.</returns>
        public bool SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            if (CategoryId != (int)DirectEve.Const.CategoryShip && name.Length > 20)
            {
                return false;
            }

            if (CategoryId != (int)DirectEve.Const.CategoryStructure && name.Length > 32)
            {
                return false;
            }

            if (name.Length > 100)
            {
                return false;
            }

            if (ItemId == 0 || !PyItem.IsValid)
                return false;

            var pyCall = DirectEve.GetLocalSvc("invCache").Call("GetInventoryMgr").Attribute("SetLabel");
            return DirectEve.ThreadedCall(pyCall, ItemId, name.Replace('\n', ' '));
        }

        public bool ActivatePLEX()
        {
            if (this.TypeId != 29668)
                return false;

			PyObject ApplyPilotLicence = PySharp.Import("eve.client.script.ui.services.menuSvcExtras.menuFunctions").Attribute("ApplyPilotLicence");
			return DirectEve.ThreadedCall(ApplyPilotLicence, ItemId);
        }
    }
}