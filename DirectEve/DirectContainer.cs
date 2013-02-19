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

    public class DirectContainer : DirectInvType
    {
        /// <summary>
        ///   Item Id
        /// </summary>
        private long _itemId;

        /// <summary>
        ///   Items cache
        /// </summary>
        private List<DirectItem> _items;

        /// <summary>
        ///   Flag reference
        /// </summary>
        private PyObject _pyFlag;

        /// <summary>
        ///   Inventory reference
        /// </summary>
        private PyObject _pyInventory;

        /// <summary>
        ///   Is this the ship's modules 'container'
        /// </summary>
        private bool _shipModules;

        /// <summary>
        ///   Associated window cache
        /// </summary>
        private DirectContainerWindow _window;

        /// <summary>
        ///   Window name
        /// </summary>
        private string _windowName;

        internal DirectContainer(DirectEve directEve, PyObject pyInventory, PyObject pyFlag)
            : base(directEve)
        {
            _pyInventory = pyInventory;
            _pyFlag = pyFlag;

            TypeId = (int) pyInventory.Attribute("typeID");

            if (!pyInventory.Attribute("listed").IsValid || pyInventory.Attribute("listed").IsNull || pyInventory.Attribute("listed").IsNone)
                DirectItem.RefreshItems(directEve, pyInventory, pyFlag);
        }

        /// <summary>
        ///   DirectContainer
        /// </summary>
        /// <param name = "directEve"></param>
        /// <param name = "pyInventory"></param>
        /// <param name = "pyFlag"></param>
        /// <param name = "windowName"></param>
        internal DirectContainer(DirectEve directEve, PyObject pyInventory, PyObject pyFlag, string windowName)
            : this(directEve, pyInventory, pyFlag)
        {
            _windowName = windowName;
        }

        /// <summary>
        ///   DirectContainer
        /// </summary>
        /// <param name = "directEve"></param>
        /// <param name = "pyInventory"></param>
        /// <param name = "pyFlag"></param>
        /// <param name = "itemId"></param>
        internal DirectContainer(DirectEve directEve, PyObject pyInventory, PyObject pyFlag, long itemId)
            : this(directEve, pyInventory, pyFlag)
        {
            _itemId = itemId;
            _windowName = string.Empty;
        }

        /// <summary>
        ///   DirectContainer
        /// </summary>
        /// <param name = "directEve"></param>
        /// <param name = "pyInventory"></param>
        /// <param name = "shipModules"></param>
        internal DirectContainer(DirectEve directEve, PyObject pyInventory, bool shipModules) : base(directEve)
        {
            // You can't build a DirectContainer with these parameters if its not shipModules
            if (!shipModules)
                throw new Exception("Invalid container");

            _pyInventory = pyInventory;
            _pyFlag = global::DirectEve.PySharp.PySharp.PyNone;
            _windowName = string.Empty;
            _shipModules = true;
        }


        /// <summary>
        ///   Get the items from the container
        /// </summary>
        public List<DirectItem> Items
        {
            get
            {
                if (_items == null)
                {
                    _items = DirectItem.GetItems(DirectEve, _pyInventory, _pyFlag);

                    // Special case
                    var categoryShip = (int)DirectEve.Const.CategoryShip;

                    if (_windowName.Contains("StationItems"))                        
                        _items.RemoveAll(i => i.CategoryId == categoryShip);
                    if (_windowName.Contains("StationShips"))                        
                        _items.RemoveAll(i => i.CategoryId != categoryShip);

                    // Special case #2 (filter out hi/med/low slots)
                    if (_shipModules)
                    {
                        var flags = new List<int>();
                        for (var i = 0; i < 8; i ++)
                        {
                            flags.Add((int) DirectEve.Const["flagHiSlot" + i]);
                            flags.Add((int) DirectEve.Const["flagMedSlot" + i]);
                            flags.Add((int) DirectEve.Const["flagLoSlot" + i]);
                        }

                        _items.RemoveAll(i => !flags.Any(f => f == i.FlagId));
                    }
                }

                return _items;
            }
        }

        /// <summary>
        ///   Get the associated window for this container
        /// </summary>
        public DirectContainerWindow Window
        {
            get
            {
                if (_shipModules)
                    return null;

                if (_window == null && !string.IsNullOrEmpty(_windowName))
                {
                    _window = DirectEve.Windows.OfType<DirectContainerWindow>().FirstOrDefault(w => w.Name.Contains(_windowName));                    
                }

                if (_window == null && _itemId != 0)
                {

                    _window = DirectEve.Windows.OfType<DirectContainerWindow>().FirstOrDefault(w => w.Name.Contains("Secondary") && w.currInvIdItem == _itemId);
                    if (_window == null)
                        _window = DirectEve.Windows.OfType<DirectContainerWindow>().FirstOrDefault(w => w.currInvIdItem == _itemId);

                }

                return _window;
            }
        }

        /// <summary>
        ///   Is the container valid?
        /// </summary>
        /// <remarks>
        ///   Valid is not the same as ready!
        /// </remarks>
        public bool IsValid
        {
            get { return _pyInventory.IsValid && _pyInventory.Attribute("listed").IsValid; }
        }

        /// <summary>
        ///   Is the container ready?
        /// </summary>
        /// <remarks>
        ///   For the container to be ready, you need the container window to exist and for that window to be "ready"
        /// </remarks>
        public bool IsReady
        {
            get
            {
                if (_shipModules)
                    return true;

                if (Window == null)
                    return false;

                return Window.IsReady && IsValid;
            }
        }

        /// <summary>
        ///   Return the container's capacity
        /// </summary>
        /// <returns></returns>
        public new double Capacity
        {
            get
            {
                if (_shipModules)
                    return 0;

                return (double) (_pyFlag.IsValid ? _pyInventory.Call("GetCapacity", _pyFlag) : _pyInventory.Call("GetCapacity")).Attribute("capacity");
            }
        }

        /// <summary>
        ///   Return the container's capacity
        /// </summary>
        /// <returns></returns>
        public double UsedCapacity
        {
            get
            {
                if (_shipModules)
                    return 0;

                return (double) (_pyFlag.IsValid ? _pyInventory.Call("GetCapacity", _pyFlag) : _pyInventory.Call("GetCapacity")).Attribute("used");
            }
        }

        /// <summary>
        ///   Get the inventory object using the specified method (GetInventory or GetInventoryFromId) and an Id (e.g. ship-id, etc)
        /// </summary>
        /// <param name = "directEve"></param>
        /// <param name = "method"></param>
        /// <param name = "id"></param>
        /// <returns></returns>
        private static PyObject GetInventory(DirectEve directEve, string method, long id)
        {
            var inventories = directEve.GetLocalSvc("invCache").Attribute("inventories").ToDictionary();
            foreach (var inventory in inventories)
            {
                var keyid = (long) inventory.Key.Item(0);
                if (keyid != id)
                    continue;

                return inventory.Value;
            }

            // Do a threaded call and consider this failed (for now)
            directEve.ThreadedLocalSvcCall("invCache", method, id);
            // Return none
            return global::DirectEve.PySharp.PySharp.PyNone;
        }

        /// <summary>
        ///   Get the corporation hangar container based on division name
        /// </summary>
        /// <param name = "directEve"></param>
        /// <param name = "divisionName"></param>
        /// <returns></returns>
        internal static DirectContainer GetCorporationHangar(DirectEve directEve, string divisionName)
        {
            var divisions = directEve.GetLocalSvc("corp").Call("GetDivisionNames");
            for (var i = 1; i <= 7; i ++)
            {
                if (string.Compare(divisionName, (string) divisions.DictionaryItem(i), true) == 0)
                    return GetCorporationHangar(directEve, i);
            }

            return new DirectContainer(directEve, global::DirectEve.PySharp.PySharp.PyZero, global::DirectEve.PySharp.PySharp.PyZero, string.Empty);
        }

        /// <summary>
        ///   Get the corporation hangar container based on division id (1-7)
        /// </summary>
        /// <param name = "directEve"></param>
        /// <param name = "divisionId"></param>
        /// <returns></returns>
        internal static DirectContainer GetCorporationHangar(DirectEve directEve, int divisionId)
        {
            PyObject flag = null;
            switch (divisionId)
            {
                case 1:
                    flag = directEve.Const.FlagHangar;
                    break;
                case 2:
                    flag = directEve.Const.FlagCorpSAG2;
                    break;
                case 3:
                    flag = directEve.Const.FlagCorpSAG3;
                    break;
                case 4:
                    flag = directEve.Const.FlagCorpSAG4;
                    break;
                case 5:
                    flag = directEve.Const.FlagCorpSAG5;
                    break;
                case 6:
                    flag = directEve.Const.FlagCorpSAG6;
                    break;
                case 7:
                    flag = directEve.Const.FlagCorpSAG7;
                    break;
            }

            if (flag == null)
                return new DirectContainer(directEve, global::DirectEve.PySharp.PySharp.PyZero, global::DirectEve.PySharp.PySharp.PyZero, string.Empty);

            var itemId = (long) directEve.GetLocalSvc("corp").Call("GetOffice", directEve.Session.CorporationId).Attribute("itemID");
            var inventory = GetInventory(directEve, "GetInventoryFromId", itemId);
            return new DirectContainer(directEve, inventory, flag, "('StationCorpHangars_");
        }

        internal static DirectContainer GetCorporationHangarArray(DirectEve directEve, long itemId, string divisionName)
        {
            var divisions = directEve.GetLocalSvc("corp").Call("GetDivisionNames");
            for (var i = 1; i <= 7; i++)
            {
                if (string.Compare(divisionName, (string)divisions.DictionaryItem(i), true) == 0)
                    return GetCorporationHangarArray(directEve, itemId, i);
            }

            return new DirectContainer(directEve, global::DirectEve.PySharp.PySharp.PyZero, global::DirectEve.PySharp.PySharp.PyZero, string.Empty);
        }

        internal static DirectContainer GetCorporationHangarArray(DirectEve directEve, long itemId, int divisionId)
        {
            PyObject flag = null;
            switch (divisionId)
            {
                case 1:
                    flag = directEve.Const.FlagHangar;
                    break;
                case 2:
                    flag = directEve.Const.FlagCorpSAG2;
                    break;
                case 3:
                    flag = directEve.Const.FlagCorpSAG3;
                    break;
                case 4:
                    flag = directEve.Const.FlagCorpSAG4;
                    break;
                case 5:
                    flag = directEve.Const.FlagCorpSAG5;
                    break;
                case 6:
                    flag = directEve.Const.FlagCorpSAG6;
                    break;
                case 7:
                    flag = directEve.Const.FlagCorpSAG7;
                    break;
            }

            if (flag == null)
                return new DirectContainer(directEve, global::DirectEve.PySharp.PySharp.PyZero, global::DirectEve.PySharp.PySharp.PyZero, string.Empty);

            var inventory = GetInventory(directEve, "GetInventoryFromId", itemId);
            return new DirectContainer(directEve, inventory, flag, "('POSCorpHangar_" + itemId + "_" + (divisionId - 1));
        }

        /// <summary>
        ///   Get the item hangar container
        /// </summary>
        /// <param name = "directEve"></param>
        /// <returns></returns>
        internal static DirectContainer GetItemHangar(DirectEve directEve)
        {
            var inventory = GetInventory(directEve, "GetInventory", (long)directEve.Const.ContainerHangar);
            return new DirectContainer(directEve, inventory, directEve.Const.FlagHangar, "('StationItems");            
        }

        /// <summary>
        ///   Get the ship hangar container
        /// </summary>
        /// <param name = "directEve"></param>
        /// <returns></returns>
        internal static DirectContainer GetShipHangar(DirectEve directEve)
        {
            var inventory = GetInventory(directEve, "GetInventory", (long)directEve.Const.ContainerHangar);
            return new DirectContainer(directEve, inventory, directEve.Const.FlagHangar, "('StationShips', ");
        }

        /// <summary>
        ///   Get the ship's cargo container
        /// </summary>
        /// <param name = "directEve"></param>
        /// <returns></returns>
        internal static DirectContainer GetShipsCargo(DirectEve directEve)
        {
            if (!directEve.Session.ShipId.HasValue)
                return new DirectContainer(directEve, global::DirectEve.PySharp.PySharp.PyZero, global::DirectEve.PySharp.PySharp.PyZero, string.Empty);

            var inventory = GetInventory(directEve, "GetInventoryFromId", directEve.Session.ShipId.Value);
            return new DirectContainer(directEve, inventory, directEve.Const.FlagCargo, "('ActiveShipCargo', ");
        }

        /// <summary>
        ///   Get the ship's ore hold
        /// </summary>
        /// <param name = "directEve"></param>
        /// <returns></returns>
        internal static DirectContainer GetShipsOreHold(DirectEve directEve)
        {
            if (!directEve.Session.ShipId.HasValue)
                return new DirectContainer(directEve, global::DirectEve.PySharp.PySharp.PyZero, global::DirectEve.PySharp.PySharp.PyZero, string.Empty);

            var inventory = GetInventory(directEve, "GetInventoryFromId", directEve.Session.ShipId.Value);
            return new DirectContainer(directEve, inventory, directEve.Const.FlagOreHold, "('ShipOreHold', ");
        }

        /// <summary>
        ///   Get the ship's modules 'container'
        /// </summary>
        /// <param name = "directEve"></param>
        /// <returns></returns>
        internal static DirectContainer GetShipsModules(DirectEve directEve)
        {
            if (!directEve.Session.ShipId.HasValue)
                return new DirectContainer(directEve, global::DirectEve.PySharp.PySharp.PyZero, global::DirectEve.PySharp.PySharp.PyZero, string.Empty);

            var inventory = GetInventory(directEve, "GetInventoryFromId", directEve.Session.ShipId.Value);
            return new DirectContainer(directEve, inventory, true);
        }

        /// <summary>
        ///   Get the ship's drone container
        /// </summary>
        /// <param name = "directEve"></param>
        /// <returns></returns>
        internal static DirectContainer GetShipsDroneBay(DirectEve directEve)
        {
            if (!directEve.Session.ShipId.HasValue)
                return new DirectContainer(directEve, global::DirectEve.PySharp.PySharp.PyZero, global::DirectEve.PySharp.PySharp.PyZero, string.Empty);

            var inventory = GetInventory(directEve, "GetInventoryFromId", directEve.Session.ShipId.Value);
            return new DirectContainer(directEve, inventory, directEve.Const.FlagDroneBay, "('ShipDroneBay_" + directEve.Session.ShipId.Value);
        }

        /// <summary>
        ///   Get a item container
        /// </summary>
        /// <param name = "directEve"></param>
        /// <param name = "itemId"></param>
        /// <returns></returns>
        internal static DirectContainer GetContainer(DirectEve directEve, long itemId)
        {
            var inventory = GetInventory(directEve, "GetInventoryFromId", itemId);
            return new DirectContainer(directEve, inventory, global::DirectEve.PySharp.PySharp.PyNone, itemId);
        }

        /// <summary>
        ///   Stack all the items in the container
        /// </summary>
        /// <returns></returns>
        public bool StackAll()
        {
            // You can't stack modules
            if (_shipModules)
                return false;

            return _pyFlag.IsValid ? DirectEve.ThreadedCall(_pyInventory.Attribute("StackAll"), _pyFlag) : DirectEve.ThreadedCall(_pyInventory.Attribute("StackAll"));
        }

        /// <summary>
        ///   Add an item to this container
        /// </summary>
        /// <param name = "item"></param>
        /// <returns></returns>
        public bool Add(DirectItem item)
        {
            return Add(item, item.Stacksize);
        }

        /// <summary>
        ///   Add an item to this container
        /// </summary>
        /// <param name = "item"></param>
        /// <param name = "quantity"></param>
        /// <returns></returns>
        public bool Add(DirectItem item, int quantity)
        {
            // You can't fit modules like this
            if (_shipModules)
                return false;

            if (item.LocationId == -1 || quantity < 1)
                return false;

            var keywords = new Dictionary<string, object>();
            keywords.Add("qty", quantity);
            if (_pyFlag.IsValid)
                keywords.Add("flag", _pyFlag);
            if (!_pyFlag.IsValid && GroupId == (int) DirectEve.Const.GroupAuditLogSecureContainer)
                keywords.Add("flag", DirectEve.Const.FlagUnlocked);
            return DirectEve.ThreadedCallWithKeywords(_pyInventory.Attribute("Add"), keywords, item.ItemId, item.LocationId);
        }

        /// <summary>
        ///   Add multiple items to this container
        /// </summary>
        /// <param name = "items"></param>
        /// <returns></returns>
        public bool Add(IEnumerable<DirectItem> items)
        {
            // You can't fit modules like this
            if (_shipModules)
                return false;

            if (items.Count() == 0)
                return true;

            var keywords = new Dictionary<string, object>();
            if (_pyFlag.IsValid)
                keywords.Add("flag", _pyFlag);
            if (!_pyFlag.IsValid && GroupId == (int) DirectEve.Const.GroupAuditLogSecureContainer)
                keywords.Add("flag", DirectEve.Const.FlagUnlocked);
            return DirectEve.ThreadedCallWithKeywords(_pyInventory.Attribute("MultiAdd"), keywords, items.Select(i => i.ItemId), items.First().LocationId);
        }

        /// <summary>
        ///   Add multiple bookmarks to this container
        /// </summary>
        /// <param name = "bookmarkIds"></param>
        /// <returns></returns>
        /// <remarks>
        ///   This only works on shipCargo &amp; hangarFloor containers and only 5 bookmarks per AddBookmarks call
        /// </remarks>
        public bool AddBookmarks(IEnumerable<long> bookmarkIds)
        {
            if (!(Window is DirectOwnContainerWindow))
                return false;

            return (Window as DirectOwnContainerWindow).AddBookmarks(bookmarkIds);
        }

        /// <summary>
        ///   Jettison item
        /// </summary>
        /// <param name = "itemId"></param>
        /// <returns></returns>
        /// <remarks>
        ///   This will fail on items not located in the ship's cargo hold
        /// </remarks>
        public bool Jettison(long itemId)
        {
            return Jettison(new[] {itemId});
        }

        /// <summary>
        ///   Jettison items
        /// </summary>
        /// <param name = "itemIds"></param>
        /// <returns></returns>
        /// <remarks>
        ///   This will fail on items not located in the ship's cargo hold
        /// </remarks>
        public bool Jettison(IEnumerable<long> itemIds)
        {
            // You can't jettison modules
            if (_shipModules)
                return false;

            if (itemIds.Count() == 0)
                return true;

            var jettison = DirectEve.GetLocalSvc("gameui").Call("GetShipAccess").Attribute("Jettison");
            return DirectEve.ThreadedCall(jettison, itemIds);
        }
    }
}