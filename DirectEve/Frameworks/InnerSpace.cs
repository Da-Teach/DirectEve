// ------------------------------------------------------------------------------
//   <copyright from='2010' to='2015' company='THEHACKERWITHIN.COM'>
//     Copyright (c) TheHackerWithin.COM. All Rights Reserved.
// 
//     Please look in the accompanying license.htm file for the license that 
//     applies to this source code. (a copy can also be found at: 
//     http://www.thehackerwithin.com/license.htm)
//   </copyright>
// -------------------------------------------------------------------------------

namespace DirectEve
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public class InnerSpaceFramework : IFramework
    {
        /// <summary>
        ///     The event ID we register with InnerSpace.
        /// </summary>
        private uint _innerspaceOnFrameId;

        /// <summary>
        ///     The user supplied FrameHook event handler.
        /// </summary>
        private event EventHandler<EventArgs> _frameHook;

        /// <summary>
        ///     The Lavish.InnerSpace.dll assembly
        /// </summary>
        private Assembly _lavishInnerSpaceAssembly;

        /// <summary>
        ///     The reflected InnerSpaceAPI.InnerSpace class.
        /// </summary>
        private Type _innerSpace;

        /// <summary>
        ///     The reflected LavishScriptAPI.LavishScript class.
        /// </summary>
        private Type _lavishScript;

        /// <summary>
        ///     The reflected LavishScriptAPI.LavishScript.Events class.
        /// </summary>
        private Type _lavishScriptEvents;

        /// <summary>
        ///     The reflected LavishScriptAPI.LSEventArgs  class.
        /// </summary>
        private Type _lsEventArgs;

        /// <summary>
        ///     A delegate that maps our FrameHook to EventHandler
        /// </summary>
        private Delegate _frameHookDelegate;

        /// <summary>
        ///     The reflected InnerSpaceAPI.InnerSpace.Echo method.
        /// </summary>
        private MethodInfo _echoMethod;

        /// <summary>
        ///     Internal frame hook function.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void FrameHook(object sender, EventArgs e)
        {
            var handler = _frameHook;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        /// <summary>
        ///     The IFramework constructor.  This method will perform runtime
        ///     binding of all the types and methods we need from InnerSpace.
        /// </summary>
        public InnerSpaceFramework()
        {
            var assemblyDirectory = GetInnerSpaceDirectory();

            if (string.IsNullOrEmpty(assemblyDirectory) == false)
            {
                // load the InnerSpace assembly
                _lavishInnerSpaceAssembly = Assembly.LoadFrom(Path.Combine(assemblyDirectory, "Lavish.InnerSpace.dll"));

                // use reflection to get all the InnerSpace classes and types at runtime
                _lavishScript = _lavishInnerSpaceAssembly.GetType("LavishScriptAPI.LavishScript");
                _lavishScriptEvents = _lavishScript.GetNestedType("Events");
                _innerSpace = _lavishInnerSpaceAssembly.GetType("InnerSpaceAPI.InnerSpace");
                _lsEventArgs = _lavishInnerSpaceAssembly.GetType("LavishScriptAPI.LSEventArgs");

                // Reflect the Echo() method so we can call it efficiently later
                _echoMethod = _innerSpace.GetMethod("Echo", BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public);

                // Build a delegate for our frame hook which makes InnerSpace think we passed it
                // an EventHandler<LSEventArgs> function instead of a generic event handler.
                var evType = typeof (EventHandler<>); // Get the EventHandler<T> type
                Type[] typeArgs = {_lsEventArgs}; // Create a type array containing the LSEventArgs type
                var lsEvType = evType.MakeGenericType(typeArgs); // Make an EventHandler<LSEventArgs> type
                var mi = typeof (InnerSpaceFramework).GetMethod("FrameHook", BindingFlags.Public | BindingFlags.Instance);
                _frameHookDelegate = Delegate.CreateDelegate(lsEvType, this, mi); //  << Delegate for an instance method
            }
        }

        /// <summary>
        ///     Attempt to find the directory where Lavish.InnerSpace.dll.  First find the InnerSpace.exe
        ///     process and get that directory.  If the InnerSpace process has been renamed assume that our
        ///     process is run from somewhere under the .NET Programs directory and find the directory
        ///     containing the .NET Programs directory.
        /// </summary>
        /// <returns>The directory where InnerSpace is installed or null.</returns>
        private static string GetInnerSpaceDirectory()
        {
            string assemblyDirectory = null;
            var innerSpaceProcesses = Process.GetProcessesByName("InnerSpace");
            if (innerSpaceProcesses.Length > 0)
            {
                // should only be one
                var innerSpaceProcess = innerSpaceProcesses[0];
                assemblyDirectory = innerSpaceProcess.MainModule.FileName;
                assemblyDirectory = Directory.GetParent(assemblyDirectory).FullName;
            }
            else
            {
                // if the above code failed something really strange happened.  
                // try something else for good measure.
                var myAssembly = Assembly.GetExecutingAssembly();
                var myAssemblyDir = Directory.GetParent(myAssembly.Location).FullName;
                var root = Path.GetPathRoot(myAssemblyDir);
                var parts = myAssemblyDir.Split(Path.DirectorySeparatorChar);
                var index = Array.FindIndex(parts, x => x.ToLower().Contains(".net programs"));
                if (index > 0)
                {
                    // get all parts up to the .Net Programs directory
                    assemblyDirectory = root + Path.Combine(parts.Skip(1).Take(index - 1).ToArray());
                }
            }
            return assemblyDirectory;
        }

        public void RegisterFrameHook(EventHandler<EventArgs> frameHook)
        {
            // save the user's frame hook
            _frameHook = frameHook;

            // Register for the InnerSpace OnFrame event
            _innerspaceOnFrameId = (uint) _lavishScriptEvents.InvokeMember("RegisterEvent",
                BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public,
                null, null, new Object[] {"OnFrame"});

            // Attach our frame hook delegate to the OnFrame event
            _lavishScriptEvents.InvokeMember("AttachEventTarget",
                BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public,
                null, null, new Object[] {_innerspaceOnFrameId, _frameHookDelegate});
        }

        public void RegisterLogger(EventHandler<EventArgs> logger)
        {
            // no logger needed for InnerSpace
        }

        public void Log(string msg)
        {
            // Invoke InnerSpaceAPI.InnerSpace.Echo()
            _echoMethod.Invoke(null, new Object[] {msg});
        }

        #region IDisposable Members

        public void Dispose()
        {
            // Detach our frame hook delegate from the OnFrame event
            _lavishScriptEvents.InvokeMember("DetachEventTarget",
                BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public,
                null, null, new Object[] {_innerspaceOnFrameId, _frameHookDelegate});
        }

        #endregion
    }
}