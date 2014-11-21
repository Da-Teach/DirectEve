// ------------------------------------------------------------------------------
//   <copyright from='2010' to='2015' company='THEHACKERWITHIN.COM'>
//     Copyright (c) TheHackerWithin.COM. All Rights Reserved.
// 
//     Please look in the accompanying license.htm file for the license that 
//     applies to this source code. (a copy can also be found at: 
//     http://www.thehackerwithin.com/license.htm)
//   </copyright>
// -------------------------------------------------------------------------------

namespace AphackInject
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Aphack;
    using EasyHook;

    public class Main : IEntryPoint
    {
        private AphackInterface Interface;
        private LocalHook CreateKeywordHook;
        private LocalHook CreateGetModuleHandleAHook;
        private Stack<String> Queue = new Stack<String>();

        public Main(
            RemoteHooking.IContext InContext,
            String InChannelName)
        {
            // connect to host...
            Interface = RemoteHooking.IpcConnectClient<AphackInterface>(InChannelName);

            Interface.Ping();
        }

        public void Run(
            RemoteHooking.IContext InContext,
            String InChannelName)
        {
            // install hook...
            try
            {
                CreateKeywordHook = LocalHook.Create(
                    LocalHook.GetProcAddress("python27.dll", "PyEval_CallObjectWithKeywords"),
                    new DCallKeywords(CallKeywords_Hooked),
                    this);

                CreateKeywordHook.ThreadACL.SetExclusiveACL(new Int32[] {0});

                CreateGetModuleHandleAHook = LocalHook.Create(LocalHook.GetProcAddress("kernel32", "GetModuleHandleA"), new DGetModuleHandleA(GetModuleHandleHooked), this);

                CreateGetModuleHandleAHook.ThreadACL.SetExclusiveACL(new Int32[] {0});
            }
            catch (Exception ExtInfo)
            {
                Interface.ReportException(ExtInfo);

                return;
            }

            Interface.IsInstalled(RemoteHooking.GetCurrentProcessId());

            RemoteHooking.WakeUpProcess();

            // wait for host process termination...
            try
            {
                while (true)
                {
                    Thread.Sleep(500);

                    // transmit newly monitored file accesses...
                    if (Queue.Count > 0)
                    {
                        String[] Package = null;

                        lock (Queue)
                        {
                            Package = Queue.ToArray();

                            Queue.Clear();
                        }
                    }
                    else
                        Interface.Ping();
                }
            }
            catch
            {
                // Ping() will raise an exception if host is unreachable
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr DCallKeywords(IntPtr op, IntPtr args, IntPtr kw);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
        private delegate IntPtr DGetModuleHandleA(IntPtr lpModuleName);

        // just use a P-Invoke implementation to get native API access from C# (this step is not necessary for C++.NET)
        [DllImport("python27.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr PyEval_CallObjectWithKeywords(IntPtr op, IntPtr args, IntPtr kw);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        public static extern IntPtr GetModuleHandleA(IntPtr lpModuleName);

        // this is where we are intercepting all file accesses!
        private static bool foundpos = false;

        private static IntPtr CallKeywords_Hooked(IntPtr op, IntPtr args, IntPtr kw)
        {
            Debugger.Launch();

            var pyOp = new PyObject(op);
            var pyArgs = new PyObject(args);
            if (pyArgs.Type == Py.PyType.TupleType)
            {
                if (pyArgs.Size > 0)
                {
                    var i = 0;
                    foreach (var item in pyArgs.Tuple)
                    {
                        if (item.String != null && item.String.ToString().ToLower().Contains("cmdwarptostuffautopilot"))
                        {
                            foundpos = true;
                        }
                        else if (foundpos && item.Type == Py.PyType.LongType)
                        {
                            foundpos = false;
                            var dest = Py.PyTuple_GetItem(args, i);
                            var call = Py.PyObject_GetAttrString(Py.PyDict_GetItem(Py.PyObject_GetAttrString(Py.PyObject_GetAttrString(Py.PyImport_ImportModule("__builtin__"), "sm"), "services"), Py.PyString_FromString("menu")), "WarpToItem");
                            var param = Py.Py_BuildValue("(" + "O" + ")", dest);

                            //Appevent
                            var appcall = Py.PyObject_GetAttrString(Py.PyObject_GetAttrString(Py.PyObject_GetAttrString(Py.PyImport_ImportModule("__builtin__"), "uicore"), "uilib"), "RegisterAppEventTime");
                            PyEval_CallObjectWithKeywords(appcall, Py.Py_BuildValue("()"), IntPtr.Zero);


                            var result2 = PyEval_CallObjectWithKeywords(call, param, IntPtr.Zero);

                            return result2;
                        }
                        i++;
                    }
                }
            }

            var result = PyEval_CallObjectWithKeywords(op, args, kw);
            return result;
        }

        private static IntPtr GetModuleHandleHooked(IntPtr lpModuleName)
        {
            if (lpModuleName != IntPtr.Zero)
            {
                var fileName = Marshal.PtrToStringAnsi(lpModuleName);
                if (fileName.ToLower().Contains("easyhook") || fileName.ToLower().Contains("aphack"))
                {
                    var trash = Marshal.StringToHGlobalAnsi("ajhartyjshsg.dll");
                    return GetModuleHandleA(trash);
                }
            }
            var result = GetModuleHandleA(lpModuleName);
            return result;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        private delegate IntPtr DPyEval_CallObjectWithKeywords(IntPtr op, IntPtr args, IntPtr kw);
    }
}