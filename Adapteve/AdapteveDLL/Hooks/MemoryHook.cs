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
    using System.Runtime.InteropServices;
    using EasyHook;

    public class MemoryHook : IDisposable
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate bool GlobalMemoryStatusDelegate(IntPtr memStruct);

        private string _name;
        private LocalHook _hook;

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GlobalMemoryStatusEx(IntPtr memStruct);

        public MemoryHook(IntPtr address, ulong totalPhys)
        {
            _totalPhys = totalPhys;

            _name = string.Format("MemoryHook{0:X}", address.ToInt32());
            _hook = LocalHook.Create(address, new GlobalMemoryStatusDelegate(GlobalMemoryStatusDetour), this);
            _hook.ThreadACL.SetExclusiveACL(new Int32[] {0});
        }

        private ulong _totalPhys;
        private MEMORYSTATUSEX _struct;

        private bool GlobalMemoryStatusDetour(IntPtr memStruct)
        {
            ////Prevents eve crashes
            if (_struct == null)
            {
                var result = GlobalMemoryStatusEx(memStruct);
                _struct = (MEMORYSTATUSEX) Marshal.PtrToStructure(memStruct, typeof (MEMORYSTATUSEX));
                _struct.ullTotalPhys = _totalPhys*1024*1024;
            }

            Marshal.StructureToPtr(_struct, memStruct, true);
            return true;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }

        public void Dispose()
        {
            if (_hook == null)
                return;

            _hook.Dispose();
            _hook = null;
        }
    }
}