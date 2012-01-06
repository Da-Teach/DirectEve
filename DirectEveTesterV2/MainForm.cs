namespace DirectEveTesterV2
{
    using System;
    using System.Reflection;
    using System.Linq;
    using DirectEve;
    using System.Windows.Forms;
    using InnerSpaceAPI;

    public partial class MainForm : Form
    {
        [AttributeUsage(AttributeTargets.All)]
        public class Test : System.Attribute
        {
            public readonly string _desc;

            public Test(string desc)
            {
                this._desc = desc;
            }

            public Test()
            {
                this._desc = null;
            }
        }

        private DirectEve _directEve;
        private TestState _state;
        private string _activeTest;

        private enum TestState
        {
            Idle,
            RunTest,
            Done
        }

        public MainForm()
        {
            InitializeComponent();

            Type t = typeof(MainForm);
            foreach (MethodInfo mi in t.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                object[] attrs = mi.GetCustomAttributes(typeof(Test),false);
                foreach (object attr in attrs)
                {
                    TestStatesComboBox.Items.Add(mi.Name);
                }
            }
            _activeTest = "";
            TestStatesComboBox.SelectedIndex = 0;

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
                    case TestState.Idle:
                        lblStatus.Text = "Idle";
                        break;

                    case TestState.RunTest:
                        RunSelectedTest();
                        break;
                }
            }
            catch (Exception ex)
            {
                Log("Exception: {0}", ex);
            }
        }

        private void RunSelectedTest()
        {
            if (!String.IsNullOrEmpty(_activeTest))
            {
                Type t = typeof(MainForm);
                MethodInfo mi = t.GetMethod(_activeTest, BindingFlags.NonPublic | BindingFlags.Instance);
                object[] attrs = mi.GetCustomAttributes(typeof(Test),false);
                foreach (object attr in attrs)
                {
                    Test ta = (Test)attr;
                    if (String.IsNullOrEmpty(ta._desc))
                    {
                        Log("Running {0} test...", mi.Name);
                    }
                    else
                    {
                        Log("Running {0} test...", ta._desc);
                    }
                }
                mi.Invoke(this, null);
                _activeTest = null;
            }
        }

        [Test("Ship's cargo")]
        private void ListShipsCargoTest()
        {
            _state = TestState.Idle;

            var cargo = _directEve.GetShipsCargo();
            if (!cargo.IsReady)
            {
                Log("Your ship's cargo is not ready!");
                return;
            }

            for (int i = 0; i < cargo.Items.Count; i++)
            {
                var item = cargo.Items[i];
                LogItem("cargo[" + i + "].{0}: {1}", item);
            }
        }

        [Test("Warp to bookmark")]
        private void WarpToBookmarkTest()
        {
            _state = TestState.Idle;

            if (!_directEve.Session.IsInSpace)
            {
                Log("Your not in space, can't warp to a bookmark!");
                return;
            }

            var bookmark = _directEve.Bookmarks.FirstOrDefault(b => b.LocationId == _directEve.Session.SolarSystemId);
            if (bookmark == null)
            {
                Log("No bookmark found in this system!");
                return;
            }

            Log("Warping to bookmark {0}", bookmark.Title);
            bookmark.WarpTo(0);
        }

        private void DeleteBookmarkFolderTest()
        {
            _state = TestState.Idle;

            var folder = _directEve.BookmarkFolders.FirstOrDefault(f => f.Name == "Wassup Folder");
            if (folder == null)
            {
                Log("No Test Bookmark Folder to delete");
                return;
            }

            folder.Delete();
        }

        [Test("Create bookmark")]
        private void CreateBookmarkFolderTest()
        {
            _state = TestState.Idle;

            _directEve.CreateBookmarkFolder("Wassup Folder");
        }

        [Test("Delete bookmark")]
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

        [Test("Bookmark location")]
        private void BookmarkCurrentLocationTest()
        {
            _state = TestState.Idle;

            var folder = _directEve.BookmarkFolders.FirstOrDefault(f => f.Name == "Wassup Folder");

            _directEve.BookmarkCurrentLocation("Wassup", "This is the drinking bar", folder != null ? folder.Id : (long?)null);
        }

        [Test("Refresh bookmarks")]
        private void RefreshBookmarksTest()
        {
            _state = TestState.Idle;

            Log("Last bookmarks update: {0}", _directEve.LastBookmarksUpdate);
            _directEve.RefreshBookmarks();
        }

        [Test("List bookmarks")]
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

        [Test]
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

        [Test]
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

        [Test]
        private void ListEntitiesTests()
        {
            _state = TestState.Idle;

            foreach(var entity in _directEve.Entities)
                LogEntity("Entity[" + entity.Id + "].{0}: {1}", entity);
        }

        [Test]
        private void ActiveShipTests()
        {
            _state = TestState.Idle;

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
        }

        [Test]
        private void ListWindowsTest()
        {
            _state = TestState.Idle;

            for (int i = 0; i < _directEve.Windows.Count; i++)
            {
                var window = _directEve.Windows[i];
                Log("Window[" + i + "].Id: {0}", window.Id);
                Log("Window[" + i + "].Type: {0}", window.Type);
                Log("Window[" + i + "].Caption: {0}", window.Caption);
                Log("Window[" + i + "].Name: {0}", window.Name);
                Log("Window[" + i + "].ClassName: {0}", window.GetType().Name);

                if (window is DirectContainerWindow)
                {
                    var cargoWindow = window as DirectContainerWindow;
                    Log("Window[" + i + "].IsReady: {0}", cargoWindow.IsReady);
                }
            }
        }

        [Test]
        private void MeTests()
        {
            _state = TestState.Idle;

            Log("Me.MaxActiveDrones: {0}", _directEve.Me.MaxActiveDrones);
            Log("Me.MaxLockedTargets: {0}", _directEve.Me.MaxLockedTargets);
            Log("Me.Name: {0}", _directEve.Me.Name);
            Log("Me.Wealth: {0}", _directEve.Me.Wealth);
        }

        [Test]
        private void SessionTests()
        {
            _state = TestState.Idle;

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
            Log(format, "GroupId", entity.GroupId);
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
            _state = TestState.RunTest;
            _activeTest = TestStatesComboBox.Text;
            if (TestStatesComboBox.SelectedIndex < TestStatesComboBox.Items.Count - 1)
                TestStatesComboBox.SelectedIndex++;
            ExecuteTest.Enabled = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(_activeTest))
            {
                ExecuteTest.Enabled = true;
            }
            timer1.Enabled = true;
        }
    }
}
