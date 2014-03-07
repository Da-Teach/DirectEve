using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using EasyHook;

namespace AdapteveDLL
{
    public class RegistryHook : IDisposable
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
        private delegate int RegQueryValueExADelegate(UIntPtr hKey, IntPtr lpValueName, int lpReserved, IntPtr lpType, IntPtr lpData, IntPtr lpcbData);

        private string _name;
        private LocalHook _hook;

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int RegQueryValueExA(UIntPtr hKey, IntPtr lpValueName, int lpReserved, IntPtr lpType, IntPtr lpData, IntPtr lpcbData);

        private string _prodKey;
        public RegistryHook(IntPtr address, string prodKey)
        {
            _prodKey = prodKey;

            _name = string.Format("RegQueryValueExAHook_{0:X}", address.ToInt32());
            _hook = LocalHook.Create(address, new RegQueryValueExADelegate(RegQueryValueExADetour), this);
            _hook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });
        }

        private int RegQueryValueExADetour(UIntPtr hKey, IntPtr lpValueName, int lpReserved, IntPtr lpType, IntPtr lpData, IntPtr lpcbData)
        {
            var result = RegQueryValueExA(hKey, lpValueName, lpReserved, lpType, lpData, lpcbData);

            var keyValue = Marshal.PtrToStringAnsi(lpValueName);
            var lpDataString = Marshal.PtrToStringAnsi(lpData);
            if (keyValue == "ProductId")
            {
                var returnValue = Marshal.PtrToStringAnsi(lpData);
                IntPtr newValue = IntPtr.Zero;
                try
                {
                    newValue = Marshal.StringToHGlobalAnsi(_prodKey);
                    var size = Marshal.ReadInt32(lpcbData);
                    Utility.CopyMemory(lpData, newValue, (uint)size);
                }
                finally
                {
                    if (newValue != IntPtr.Zero)
                        Marshal.FreeHGlobal(newValue);
                }
            }
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