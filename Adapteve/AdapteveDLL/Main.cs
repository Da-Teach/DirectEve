// ------------------------------------------------------------------------------
//   <copyright from='2010' to='2015' company='THEHACKERWITHIN.COM'>
//     Copyright (c) TheHackerWithin.COM. All Rights Reserved.
// 
//     Please look in the accompanying license.htm file for the license that 
//     applies to this source code. (a copy can also be found at: 
//     http://www.thehackerwithin.com/license.htm)
//   </copyright>
// -------------------------------------------------------------------------------

namespace AdapteveDLL
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using EasyHook;

    public class Main : IEntryPoint
    {
        public Main(RemoteHooking.IContext InContext, string channelname, string iniFile, string qDll, string qParam)
        {
        }

        private Settings settings;

        public void Run(RemoteHooking.IContext InContext, string channelname, string iniFile, string qDll, string qParam)
        {
            settings = new Settings(iniFile);
            InitializeHooks();
            Environment.SetEnvironment(settings);
            RemoteHooking.WakeUpProcess();

            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        public static List<IDisposable> _hooks = new List<IDisposable>();

        public void InitializeHooks()
        {
            //Load unloaded DLL's so we can have them hooked before the eve process loads them
            Utility.LoadLibrary("blue.dll");
            Utility.LoadLibrary("python27.dll");
            Utility.LoadLibrary("WS2_32.dll");
            Utility.LoadLibrary("kernel32.dll");
            Utility.LoadLibrary("advapi32.dll");
            Utility.LoadLibrary("Iphlpapi.dll");
            Utility.LoadLibrary("dbghelp.dll");
            Utility.LoadLibrary("_ctypes.pyd");
            Utility.LoadLibrary("d3d11.dll");

            _hooks.Add(new MemoryHook(LocalHook.GetProcAddress("kernel32.dll", "GlobalMemoryStatusEx"), settings.TotalPhysRam));
            _hooks.Add(new AppdataHook(LocalHook.GetProcAddress("shell32.dll", "SHGetFolderPathW"), settings.WindowsUserLogin));
            _hooks.Add(new RegistryHook(LocalHook.GetProcAddress("advapi32.dll", "RegQueryValueExA"), settings.WindowsKey));
            _hooks.Add(new NetworkAdapterHook(LocalHook.GetProcAddress("Iphlpapi.dll", "GetAdaptersInfo"), settings.NetworkAdapterGuid, settings.MacAddress, settings.NetworkAddress));
            _hooks.Add(new BlockMinidumpHook(LocalHook.GetProcAddress("dbghelp.dll", "MiniDumpWriteDump")));
            _hooks.Add(new HideProcessHook(LocalHook.GetProcAddress("kernel32.dll", "K32EnumProcesses")));
            _hooks.Add(new GraphicsCardHook(LocalHook.GetProcAddress("d3d11.dll", "D3D11CreateDevice"), settings));
            _hooks.Add(new CryptHashDataHook(LocalHook.GetProcAddress("advapi32.dll", "CryptHashData")));

            //HookImports("_ctypes.pyd");
            HookImports("blue.dll");
            HookImports("exefile.exe");
            //HookImports("psapi.dll");
        }

        public void HookImports(string module)
        {
            var address = Utility.GetImportAddress(module, "shell32.dll", "SHGetFolderPathW");
            if (address != null && address != IntPtr.Zero)
                _hooks.Add(new AppdataHook(address, settings.WindowsUserLogin));

            address = Utility.GetImportAddress(module, "advapi32.dll", "RegQueryValueExA");
            if (address != null && address != IntPtr.Zero)
                _hooks.Add(new RegistryHook(address, settings.WindowsKey));
        }
    }
}