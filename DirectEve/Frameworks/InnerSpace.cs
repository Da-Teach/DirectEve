namespace DirectEve
{
    using System;
#if WIP
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
#endif
    using LavishScriptAPI;
    using InnerSpaceAPI;

    public class InnerSpaceFramework : IFramework
    {
        // remember the hook so we can dispose it later
        private uint _innerspaceOnFrameId;

        // remember the user's frame hook so we can call it
        private event EventHandler<EventArgs> _frameHook;

#if WIP
        /// <summary>
        /// The LavishScriptAPI.dll assembly
        /// </summary>
        private Assembly _lavishScriptAPI;
        
        /// <summary>
        /// The InnerSpaceAPI.dll assembly
        /// </summary>
        private Assembly _innerSpaceAPI;
#endif

        /// <summary>
        /// This internal frame hook function works around the rigid InnerSpace type requirements.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrameHook(object sender, LSEventArgs e)
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
            AssemblyName name = new AssemblyName();

            string assemblyDirectory = null;

            // figure out where our assembly was loaded from
            Process[] innerSpaceProcesses = Process.GetProcessesByName("InnderSpace"); 
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



            // load the InnerSpace assemblies
            //Assembly assemblyInstance = Assembly.LoadFrom(@"c:\AssemblyDir\SomeControls.dll");
            
            // use reflection to get all the InnerSpace classes and types at runtime

        }
#endif
        public void RegisterFrameHook(EventHandler<EventArgs> frameHook)
        {
            _frameHook = frameHook;
            _innerspaceOnFrameId = LavishScript.Events.RegisterEvent("OnFrame");
            LavishScript.Events.AttachEventTarget(_innerspaceOnFrameId, FrameHook);
        }

        public void RegisterLogger(EventHandler<EventArgs> logger)
        {
            // no logger needed for InnerSpace
        }

        public void Log(string msg)
        {
            InnerSpace.Echo(msg);           
        }

        #region IDisposable Members
        public void Dispose()
        {
            LavishScript.Events.DetachEventTarget(_innerspaceOnFrameId, FrameHook);
        }
        #endregion
    }
}