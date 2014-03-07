using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using EasyHook;

namespace AdapteveDLL
{
    public class HideProcessHook : IDisposable
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
        private delegate bool EnumProcessesDelegate([In] [Out] IntPtr processIds, IntPtr arraySizeBytes, [In] [Out] IntPtr bytesCopied);

        private string _name;
        private LocalHook _hook;

        public HideProcessHook(IntPtr address)
        {
            _name = string.Format("EnumProcessesHook_{0:X}", address.ToInt32());
            _hook = LocalHook.Create(address, new EnumProcessesDelegate(EnumProcessDetour), this);
            _hook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });
        }

        private bool EnumProcessDetour([In] [Out] IntPtr processIds, IntPtr arraySizeBytes, [In] [Out] IntPtr bytesCopied)
        {
            if (processIds == IntPtr.Zero || bytesCopied == IntPtr.Zero)
                return false;

            Marshal.WriteInt32(processIds, System.Diagnostics.Process.GetCurrentProcess().Id);
            Marshal.WriteInt32(bytesCopied, Marshal.SizeOf(processIds));
            return true;
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
