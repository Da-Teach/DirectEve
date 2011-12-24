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
    using LavishScriptAPI;

    public class DirectEve : IDisposable
    {
        /// <summary>
        ///   ActiveShip cache
        /// </summary>
        private DirectActiveShip _activeShip;

        /// <summary>
        ///   Cache the Agent Missions
        /// </summary>
        private List<DirectAgentMission> _agentMissions;

        /// <summary>
        ///   Cache the Bookmarks
        /// </summary>
        private List<DirectBookmark> _bookmarks;

        /// <summary>
        ///   Cache the Bookmark Folders
        /// </summary>
        private List<DirectBookmarkFolder> _bookmarkFolders;

        /// <summary>
        ///   Const cache
        /// </summary>
        private DirectConst _const;

        /// <summary>
        ///   Item container cache
        /// </summary>
        private Dictionary<long, DirectContainer> _containers;

        /// <summary>
        ///   Cache the Entities
        /// </summary>
        private Dictionary<long, DirectEntity> _entitiesById;

        private uint _innerspaceOnFrameId;

        /// <summary>
        ///   Item Hangar container cache
        /// </summary>
        private DirectContainer _itemHangar;

        /// <summary>
        ///   Info on when a certain target was last targeted
        /// </summary>
        private Dictionary<long, DateTime> _lastKnownTargets;

        /// <summary>
        ///   Cache the LocalSvc objects
        /// </summary>
        private Dictionary<string, PyObject> _localSvcCache;

        /// <summary>
        ///   Login cache
        /// </summary>
        private DirectLogin _login;

        /// <summary>
        ///   Me cache
        /// </summary>
        private DirectMe _me;

        /// <summary>
        ///   Cache the Windows
        /// </summary>
        private List<DirectModule> _modules;

        /// <summary>
        ///   Navigation cache
        /// </summary>
        private DirectNavigation _navigation;

        /// <summary>
        ///   Session cache
        /// </summary>
        private DirectSession _session;

        /// <summary>
        ///   Ship Hangar container cache
        /// </summary>
        private DirectContainer _shipHangar;

        /// <summary>
        ///   Ship's cargo container cache
        /// </summary>
        private DirectContainer _shipsCargo;

        /// <summary>
        ///   Ship's drone bay cache
        /// </summary>
        private DirectContainer _shipsDroneBay;

        /// <summary>
        ///   Ship's modules container cache
        /// </summary>
        private DirectContainer _shipsModules;

        /// <summary>
        ///   Standings cache
        /// </summary>
        private DirectStandings _standings;

        /// <summary>
        ///   Cache the GetWindows call
        /// </summary>
        private List<DirectWindow> _windows;

        /// <summary>
        ///   Cache the GetStations call
        /// </summary>
        private Dictionary<long, DirectStation> _stations;

        /// <summary>
        ///   Cache the GetRegions call
        /// </summary>
        private Dictionary<long, DirectSolarSystem> _solarSystems;
        
        /// <summary>
        ///   Cache the GetConstellations call
        /// </summary>
        private Dictionary<long, DirectConstellation> _constellations;
        
        /// <summary>
        ///   Cache the GetRegions call
        /// </summary>
        private Dictionary<long, DirectRegion> _regions;

        /// <summary>
        ///   Create a DirectEve object
        /// </summary>
        public DirectEve()
        {
            _localSvcCache = new Dictionary<string, PyObject>();
            _containers = new Dictionary<long, DirectContainer>();
            _lastKnownTargets = new Dictionary<long, DateTime>();

            _innerspaceOnFrameId = LavishScript.Events.RegisterEvent("OnFrame");
            LavishScript.Events.AttachEventTarget(_innerspaceOnFrameId, InnerspaceOnFrame);
        }

        /// <summary>
        ///   Return a DirectConst object
        /// </summary>
        internal DirectConst Const
        {
            get { return _const ?? (_const = new DirectConst(this)); }
        }

        /// <summary>
        ///   Return a DirectNavigation object
        /// </summary>
        public DirectLogin Login
        {
            get { return _login ?? (_login = new DirectLogin(this)); }
        }

        /// <summary>
        ///   Return a DirectNavigation object
        /// </summary>
        public DirectNavigation Navigation
        {
            get { return _navigation ?? (_navigation = new DirectNavigation(this)); }
        }

        /// <summary>
        ///   Return a DirectMe object
        /// </summary>
        public DirectMe Me
        {
            get { return _me ?? (_me = new DirectMe(this)); }
        }

        /// <summary>
        ///   Return a DirectStandings object
        /// </summary>
        public DirectStandings Standings
        {
            get { return _standings ?? (_standings = new DirectStandings(this)); }
        }

        /// <summary>
        ///   Return a DirectActiveShip object
        /// </summary>
        public DirectActiveShip ActiveShip
        {
            get { return _activeShip ?? (_activeShip = new DirectActiveShip(this)); }
        }

        /// <summary>
        ///   Return a DirectSession object
        /// </summary>
        public DirectSession Session
        {
            get { return _session ?? (_session = new DirectSession(this)); }
        }

        /// <summary>
        ///   Internal reference to the PySharp object that is used for the frame
        /// </summary>
        /// <remarks>
        ///   This reference is only valid while in an OnFrame event
        /// </remarks>
        internal PySharp.PySharp PySharp { get; private set; }

        /// <summary>
        ///   Return a list of entities
        /// </summary>
        /// <value></value>
        /// <remarks>
        ///   Only works in space
        /// </remarks>
        public List<DirectEntity> Entities
        {
            get { return EntitiesById.Values.ToList(); }
        }

        /// <summary>
        ///   Return a dictionary of entities by id
        /// </summary>
        /// <value></value>
        /// <remarks>
        ///   Only works in space
        /// </remarks>
        public Dictionary<long, DirectEntity> EntitiesById
        {
            get { return _entitiesById ?? (_entitiesById = DirectEntity.GetEntities(this)); }
        }

        /// <summary>
        ///   Refresh the bookmark cache (if needed)
        /// </summary>
        /// <returns></returns>
        public bool RefreshBookmarks()
        {
            return DirectBookmark.RefreshBookmarks(this);
        }

        /// <summary>
        ///   The last bookmark update
        /// </summary>
        public DateTime LastBookmarksUpdate
        {
            get { return DirectBookmark.GetLastBookmarksUpdate(this) ?? new DateTime(0, 0, 0); }
        }

        /// <summary>
        ///   Return a list of bookmarks
        /// </summary>
        /// <value></value>
        public List<DirectBookmark> Bookmarks
        {
            get { return _bookmarks ?? (_bookmarks = DirectBookmark.GetBookmarks(this)); }
        }

        /// <summary>
        ///   Return a list of bookmark folders
        /// </summary>
        /// <value></value>
        public List<DirectBookmarkFolder> BookmarkFolders
        {
            get { return _bookmarkFolders ?? (_bookmarkFolders = DirectBookmark.GetFolders(this)); }
        }

        /// <summary>
        ///   Return a list of agent missions
        /// </summary>
        /// <value></value>
        public List<DirectAgentMission> AgentMissions
        {
            get { return _agentMissions ?? (_agentMissions = DirectAgentMission.GetAgentMissions(this)); }
        }

        /// <summary>
        ///   Return a list of all open windows
        /// </summary>
        /// <value></value>
        public List<DirectWindow> Windows
        {
            get { return _windows ?? (_windows = DirectWindow.GetWindows(this)); }
        }

        /// <summary>
        ///   Return a list of all modules
        /// </summary>
        /// <value></value>
        /// <remarks>
        ///   Only works inspace and does not return hidden modules
        /// </remarks>
        public List<DirectModule> Modules
        {
            get { return _modules ?? (_modules = DirectModule.GetModules(this)); }
        }

        /// <summary>
        ///   Return active drone id's
        /// </summary>
        /// <value></value>
        public List<DirectEntity> ActiveDrones
        {
            get
            {
                var droneIds = GetLocalSvc("michelle").Call("GetDrones").Attribute("items").ToDictionary<long>().Keys;
                return Entities.Where(e => droneIds.Any(d => d == e.Id)).ToList();
            }
        }

        /// <summary>
        ///   Return a dictionary of stations
        /// </summary>
        /// <remarks>This is cached throughout the existance of this DirectEve Instance</remarks>
        public Dictionary<long, DirectStation> Stations
        {
            get { return _stations ?? (_stations = DirectStation.GetStations(this)); }
        }

        /// <summary>
        ///   Return a dictionary of solar systems
        /// </summary>
        /// <remarks>This is cached throughout the existance of this DirectEve Instance</remarks>
        public Dictionary<long, DirectSolarSystem> SolarSystems
        {
            get { return _solarSystems ?? (_solarSystems = DirectSolarSystem.GetSolarSystems(this)); }
        }

        /// <summary>
        ///   Return a dictionary of solar systems
        /// </summary>
        /// <remarks>This is cached throughout the existance of this DirectEve Instance</remarks>
        public Dictionary<long, DirectConstellation> Constellations
        {
            get { return _constellations ?? (_constellations = DirectConstellation.GetConstellations(this)); }
        }

        /// <summary>
        ///   Return a dictionary of solar systems
        /// </summary>
        /// <remarks>This is cached throughout the existance of this DirectEve Instance</remarks>
        public Dictionary<long, DirectRegion> Regions
        {
            get { return _regions ?? (_regions = DirectRegion.GetRegions(this)); }
        }

        /// <summary>
        ///   Is EVE rendering 3D, you can enable/disable rendering by setting this value to true or false
        /// </summary>
        /// <remarks>
        ///   Only works in space!
        /// </remarks>
        public bool Rendering3D
        {
            get
            {
                var rendering1 = (bool) GetLocalSvc("sceneManager").Attribute("registeredScenes").DictionaryItem("default").Attribute("display");
                var rendering2 = (bool) GetLocalSvc("sceneManager").Attribute("registeredScenes2").DictionaryItem("default").Attribute("display");
                return rendering1 && rendering2;
            }
            set
            {
                GetLocalSvc("sceneManager").Attribute("registeredScenes").DictionaryItem("default").SetAttribute("display", value);
                GetLocalSvc("sceneManager").Attribute("registeredScenes2").DictionaryItem("default").SetAttribute("display", value);
            }
        }

        #region IDisposable Members

        /// <summary>
        ///   Dispose of DirectEve
        /// </summary>
        public void Dispose()
        {
            LavishScript.Events.DetachEventTarget(_innerspaceOnFrameId, InnerspaceOnFrame);
        }

        #endregion

        /// <summary>
        ///   OnFrame event, use this to do your eve-stuff
        /// </summary>
        public event EventHandler OnFrame;

        /// <summary>
        ///   Internal "OnFrame" handler
        /// </summary>
        /// <param name = "sender"></param>
        /// <param name = "e"></param>
        private void InnerspaceOnFrame(object sender, LSEventArgs e)
        {
            using (var pySharp = new PySharp.PySharp(true))
            {
                // Make the link to the instance
                PySharp = pySharp;

                // Get current target list
                var targets = pySharp.Import("__builtin__").Attribute("sm").Attribute("services").DictionaryItem("target").Attribute("targets").ToList<long>();
                targets.AddRange(pySharp.Import("__builtin__").Attribute("sm").Attribute("services").DictionaryItem("target").Attribute("targeting").ToList<long>());
                // Update currently locked targets
                targets.ForEach(t => _lastKnownTargets[t] = DateTime.Now);
                // Remove all targets that have not been locked for 3 seconds
                foreach (var t in _lastKnownTargets.Keys.ToArray())
                {
                    if (DateTime.Now.AddSeconds(-3) < _lastKnownTargets[t])
                        continue;

                    _lastKnownTargets.Remove(t);
                }

                if (OnFrame != null)
                    OnFrame(this, new EventArgs());

                // Clear any cache that we had during this frame
                _localSvcCache.Clear();
                _entitiesById = null;
                _windows = null;
                _modules = null;
                _const = null;
                _bookmarks = null;
                _agentMissions = null;

                _containers.Clear();
                _itemHangar = null;
                _shipHangar = null;
                _shipsCargo = null;
                _shipsModules = null;
                _shipsDroneBay = null;
                _me = null;
                _activeShip = null;
                _standings = null;
                _navigation = null;
                _session = null;
                _login = null;

                // Remove the link
                PySharp = null;
            }
        }

        /// <summary>
        ///   Open the corporation hangar
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        ///   Only works in a station!
        /// </remarks>
        public bool OpenCorporationHangar()
        {
            return ThreadedLocalSvcCall("window", "OpenCorpHangar", global::DirectEve.PySharp.PySharp.PyNone, global::DirectEve.PySharp.PySharp.PyNone, 1);
        }

        /// <summary>
        ///   Execute a command
        /// </summary>
        /// <param name = "cmd"></param>
        /// <returns></returns>
        public bool ExecuteCommand(DirectCmd cmd)
        {
            return ThreadedLocalSvcCall("cmd", cmd.ToString());
        }

        /// <summary>
        ///   Return a list of locked items
        /// </summary>
        /// <returns></returns>
        public List<long> GetLockedItems()
        {
            var locks = GetLocalSvc("invCache").Attribute("lockedItems").ToDictionary<long>();
            return locks.Keys.ToList();
        }

        /// <summary>
        ///   Remove all item locks
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        ///   Do not abuse this, the client probably placed them for a reason!
        /// </remarks>
        public bool UnlockItems()
        {
            return GetLocalSvc("invCache").Attribute("lockedItems").Clear();
        }

        /// <summary>
        ///   Item hangar container
        /// </summary>
        /// <returns></returns>
        public DirectContainer GetItemHangar()
        {
            return _itemHangar ?? (_itemHangar = DirectContainer.GetItemHangar(this));
        }

        /// <summary>
        ///   Ship hangar container
        /// </summary>
        /// <returns></returns>
        public DirectContainer GetShipHangar()
        {
            return _shipHangar ?? (_shipHangar = DirectContainer.GetShipHangar(this));
        }

        /// <summary>
        ///   Ship's cargo container
        /// </summary>
        /// <returns></returns>
        public DirectContainer GetShipsCargo()
        {
            return _shipsCargo ?? (_shipsCargo = DirectContainer.GetShipsCargo(this));
        }

        /// <summary>
        ///   Ship's modules container
        /// </summary>
        /// <returns></returns>
        public DirectContainer GetShipsModules()
        {
            return _shipsModules ?? (_shipsModules = DirectContainer.GetShipsModules(this));
        }


        /// <summary>
        ///   Ship's drone bay
        /// </summary>
        /// <returns></returns>
        public DirectContainer GetShipsDroneBay()
        {
            return _shipsDroneBay ?? (_shipsDroneBay = DirectContainer.GetShipsDroneBay(this));
        }

        /// <summary>
        ///   Item container
        /// </summary>
        /// <param name = "itemId"></param>
        /// <returns></returns>
        public DirectContainer GetContainer(long itemId)
        {
            if (!_containers.ContainsKey(itemId))
                _containers[itemId] = DirectContainer.GetContainer(this, itemId);

            return _containers[itemId];
        }

        /// <summary>
        ///   Get the corporation hangar container based on division name
        /// </summary>
        /// <param name = "divisionName"></param>
        /// <returns></returns>
        public DirectContainer GetCorporationHangar(string divisionName)
        {
            return DirectContainer.GetCorporationHangar(this, divisionName);
        }

        /// <summary>
        ///   Get the corporation hangar container based on division id (1-7)
        /// </summary>
        /// <param name = "divisionId"></param>
        /// <returns></returns>
        public DirectContainer GetCorporationHangar(int divisionId)
        {
            return DirectContainer.GetCorporationHangar(this, divisionId);
        }

        /// <summary>
        ///   Return the entity by it's id
        /// </summary>
        /// <param name = "entityId"></param>
        /// <returns></returns>
        public DirectEntity GetEntityById(long entityId)
        {
            DirectEntity entity;
            if (EntitiesById.TryGetValue(entityId, out entity))
                return entity;

            return null;
        }


        /// <summary>
        ///   Bookmark the current location
        /// </summary>
        /// <param name = "name"></param>
        /// <param name = "comment"></param>
        /// <param name = "folderId"></param>
        /// <returns></returns>
        public bool BookmarkCurrentLocation(string name, string comment, long? folderId)
        {
            if (Session.StationId.HasValue)
            {
                var station = GetLocalSvc("station").Attribute("station");
                if (!station.IsValid)
                    return false;

                return DirectBookmark.BookmarkLocation(this, (long) station.Attribute("stationID"), name, comment, (int) station.Attribute("stationTypeID"), (long?) station.Attribute("solarSystemID"), folderId);
            }

            if (ActiveShip.Entity.IsValid && Session.SolarSystemId.HasValue)
                return DirectBookmark.BookmarkLocation(this, ActiveShip.Entity.Id, name, comment, ActiveShip.Entity.TypeId, Session.SolarSystemId, folderId);

            return false;
        }

        /// <summary>
        ///   Bookmark an entity
        /// </summary>
        /// <param name = "entity"></param>
        /// <param name = "name"></param>
        /// <param name = "comment"></param>
        /// <param name = "folderId"></param>
        /// <returns></returns>
        public bool BookmarkEntity(DirectEntity entity, string name, string comment, long? folderId)
        {
            if (!entity.IsValid)
                return false;

            return DirectBookmark.BookmarkLocation(this, entity.Id, name, comment, entity.TypeId, Session.SolarSystemId, folderId);
        }

        /// <summary>
        ///   Create a bookmark folder
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool CreateBookmarkFolder(string name)
        {
            return DirectBookmark.CreateBookmarkFolder(this, name);
        }

        /// <summary>
        ///   Drop bookmarks into people & places
        /// </summary>
        /// <param name = "bookmarks"></param>
        /// <returns></returns>
        public bool DropInPeopleAndPlaces(IEnumerable<DirectItem> bookmarks)
        {
            return DirectItem.DropInPlaces(this, bookmarks);
        }

        /// <summary>
        ///   Refine items from the hangar floor
        /// </summary>
        /// <param name = "items"></param>
        /// <returns></returns>
        public bool ReprocessStationItems(IEnumerable<DirectItem> items)
        {
            if (items == null)
                return false;

            if (items.Any(i => !i.PyItem.IsValid))
                return false;

            if (!Session.IsInStation)
                return false;

            if (items.Any(i => i.LocationId != Session.StationId))
                return false;

            return ThreadedLocalSvcCall("menu", "Refine", items.Select(i => i.PyItem));
        }

        /// <summary>
        ///   Return an owner
        /// </summary>
        /// <param name = "ownerId"></param>
        /// <returns></returns>
        public DirectOwner GetOwner(long ownerId)
        {
            return DirectOwner.GetOwner(this, ownerId);
        }

        /// <summary>
        ///   Return a location
        /// </summary>
        /// <param name = "locationId"></param>
        /// <returns></returns>
        public DirectLocation GetLocation(long locationId)
        {
            return DirectLocation.GetLocation(this, locationId);
        }

        /// <summary>
        ///   Return the name of a location
        /// </summary>
        /// <param name = "locationId"></param>
        /// <returns></returns>
        public string GetLocationName(long locationId)
        {
            return DirectLocation.GetLocationName(this, locationId);
        }

        /// <summary>
        ///   Return the agent by id
        /// </summary>
        /// <param name = "agentId"></param>
        /// <returns></returns>
        public DirectAgent GetAgentById(long agentId)
        {
            return DirectAgent.GetAgentById(this, agentId);
        }

        /// <summary>
        ///   Return the agent by name
        /// </summary>
        /// <param name = "agentName"></param>
        /// <returns></returns>
        public DirectAgent GetAgentByName(string agentName)
        {
            return DirectAgent.GetAgentByName(this, agentName);
        }

        /// <summary>
        ///   Return what "eve.LocalSvc" would return, unless the service wasn't started yet
        /// </summary>
        /// <param name = "svc"></param>
        /// <returns></returns>
        internal PyObject GetLocalSvc(string svc)
        {
            PyObject service;
            // Do we have a cached version (this is to stop overloading the LocalSvc call)
            if (_localSvcCache.TryGetValue(svc, out service))
                return service;

            // First try to get it from services
            service = PySharp.Import("__builtin__").Attribute("sm").Attribute("services").DictionaryItem(svc);

            // Add it to the cache (it doesn't matter if its not valid)
            _localSvcCache.Add(svc, service);

            // If its valid, return the service
            if (service.IsValid)
                return service;

            // Start the service in a ThreadedCall
            var localSvc = PySharp.Import("__builtin__").Attribute("eve").Attribute("LocalSvc");
            ThreadedCall(localSvc, svc);

            // Return an invalid PyObject (so that LocalSvc can start the service)
            return global::DirectEve.PySharp.PySharp.PyZero;
        }

        /// <summary>
        ///   Perform a uthread.new(pyCall, parms) call
        /// </summary>
        /// <param name = "pyCall"></param>
        /// <param name = "parms"></param>
        /// <returns></returns>
        internal bool ThreadedCall(PyObject pyCall, params object[] parms)
        {
            return ThreadedCallWithKeywords(pyCall, null, parms);
        }

        /// <summary>
        ///   Perform a uthread.new(pyCall, parms) call
        /// </summary>
        /// <param name = "pyCall"></param>
        /// <param name = "keywords"></param>
        /// <param name = "parms"></param>
        /// <returns></returns>
        internal bool ThreadedCallWithKeywords(PyObject pyCall, Dictionary<string, object> keywords, params object[] parms)
        {
            // Check specifically for this, as the call has to be valid (e.g. not null or none)
            if (!pyCall.IsValid)
                return false;

            return !PySharp.Import("uthread").CallWithKeywords("new", keywords, (new object[] {pyCall}).Concat(parms).ToArray()).IsNull;
        }

        /// <summary>
        ///   Perform a uthread.new(svc.call, parms) call
        /// </summary>
        /// <param name = "svc"></param>
        /// <param name = "call"></param>
        /// <param name = "parms"></param>
        /// <returns></returns>
        internal bool ThreadedLocalSvcCall(string svc, string call, params object[] parms)
        {
            var pyCall = GetLocalSvc(svc).Attribute(call);
            return ThreadedCall(pyCall, parms);
        }

        /// <summary>
        ///   Return's true if the entity has not been a target in the last 3 seconds
        /// </summary>
        /// <param name = "id"></param>
        /// <returns></returns>
        internal bool CanTarget(long id)
        {
            return !_lastKnownTargets.ContainsKey(id);
        }

        /// <summary>
        ///   Remove's the target from the last known targets
        /// </summary>
        /// <param name = "id"></param>
        internal void ClearTargetTimer(long id)
        {
            _lastKnownTargets.Remove(id);
        }

        /// <summary>
        ///   Set the target's last target time
        /// </summary>
        /// <param name = "id"></param>
        internal void SetTargetTimer(long id)
        {
            _lastKnownTargets[id] = DateTime.Now;
        }

        /// <summary>
        ///   Open the fitting management window
        /// </summary>
        public void OpenFitingManager()
        {
            var pySharp = this.PySharp;
            var form = pySharp.Import("form");
            ThreadedCall(form.Attribute("FittingMgmt").Attribute("Open"));
        }

        /// <summary>
        /// Read a script file and execute it
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool RunScript(string filename)
        {
            var pySharp = this.PySharp;
            string text = "";

            try
            {
                text = System.IO.File.ReadAllText(filename);
            }
            catch
            {
                var path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                var scriptPath = System.IO.Path.Combine(path, filename);
                text = System.IO.File.ReadAllText(scriptPath);
            }
            return pySharp.Run(text);
        }
    }
}