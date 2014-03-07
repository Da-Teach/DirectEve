using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using EasyHook;

namespace AdapteveDLL
{
    public class MemoryHook : IDisposable
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate bool GlobalMemoryStatusDelegate(IntPtr memStruct);

        private string _name;
        private LocalHook _hook;

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        public static extern bool GlobalMemoryStatusEx(IntPtr memStruct);

        public MemoryHook(IntPtr address, ulong totalPhys)
        {
            this._totalPhys = totalPhys;

            _name = string.Format("MemoryHook{0:X}", address.ToInt32());
            _hook = LocalHook.Create(address, new GlobalMemoryStatusDelegate(GlobalMemoryStatusDetour), this);
            _hook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });
        }

        private ulong _totalPhys;
        private bool GlobalMemoryStatusDetour(IntPtr memStruct)
        {
            var result = GlobalMemoryStatusEx(memStruct);
            MEMORYSTATUSEX structure = (MEMORYSTATUSEX)Marshal.PtrToStructure(memStruct, typeof(MEMORYSTATUSEX));
            structure.ullTotalPhys = _totalPhys * 1024 * 1024;
            Marshal.StructureToPtr(structure, memStruct, true);
            return result;
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