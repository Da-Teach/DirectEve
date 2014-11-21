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

    public class GraphicsCardHook : IDisposable
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate uint GetDesc1Delegate(int index, IntPtr adapter);

        private GetDesc1Delegate GetDesc1Original;

        private string _name;
        private LocalHook _hook;
        private int getDescOffset = 0xba24;

        private Settings _settings;

        public GraphicsCardHook(IntPtr address, Settings settings)
        {
            _settings = settings;

            var dxgiHandle = Utility.GetModuleHandle("dxgi.dll");
            var getDescPtr = dxgiHandle + getDescOffset;
            GetDesc1Original = (GetDesc1Delegate) Marshal.GetDelegateForFunctionPointer(getDescPtr, typeof (GetDesc1Delegate));

            _name = string.Format("GetDescHook_{0:X}", getDescPtr.ToInt32());
            _hook = LocalHook.Create(getDescPtr, new GetDesc1Delegate(GetDesc1Detour), this);
            _hook.ThreadACL.SetExclusiveACL(new Int32[] {0});
        }

        private uint GetDesc1Detour(int index, IntPtr adapter)
        {
            var result = GetDesc1Original(index, adapter);

            if (result == 0)
            {
                var structure = (DXGI_ADAPTER_DESC1) Marshal.PtrToStructure(adapter, typeof (DXGI_ADAPTER_DESC1));
                structure.Description = _settings.GpuDescription;
                structure.DeviceId = _settings.GpuDeviceId;
                structure.Revision = _settings.GpuRevision;
                structure.VendorId = _settings.GpuVendorId;
                Marshal.StructureToPtr(structure, adapter, true);
            }
            return result;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
        public struct DXGI_ADAPTER_DESC1
        {
            /// <summary>
            ///     A string that contains the adapter description.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public String Description;

            /// <summary>
            ///     The PCI ID of the hardware vendor.
            /// </summary>
            public UInt32 VendorId;

            /// <summary>
            ///     The PCI ID of the hardware device.
            /// </summary>
            public UInt32 DeviceId;

            /// <summary>
            ///     The PCI ID of the sub system.
            /// </summary>
            public UInt32 SubSysId;

            /// <summary>
            ///     The PCI ID of the revision number of the adapter.
            /// </summary>
            public UInt32 Revision;

            /// <summary>
            ///     The number of bytes of dedicated video memory that are not shared with the CPU.
            /// </summary>
            public IntPtr DedicatedVideoMemory;

            /// <summary>
            ///     The number of bytes of dedicated system memory that are not shared with the GPU. This memory is allocated from
            ///     available system memory at boot time.
            /// </summary>
            public IntPtr DedicatedSystemMemory;

            /// <summary>
            ///     The number of bytes of shared system memory. This is the maximum value of system memory that may be consumed by the
            ///     adapter during operation.
            ///     Any incidental memory consumed by the driver as it manages and uses video memory is additional.
            /// </summary>
            public IntPtr SharedSystemMemory;

            /// <summary>
            ///     A unique value that identifies the adapter. See <see cref="LUID" /> for a definition of the structure.
            /// </summary>
            public LUID AdapterLuid;

            /// <summary>
            ///     A member of the <see cref="DXGI_ADAPTER_FLAG" /> enumerated type that describes the adapter type.
            ///     The <see cref="DXGI_ADAPTER_FLAG.DXGI_ADAPTER_FLAG_REMOTE" /> flag specifies that the adapter is a remote adapter.
            /// </summary>
            public DXGI_ADAPTER_FLAG Flags;
        }

        public struct LUID
        {
            public UInt32 LowPart;
            public Int32 HighPart;
        }

        public enum DXGI_ADAPTER_FLAG : uint
        {
            DXGI_ADAPTER_FLAG_NONE = 0,
            DXGI_ADAPTER_FLAG_REMOTE = 1,
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