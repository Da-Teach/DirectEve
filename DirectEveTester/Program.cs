// ------------------------------------------------------------------------------
//   <copyright from='2010' to='2015' company='THEHACKERWITHIN.COM'>
//     Copyright (c) TheHackerWithin.COM. All Rights Reserved.
// 
//     Please look in the accompanying license.htm file for the license that 
//     applies to this source code. (a copy can also be found at: 
//     http://www.thehackerwithin.com/license.htm)
//   </copyright>
// -------------------------------------------------------------------------------
namespace DirectEveTester
{
    using System;
    using System.Linq;
    using System.Threading;
    using DirectEve;
    using InnerSpaceAPI;

    internal static class Program
    {
        private static bool _done;
        private static DirectEve _directEve;
        private static long _frameCount = 0;

        /// <summary>
        ///   The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Log("Starting test...");
            _directEve = new DirectEve();
            _directEve.OnFrame += OnFrame;

            // Sleep until we're done
            while (!_done)
                Thread.Sleep(50);

            _directEve.Dispose();
            Log("Test finished.");
        }

        private static void Log(string line)
        {
            InnerSpace.Echo(string.Format("{0:D} {1:HH:mm:ss} {2}", _frameCount, DateTime.Now, line));
        }

        private static void OnFrame(object sender, EventArgs eventArgs)
        {
            _frameCount++;

            if (_done)
                return;

            try
            {
                //_done = true;

                var scanner = _directEve.Windows.OfType<DirectScannerWindow>().FirstOrDefault();
                if (scanner == null)
                {
                    _directEve.ExecuteCommand(DirectCmd.OpenScanner);
                    return;
                }

                if (!scanner.IsReady)
                {
                    // Wait for it to become ready
                    return;
                }

                //if (_frameCount % 8 == 7)
                //{
                //    scanner.NextTab();
                //}
                //if (_frameCount % 8 == 7)
                //{
                //    scanner.PrevTab();
                //}
                //if (_frameCount % 8 == 7)
                //{
                //    scanner.SelectByIdx((int)_frameCount % 3);
                //}

                if (scanner.GetSelectedIdx() != 1)
                {
                    scanner.SelectByIdx(1);  // select dscan tab
                }

                if (_frameCount > 75)
                {
                    scanner.Close();
                    _done = true;
                }

                //foreach (var fitting in fittingMgr.Fittings)
                //{
                //    Log(fitting.Name);
                //    foreach (var module in fitting.Modules)
                //    {
                //        Log(" - " + module.TypeName);
                //    }
                //}

                //if (fittingMgr.Fittings.Any())
                //    fittingMgr.Fittings.FirstOrDefault().Fit();

                //var cargo = _directEve.GetShipsCargo();
                //Log("--------- CARGO --------");
                //foreach(var item in cargo.Items)
                //{
                //    Log(item.ItemId + " - " + item.TypeName + " - " + item.FlagId + " - " + item.LocationId);
                //}

                //Log("--------- HANGAR --------");
                //var hangar = _directEve.GetItemHangar();
                //foreach (var item in hangar.Items)
                //{
                //    Log(item.ItemId + " - " + item.TypeName + " - " + item.FlagId + " - " + item.LocationId + " - " + item.Stacksize);
                //}

                //foreach(var entity in _directEve.Entities)
                //{
                //    Log(entity.Name + " " + entity.IsNpc + " " + entity.CategoryId + " " + entity.GroupId);
                //}

                //var fittingMgr = _directEve.Windows.OfType<DirectFittingManagerWindow>().FirstOrDefault();
                //if (fittingMgr == null)
                //{
                //    _directEve.OpenFitingManager();
                //    return;
                //}

                //if (!fittingMgr.IsReady)
                //{
                //    // Wait for it to become ready
                //    return;
                //}

                //foreach (var fitting in fittingMgr.Fittings)
                //{
                //    Log(fitting.Name);
                //    foreach (var module in fitting.Modules)
                //    {
                //        Log(" - " + module.TypeName);
                //    }
                //}

                //if (fittingMgr.Fittings.Any())
                //    fittingMgr.Fittings.FirstOrDefault().Fit();

                //var hangar = _directEve.GetItemHangar();

                //foreach(var item in _directEve.GetShipsModules().Items)
                //{
                //    Log(item.ItemId + " - " + item.TypeName + " - " + item.FlagId + " - " + item.LocationId);

                //    if (item.TypeName == "Cap Recharger II")
                //        hangar.Add(item);
                //}

                //foreach(var item in hangar.Items)
                //{
                //    if (item.TypeName == "Cap Recharger II")
                //        item.FitToActiveShip();
                //}

                //if ( _directEve.Windows.FirstOrDefault(w => (string)w.Type == "form.FittingMgmt") != null )
                //{
                //    Log("Swapping fit...");
                //    if ( _directEve.FitFitting("phantaztik") )
                //        _done = true;
                //}
                //else
                //{
                //    Log("Opening fitting manager...");
                //    _directEve.OpenFitingManager();
                //}

                //var hangar = _directEve.GetItemHangar();

                //foreach (var item in hangar.Items)
                //{
                //    item.FitToActiveShip();
                //}

                //Log("Logging off...");
                //_directEve.ExecuteCommand(DirectCmd.CmdLogOff);
                //Log("Done.");

                //foreach (var module in _directEve.Modules)
                //{
                //    Log("Module " + module.TypeName + " - " + module.GroupId);
                //    if (module.GroupId != 53)
                //        continue;

                //    var typeId = module.Charge != null ? module.Charge.TypeId : 0;
                //    var typeName = module.Charge != null ? module.Charge.TypeName : "";
                //    var ammo = module.MatchingAmmo.FirstOrDefault(a => a.TypeId != typeId);
                //    if (ammo == null)
                //    {
                //        Log("No matching ammo found");
                //        continue;
                //    }

                //    module.ChangeAmmo(ammo);
                //    Log("Changing ammo from " + typeName + " to " + ammo.TypeName);
                //}

                //var hangar = _directEve.GetItemHangar();
                //var cargo = _directEve.GetShipsCargo();

                //foreach (var item in cargo.Items)
                //{
                //    if (item.ItemId == _directEve.Session.ShipId)
                //        continue;

                //    hangar.Add(item);
                //}

                //foreach (var module in _directEve.Modules)
                //{
                //    Log("Module " + module.Name + " attributes:");
                //    foreach (var attribute in module.Attributes.GetAttributes())
                //    {
                //        if (attribute.Value == typeof(bool))
                //            Log("   - " + attribute.Key + " = " + module.Attributes.TryGet<bool>(attribute.Key) + " (bool)");
                //        if (attribute.Value == typeof(int))
                //            Log("   - " + attribute.Key + " = " + module.Attributes.TryGet<int>(attribute.Key) + " (int)");
                //        if (attribute.Value == typeof(long))
                //            Log("   - " + attribute.Key + " = " + module.Attributes.TryGet<long>(attribute.Key) + " (long)");
                //        if (attribute.Value == typeof(double))
                //            Log("   - " + attribute.Key + " = " + module.Attributes.TryGet<double>(attribute.Key) + " (double)");
                //        if (attribute.Value == typeof(string))
                //            Log("   - " + attribute.Key + " = " + module.Attributes.TryGet<string>(attribute.Key) + " (string)");
                //    }

                //    if (module.Charge != null)
                //    {
                //        Log(" * Module charge " + module.Charge.Name + " attributes:");
                //        foreach (var attribute in module.Charge.Attributes.GetAttributes())
                //        {
                //            if (attribute.Value == typeof(bool))
                //                Log("   - " + attribute.Key + " = " + module.Charge.Attributes.TryGet<bool>(attribute.Key) + " (bool)");
                //            if (attribute.Value == typeof(int))
                //                Log("   - " + attribute.Key + " = " + module.Charge.Attributes.TryGet<int>(attribute.Key) + " (int)");
                //            if (attribute.Value == typeof(long))
                //                Log("   - " + attribute.Key + " = " + module.Charge.Attributes.TryGet<long>(attribute.Key) + " (long)");
                //            if (attribute.Value == typeof(double))
                //                Log("   - " + attribute.Key + " = " + module.Charge.Attributes.TryGet<double>(attribute.Key) + " (double)");
                //            if (attribute.Value == typeof(string))
                //                Log("   - " + attribute.Key + " = " + module.Charge.Attributes.TryGet<string>(attribute.Key) + " (string)");
                //        }
                //    }
                //}

                //Log("Shield - " + _directEve.ActiveShip.Shield + " - " + _directEve.ActiveShip.ShieldPercentage);

                //var local = _directEve.Windows.OfType<DirectChatWindow>().FirstOrDefault();
                //if (local != null)
                //{
                //    foreach(var member in local.Members)
                //        Log(member.CharacterId + ": " + member.Name);

                //    foreach(var message in local.Messages)
                //        Log(message.Name + ": " + message.Message);
                //}

                //Logging.Log(_directEve.ActiveShip.TypeName);

                //foreach (var entity in _directEve.Entities)
                //    Logging.Log(entity.Id + " - " + entity.Name);

                //var gate = _directEve.Entities.FirstOrDefault(entity => !string.IsNullOrEmpty(entity.Name) && entity.Name.StartsWith("Stargate") && entity.Distance < 2500);
                //if (gate != null)
                //{
                //    Logging.Log("Jumping: " + gate.Name);
                //    gate.Jump();
                //}

                //foreach(var slot in _directEve.Login.CharacterSlots)
                //    Logging.Log(slot.CharName);

                //foreach (var jammer in _directEve.Entities.SelectMany(entity => entity.ElectronicWarfare).Distinct())
                //    Logging.Log(jammer);

                //foreach (var module in _directEve.Modules)
                //{
                //    Logging.Log(module.ItemId + " - " + module.Name + "[" + module.TypeId + "]");
                //    if (module.TypeId != 3057)
                //        continue;

                //    var chargeTypeId = module.Charge == null ? 0 : module.Charge.TypeId;
                //    var otherAmmo = module.MatchingAmmo.FirstOrDefault(e => e.TypeId != chargeTypeId && e.Quantity == -1);
                //    if (otherAmmo == null)
                //        continue;

                //    Logging.Log("Changing ammo from " + chargeTypeId + " to " + otherAmmo.TypeId);
                //    module.ChangeAmmo(otherAmmo);
                //}

                //var cargo = _directEve.GetShipsCargo();
                //var metal = cargo.Items.FirstOrDefault(i => i.Name == "Metal Scraps");
                //cargo.Jettison(metal.ItemId);
            }
            catch
            {
                Log("Caught exception!!");
            }
            //finally
            //{
            //    _done = true;
            //}
        }
    }
}