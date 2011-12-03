namespace DirectEveTesterV2
{
    using System;
    using System.Linq;
    using DirectEve;
    using System.Windows.Forms;
    using InnerSpaceAPI;

    public partial class MainForm : Form
    {
        private DirectEve _directEve;
        private TestState _state;

        private enum TestState
        {
            Idle,

            Session,
            Me,
            ActiveShip,
            LaunchDrones,
            RecallDrones,
            RefreshBookmarks,
            ListBookmarks,
            BookmarkCurrentLocation,
            DeleteBookmark,

            Done
        }


        public MainForm()
        {
            InitializeComponent();

            foreach (var state in Enum.GetNames(typeof(TestState)))
                TestStatesComboBox.Items.Add(state);
        
            _directEve = new DirectEve();
            _directEve.OnFrame += OnFrame;
        }

        private void Log(string format, params object[] parms)
        {
            InnerSpace.Echo(string.Format("{0:HH:mm:ss} {1}", DateTime.Now, string.Format(format, parms)));
        }

        private void OnFrame(object sender, EventArgs e)
        {
            if (_directEve == null)
                return;

            try
            {
                switch (_state)
                {
                    case TestState.Session:
                        SessionTests();
                        break;

                    case TestState.Me:
                        MeTests();
                        break;

                    case TestState.ActiveShip:
                        ActiveShipTests();
                        break;

                    case TestState.LaunchDrones:
                        LaunchDronesTest();
                        break;

                    case TestState.RecallDrones:
                        RecallDronesTest();
                        break;

                    case TestState.ListBookmarks:
                        ListBookmarksTest();
                        break;

                    case TestState.RefreshBookmarks:
                        RefreshBookmarksTest();
                        break;

                    case TestState.BookmarkCurrentLocation:
                        BookmarkCurrentLocationTest();
                        break;

                    case TestState.DeleteBookmark:
                        DeleteBookmarkTest();
                        break;
                }
            }
            catch (Exception ex)
            {
                Log("Exception: {0}", ex);
            }
        }

        private void DeleteBookmarkTest()
        {
            _state = TestState.Idle;

            var bookmark = _directEve.Bookmarks.FirstOrDefault(b => b.Title == "Wassup");
            if (bookmark == null)
            {
                Log("No Test Bookmark to delete");
                return;
            }

            bookmark.Delete();
        }

        private void BookmarkCurrentLocationTest()
        {
            _state = TestState.Idle;

            long? folderId = null;
            if (_directEve.BookmarkFolders.Count > 0)
                folderId = _directEve.BookmarkFolders[0].Id;

            _directEve.BookmarkCurrentLocation("Wassup", "This is the drinking bar", folderId);
        }

        private void RefreshBookmarksTest()
        {
            _state = TestState.Idle;

            Log("Last bookmarks update: {0}", _directEve.LastBookmarksUpdate);
            _directEve.RefreshBookmarks();
        }

        private void ListBookmarksTest()
        {
            _state = TestState.Idle;

            Log("Last bookmarks update: {0}", _directEve.LastBookmarksUpdate);
            for (var i = 0; i < _directEve.BookmarkFolders.Count; i++)
            {
                var folder = _directEve.BookmarkFolders[i];
                Log("BookmarkFolder[{0}].Id: {1}", i, folder.Id);
                Log("BookmarkFolder[{0}].Name: {1}", i, folder.Name);
                Log("BookmarkFolder[{0}].OwnerId: {1}", i, folder.OwnerId);
                Log("BookmarkFolder[{0}].CreatorId: {1}", i, folder.CreatorId);
            }    
                
            for (var i = 0; i < _directEve.Bookmarks.Count; i++)
            {
                var bookmark = _directEve.Bookmarks[i];
                Log("Bookmark[{0}].BookmarkId: {1}", i, bookmark.BookmarkId);

                Log("Bookmark[{0}].Title: {1}", i, bookmark.Title);
                Log("Bookmark[{0}].Memo: {1}", i, bookmark.Memo);
                Log("Bookmark[{0}].Note: {1}", i, bookmark.Note);

                Log("Bookmark[{0}].CreatedOn: {1}", i, bookmark.CreatedOn);
                Log("Bookmark[{0}].LocationId: {1}", i, bookmark.LocationId);

                Log("Bookmark[{0}].ItemId: {1}", i, bookmark.ItemId);
                Log("Bookmark[{0}].OwnerId: {1}", i, bookmark.OwnerId);
                Log("Bookmark[{0}].TypeId: {1}", i, bookmark.TypeId);
                Log("Bookmark[{0}].TypeName: {1}", i, bookmark.TypeName);

                Log("Bookmark[{0}].X: {1}", i, bookmark.X);
                Log("Bookmark[{0}].Y: {1}", i, bookmark.Y);
                Log("Bookmark[{0}].Z: {1}", i, bookmark.Z);

                LogEntity("Bookmark[" + i + "].Entity.{0}: {1}", bookmark.Entity);
            }
        }

        private void RecallDronesTest()
        {
            _state = TestState.Idle;

            if (!_directEve.Session.IsInSpace)
            {
                Log("Can't perform RecallDronesTest, not in space");
                return;
            }

            if (_directEve.ActiveDrones.Count == 0)
            {
                Log("Can't recall drones, no drones in space");
                return;
            }

            _directEve.ExecuteCommand(DirectCmd.CmdDronesReturnToBay);
        }

        private void LaunchDronesTest()
        {
            _state = TestState.Idle;

            if (!_directEve.Session.IsInSpace)
            {
                Log("Can't perform LaunchDronesTest, not in space");
                return;
            }

            var count = 0;
            var drones = _directEve.GetShipsDroneBay().Items;
            foreach (var drone in drones)
            {
                LogItem("Drone[" + count + "].{0}: {1}", drone);
                count++;
            }
            
            if (count == 0)
            {
                Log("Can't launch drones, no drones in ship");
                return;
            }

            _directEve.ActiveShip.LaunchDrones(drones);
        }

        private void ActiveShipTests()
        {
            Log("--- ACTIVESHIP TESTS ---");
            Log("ActiveShip.Shield: {0}", _directEve.ActiveShip.Shield);
            Log("ActiveShip.MaxShield: {0}", _directEve.ActiveShip.MaxShield);
            Log("ActiveShip.ShieldPercentage: {0}", _directEve.ActiveShip.ShieldPercentage);
            Log("ActiveShip.Armor: {0}", _directEve.ActiveShip.Armor);
            Log("ActiveShip.MaxArmor: {0}", _directEve.ActiveShip.MaxArmor);
            Log("ActiveShip.ArmorPercentage: {0}", _directEve.ActiveShip.ArmorPercentage);
            Log("ActiveShip.Structure: {0}", _directEve.ActiveShip.Structure);
            Log("ActiveShip.MaxStructure: {0}", _directEve.ActiveShip.MaxStructure);
            Log("ActiveShip.StructurePercentage: {0}", _directEve.ActiveShip.StructurePercentage);
            Log("ActiveShip.Capacitor: {0}", _directEve.ActiveShip.Capacitor);
            Log("ActiveShip.MaxCapacitor: {0}", _directEve.ActiveShip.MaxCapacitor);
            Log("ActiveShip.CapacitorPercentage: {0}", _directEve.ActiveShip.CapacitorPercentage);
            LogEntity("ActiveShip.Entity.{0}: {1}", _directEve.ActiveShip.Entity);

            _state = TestState.Idle;
        }

        private void MeTests()
        {
            Log("--- ME TESTS ---");
            Log("Me.MaxActiveDrones: {0}", _directEve.Me.MaxActiveDrones);
            Log("Me.MaxLockedTargets: {0}", _directEve.Me.MaxLockedTargets);
            Log("Me.Name: {0}", _directEve.Me.Name);
            Log("Me.Wealth: {0}", _directEve.Me.Wealth);

            _state = TestState.Idle;
        }

        private void SessionTests()
        {
            Log("--- SESSION TESTS ---");
            Log("Session.Now: {0}", _directEve.Session.Now);

            Log("Session.Character.Name: {0}", _directEve.Session.Character.Name);
            Log("Session.Character.OwnerId: {0}", _directEve.Session.Character.OwnerId);
            Log("Session.Character.TypeId: {0}", _directEve.Session.Character.TypeId);
            Log("Session.Character.TypeName: {0}", _directEve.Session.Character.TypeName);

            Log("Session.CharacterId: {0}", _directEve.Session.CharacterId);
            Log("Session.CorporationId: {0}", _directEve.Session.CorporationId);
            Log("Session.ShipId: {0}", _directEve.Session.ShipId);

            Log("Session.RegionId: {0}", _directEve.Session.RegionId);
            Log("Session.ConstellationId: {0}", _directEve.Session.ConstellationId);
            Log("Session.SolarSystemId: {0}", _directEve.Session.SolarSystemId);
            Log("Session.LocationId: {0}", _directEve.Session.LocationId);
            Log("Session.StationId: {0}", _directEve.Session.StationId);

            Log("Session.IsInSpace: {0}", _directEve.Session.IsInSpace);
            Log("Session.IsInStation: {0}", _directEve.Session.IsInStation);
            Log("Session.IsReady: {0}", _directEve.Session.IsReady);

            _state = TestState.Idle;
        }

        private void LogItem(string format, DirectItem item)
        {
            Log(format, "ItemId", item.ItemId);
            Log(format, "TypeId", item.TypeId);

            Log(format, "OwnerId", item.OwnerId);
            Log(format, "LocationId", item.LocationId);

            Log(format, "Quantity", item.Quantity);
            Log(format, "Stacksize", item.Stacksize);

            Log(format, "TypeName", item.TypeName);
            Log(format, "GivenName", item.GivenName);
        }

        private void LogEntity(string format, DirectEntity entity)
        {
            if (entity == null)
                return;

            Log(format, "Id", entity.Id);
            Log(format, "OwnerId", entity.OwnerId);
            Log(format, "CorpId", entity.CorpId);
            Log(format, "AllianceId", entity.AllianceId);

            Log(format, "FollowId", entity.FollowId);

            Log(format, "IsNpc", entity.IsNpc);
            Log(format, "IsPc", entity.IsPc);

            Log(format, "TypeId", entity.TypeId);
            Log(format, "TypeName", entity.TypeName);
            Log(format, "Name", entity.Name);
            Log(format, "GivenName", entity.GivenName);

            Log(format, "Distance", entity.Distance);
            Log(format, "Velocity", entity.Velocity);

            Log(format, "IsAttacking", entity.IsAttacking);
            Log(format, "IsCloaked", entity.IsCloaked);
            Log(format, "IsNeutralizingMe", entity.IsNeutralizingMe);
            Log(format, "IsJammingMe", entity.IsJammingMe);
            Log(format, "IsWebbingMe", entity.IsWebbingMe);
            Log(format, "IsSensorDampeningMe", entity.IsSensorDampeningMe);

            _state = TestState.Idle;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _directEve.Dispose();
            _directEve = null;
        }

        private void ExecuteTest_Click(object sender, EventArgs e)
        {
            _state = (TestState) TestStatesComboBox.SelectedIndex;
            if (TestStatesComboBox.SelectedIndex < TestStatesComboBox.Items.Count - 1)
                TestStatesComboBox.SelectedIndex++;
        }
    }
}
