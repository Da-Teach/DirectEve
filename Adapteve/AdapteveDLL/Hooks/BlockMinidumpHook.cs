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

    public class BlockMinidumpHook : IDisposable
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
        private delegate bool MiniDumpWriteDumpDelegate(IntPtr lpModuleName, IntPtr processId, IntPtr hFile, IntPtr dumpType, IntPtr exceptionParam, IntPtr userStreamParam, IntPtr callbackParam);

        private string _name;

        private LocalHook _hook;

        public BlockMinidumpHook(IntPtr address)
        {
            _name = string.Format("MiniDumpWriteDumpHook_{0:X}", address.ToInt32());
            _hook = LocalHook.Create(address, new MiniDumpWriteDumpDelegate(MiniDumpWriteDumpDetour), this);
            _hook.ThreadACL.SetExclusiveACL(new Int32[] {0});
        }

        private bool MiniDumpWriteDumpDetour(IntPtr lpModuleName, IntPtr processId, IntPtr hFile, IntPtr dumpType, IntPtr exceptionParam, IntPtr userStreamParam, IntPtr callbackParam)
        {
            Utility.SetLastError(8);
            return false;
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