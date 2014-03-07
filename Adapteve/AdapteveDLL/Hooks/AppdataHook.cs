using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using EasyHook;
using System.IO;

namespace AdapteveDLL
{
    public class AppdataHook : IDisposable
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int SHGetFolderPathDelegate(IntPtr hwndOwner, int nFolder, IntPtr hToken, uint dwFlags, [In][Out] IntPtr pszPath);

        private string _name;
        private LocalHook _hook;

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern int SHGetFolderPathW(IntPtr hwndOwner, int nFolder, IntPtr hToken, uint dwFlags, IntPtr pszPath);

        private string _userLogin;
        public AppdataHook(IntPtr address, string userLogin)
        {
            this._userLogin = userLogin;            

            _name = string.Format("LibraryCallHook_{0:X}", address.ToInt32());
            _hook = LocalHook.Create(address, new SHGetFolderPathDelegate(SHGetFolderPathDetour), this);
            _hook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });
        }

        private int SHGetFolderPathDetour(IntPtr hwndOwner, int nFolder, IntPtr hToken, uint dwFlags, [In][Out] IntPtr pszPath)
        {
            var result = SHGetFolderPathW(hwndOwner, nFolder, hToken, dwFlags, pszPath);

            var tekst = Marshal.PtrToStringUni(pszPath);

            var path = "";
            if (nFolder == 20)
                path = "C:\\Windows\\Fonts";
            else if (nFolder == 35)
                path = "C:\\ProgramData";
            else if (nFolder == 5)
                path = "C:\\Users\\" + _userLogin + "\\Documents";

            else if (nFolder == 26)
                path = "C:\\Users\\" + _userLogin + "\\AppData\\Roaming";
            else
                path = "C:\\Users\\" + _userLogin + "\\AppData\\Local";

            IntPtr newString = IntPtr.Zero;
            try
            {
                newString = Marshal.StringToHGlobalUni(path);
                Utility.CopyMemory(pszPath, newString, 260);
            }
            finally
            {
                if (newString != IntPtr.Zero)
                    Marshal.FreeHGlobal(newString);
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
