using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using EasyHook;
using Aphack;

namespace AphackInject
{
    public class Main : EasyHook.IEntryPoint
    {
        Aphack.AphackInterface Interface;
        LocalHook CreateKeywordHook;
        LocalHook CreateGetModuleHandleAHook;
        Stack<String> Queue = new Stack<String>();

        public Main(
            RemoteHooking.IContext InContext,
            String InChannelName)
        {
            // connect to host...
            Interface = RemoteHooking.IpcConnectClient<Aphack.AphackInterface>(InChannelName);

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

                CreateKeywordHook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });

                CreateGetModuleHandleAHook = LocalHook.Create(LocalHook.GetProcAddress("kernel32", "GetModuleHandleA"), new DGetModuleHandleA(GetModuleHandleHooked), this);

                CreateGetModuleHandleAHook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });

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
        delegate IntPtr DCallKeywords(IntPtr op, IntPtr args, IntPtr kw);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
        delegate IntPtr DGetModuleHandleA(IntPtr lpModuleName);

        // just use a P-Invoke implementation to get native API access from C# (this step is not necessary for C++.NET)
        [DllImport("python27.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr PyEval_CallObjectWithKeywords(IntPtr op, IntPtr args, IntPtr kw);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        public static extern IntPtr GetModuleHandleA(IntPtr lpModuleName);

        // this is where we are intercepting all file accesses!
        static bool foundpos = false;
        static IntPtr CallKeywords_Hooked(IntPtr op, IntPtr args, IntPtr kw)
        {
            System.Diagnostics.Debugger.Launch();

            var pyOp = new PyObject(op);
            var pyArgs = new PyObject(args);
            if (pyArgs.Type == Py.PyType.TupleType)
            {
                if (pyArgs.Size > 0)
                {
                    int i = 0;
                    foreach (var item in pyArgs.Tuple)
                    {
                        if (item.String != null && item.String.ToString().ToLower().Contains("cmdwarptostuffautopilot"))
                        {
                            foundpos = true;
                        }
                        else if (foundpos && item.Type == Py.PyType.LongType)
                        {
                            foundpos = false;
                            IntPtr dest = Py.PyTuple_GetItem(args, i);
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

        static IntPtr GetModuleHandleHooked(IntPtr lpModuleName)
        {
            if (lpModuleName != IntPtr.Zero)
            {
                string fileName = Marshal.PtrToStringAnsi(lpModuleName);
                if (fileName.ToLower().Contains("easyhook") || fileName.ToLower().Contains("aphack"))
                {
                    IntPtr trash = Marshal.StringToHGlobalAnsi("ajhartyjshsg.dll");
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
