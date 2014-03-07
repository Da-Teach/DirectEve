using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using EasyHook;

namespace AdapteveDLL
{
    public class GraphicsCardHook2 : IDisposable
    {
        [DllImport("dxgi.dll", SetLastError = true, EntryPoint = "CreateDXGIFactory", CallingConvention = CallingConvention.StdCall), System.Security.SuppressUnmanagedCodeSecurity]
        public static extern uint CreateDXGIFactory([In, MarshalAs(UnmanagedType.LPStruct)] Guid riid, [In][Out]IntPtr ppFactory);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        private delegate uint EnumAdaptersDelegate(uint index, IntPtr ppAdapter);

        private EnumAdaptersDelegate EnumAdaptersOriginal;

        private string _name;
        private LocalHook _hook;
        private List<IDisposable> _adapterHooks = new List<IDisposable>();

        private Settings _settings;
        public GraphicsCardHook2(IntPtr address, Settings settings)
        {
            this._settings = settings;

            var iid = Guid.Parse("7b7166ec-21c7-44ae-b21a-c9ae321ae369"); //iid of IDXGIFactory
            IntPtr test = Marshal.AllocHGlobal(4);
            var result = CreateDXGIFactory(iid, test);

            var enumAdapterPtr = Marshal.ReadIntPtr(Marshal.ReadIntPtr(Marshal.ReadIntPtr(test)), 28); //ppFactory --> pFactory --> Vtable --> pointer index 7 EnumAdapters?
            EnumAdaptersOriginal = (EnumAdaptersDelegate)Marshal.GetDelegateForFunctionPointer(enumAdapterPtr, typeof(EnumAdaptersDelegate));

            _name = string.Format("LibraryCallHook_{0:X}", enumAdapterPtr.ToInt32());
            _hook = LocalHook.Create(enumAdapterPtr, new EnumAdaptersDelegate(EnumAdaptersDetour), this);
            _hook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });

        }

        private uint EnumAdaptersDetour(uint index, IntPtr ppAdapter)
        {
            var result = EnumAdaptersOriginal(index, ppAdapter);
            return result;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
        public struct DXGI_ADAPTER_DESC1
        {
            /// <summary>
            /// A string that contains the adapter description.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public String Description;

            /// <summary>
            /// The PCI ID of the hardware vendor.
            /// </summary>
            public UInt32 VendorId;

            /// <summary>
            /// The PCI ID of the hardware device.
            /// </summary>
            public UInt32 DeviceId;

            /// <summary>
            /// The PCI ID of the sub system.
            /// </summary>
            public UInt32 SubSysId;

            /// <summary>
            /// The PCI ID of the revision number of the adapter.
            /// </summary>
            public UInt32 Revision;

            /// <summary>
            /// The number of bytes of dedicated video memory that are not shared with the CPU.
            /// </summary>
            public IntPtr DedicatedVideoMemory;

            /// <summary>
            /// The number of bytes of dedicated system memory that are not shared with the GPU. This memory is allocated from available system memory at boot time.
            /// </summary>
            public IntPtr DedicatedSystemMemory;

            /// <summary>
            /// The number of bytes of shared system memory. This is the maximum value of system memory that may be consumed by the adapter during operation.
            /// Any incidental memory consumed by the driver as it manages and uses video memory is additional.
            /// </summary>
            public IntPtr SharedSystemMemory;

            /// <summary>
            /// A unique value that identifies the adapter. See <see cref="LUID"/> for a definition of the structure.
            /// </summary>
            public LUID AdapterLuid;

            /// <summary>
            /// A member of the <see cref="DXGI_ADAPTER_FLAG"/> enumerated type that describes the adapter type. 
            /// The <see cref="DXGI_ADAPTER_FLAG.DXGI_ADAPTER_FLAG_REMOTE"/> flag specifies that the adapter is a remote adapter.
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
