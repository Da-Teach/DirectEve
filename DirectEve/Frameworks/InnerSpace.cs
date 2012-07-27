namespace DirectEve
{
    using System;
#if WIP
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
#else
    using LavishScriptAPI;
    using InnerSpaceAPI;
#endif

    public class InnerSpaceFramework : IFramework
    {
        // remember the hook so we can dispose it later
        private uint _innerspaceOnFrameId;

        // remember the user's frame hook so we can call it
        private event EventHandler<EventArgs> _frameHook;

#if WIP
        /// <summary>
        /// The Lavish.InnerSpace.dll assembly
        /// </summary>
        private Assembly _lavishInnerSpaceAssembly;

        private Type _innerSpace;
        private Type _lavishScript;
        private Type _lsEventArgs;
        private Type _lavishScriptEvents;
        private Delegate _frameHookDelegate;
#endif

        /// <summary>
        /// This internal frame hook function works around the rigid InnerSpace type requirements.
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
#if WIP
        public InnerSpaceFramework()
        {
            string assemblyDirectory = null;

            Process[] innerSpaceProcesses = Process.GetProcessesByName("InnerSpace"); 
            if (innerSpaceProcesses.Length > 0)
            {   // should only be one
                Process innerSpaceProcess = innerSpaceProcesses[0];
                assemblyDirectory = innerSpaceProcess.MainModule.FileName;
                assemblyDirectory = Directory.GetParent(assemblyDirectory).FullName;
            }
            else
            {   // if the above code failed something really strange happened.  
                // try something else for good measure.
                Assembly myAssembly = Assembly.GetExecutingAssembly();
                string myAssemblyDir = Directory.GetParent(myAssembly.Location).FullName;
                string root = Path.GetPathRoot(myAssemblyDir);
                string[] parts = myAssemblyDir.Split(Path.DirectorySeparatorChar);
                int index = Array.FindIndex(parts, x => x.ToLower().Contains(".net programs"));
                if (index > 0)
                {   // get all parts up to the .Net Programs directory
                    assemblyDirectory = root+Path.Combine(parts.Skip(1).Take(index-1).ToArray());
                }
            }

            if (string.IsNullOrEmpty(assemblyDirectory) == false)
            {
                // load the InnerSpace assembly
                _lavishInnerSpaceAssembly = Assembly.LoadFrom(Path.Combine(assemblyDirectory, "Lavish.InnerSpace.dll"));

                // use reflection to get all the InnerSpace classes and types at runtime
                Module[] mods = _lavishInnerSpaceAssembly.GetModules();
                Type[] types = _lavishInnerSpaceAssembly.GetTypes();

                _lavishScript = _lavishInnerSpaceAssembly.GetType("LavishScriptAPI.LavishScript");
                _lavishScriptEvents = _lavishScript.GetNestedType("Events");
                _innerSpace = _lavishInnerSpaceAssembly.GetType("InnerSpaceAPI.InnerSpace");
                _lsEventArgs = _lavishInnerSpaceAssembly.GetType("LavishScriptAPI.LSEventArgs");
                Type evType = typeof(EventHandler<>);
                Type[] typeArgs = { _lsEventArgs };
                Type lsEvType = evType.MakeGenericType(typeArgs);
                MethodInfo mi = typeof(InnerSpaceFramework).GetMethod("FrameHook", BindingFlags.Public | BindingFlags.Instance);
                _frameHookDelegate = Delegate.CreateDelegate(lsEvType, this, mi);
            }
        }
#endif
        public void RegisterFrameHook(EventHandler<EventArgs> frameHook)
        {
            _frameHook = frameHook;
#if WIP
            _innerspaceOnFrameId = (uint)_lavishScriptEvents.InvokeMember("RegisterEvent",
                BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public,
                null, null, new Object[] { "OnFrame" });
            _lavishScriptEvents.InvokeMember("AttachEventTarget",
                BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public,
                null, null, new Object[] { _innerspaceOnFrameId, _frameHookDelegate });
#else
            _innerspaceOnFrameId = LavishScript.Events.RegisterEvent("OnFrame");
            LavishScript.Events.AttachEventTarget(_innerspaceOnFrameId, FrameHook);
#endif
        }

        public void RegisterLogger(EventHandler<EventArgs> logger)
        {
            // no logger needed for InnerSpace
        }

        public void Log(string msg)
        {
#if WIP
            _innerSpace.InvokeMember("Echo",
                BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public,
                null, null, new Object[] { msg });
#else
            InnerSpace.Echo(msg);
#endif
        }

        #region IDisposable Members
        public void Dispose()
        {
#if WIP
            _lavishScriptEvents.InvokeMember("DetachEventTarget",
                BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public,
                null, null, new Object[] { _innerspaceOnFrameId, _frameHookDelegate });
#else
            LavishScript.Events.DetachEventTarget(_innerspaceOnFrameId, FrameHook);
#endif
        }
        #endregion
    }
}