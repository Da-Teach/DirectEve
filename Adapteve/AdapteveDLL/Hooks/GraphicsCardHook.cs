using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using EasyHook;

namespace AdapteveDLL
{
    public class GraphicsCardHook : IDisposable
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate UInt32 D3D11CreateDeviceDelegate(IntPtr adapter, IntPtr driverType, IntPtr software, uint flags, IntPtr pfeatureLevels, uint featureLevels, uint sdkVersion, [Out]IntPtr device, IntPtr pfeatureLevel, IntPtr context);

        [DllImport("d3d11.dll", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern uint D3D11CreateDevice(IntPtr adapter, IntPtr driverType, IntPtr software, uint flags, IntPtr pfeatureLevels, uint featureLevels, uint sdkVersion, [Out]IntPtr device, IntPtr pfeatureLevel, IntPtr context);

        [DllImport("dxgi.dll", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern uint CreateDXGIFactory(string factory, IntPtr ppFactory);

        private string _name;
        private LocalHook _hook;
        private List<IDisposable> _adapterHooks = new List<IDisposable>();

        private Settings _settings;
        public GraphicsCardHook(IntPtr address, Settings settings)
        {
            this._settings = settings;

            _name = string.Format("LibraryCallHook_{0:X}", address.ToInt32());
            _hook = LocalHook.Create(address, new D3D11CreateDeviceDelegate(D3D11CreateDeviceDetour), this);
            _hook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });
        }

        
        private UInt32 D3D11CreateDeviceDetour(IntPtr adapter, IntPtr driverType, IntPtr software, uint flags, IntPtr pfeatureLevels, uint featureLevels, uint sdkVersion, [Out] IntPtr ppDevice, IntPtr pfeatureLevel, IntPtr context)
        {
            var result = D3D11CreateDevice(adapter, driverType, software, flags, pfeatureLevels, featureLevels, sdkVersion, ppDevice, pfeatureLevel, context);

            IntPtr getAdapterPtr = Marshal.ReadIntPtr(Marshal.ReadIntPtr(ppDevice),28); //VTable index 7

            return result;
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
