namespace DirectEve
{
    using System;
    using LavishScriptAPI;
    using InnerSpaceAPI;

    public class InnerSpaceFramework : IFramework
    {
        // remember the hook so we can dispose it later
        private uint _innerspaceOnFrameId;

        // remember the user's frame hook so we can call it
        private event EventHandler<EventArgs> _frameHook;

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