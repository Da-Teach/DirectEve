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
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using EasyHook;

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
            _hook.ThreadACL.SetExclusiveACL(new Int32[] {0});
        }

        private bool EnumProcessDetour([In] [Out] IntPtr processIds, IntPtr arraySizeBytes, [In] [Out] IntPtr bytesCopied)
        {
            if (processIds == IntPtr.Zero || bytesCopied == IntPtr.Zero)
                return false;

            Marshal.WriteInt32(processIds, Process.GetCurrentProcess().Id);
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