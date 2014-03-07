using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using EasyHook;


namespace AdapteveDLL
{
    public class GetAdapterHook : IDisposable
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        private delegate uint GetAdapterDelegate(IntPtr pAdapter);

        private string _name;
        private LocalHook _hook;

        private Settings _settings;
        public GetAdapterHook(IntPtr address, Settings settings)
        {
            _settings = settings;

            _name = string.Format("GetAdaptersInfoHook_{0:X}", address.ToInt32());
            _hook = LocalHook.Create(address, new GetAdapterDelegate(GetAdapterDetour), this);
            _hook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });
        }

        private uint GetAdapterDetour(IntPtr pAdapter)
        {
            return 1;
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
