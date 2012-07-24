namespace DirectEve
{
    using System;

    public interface IFramework : IDisposable
    {
        void RegisterFrameHook(EventHandler<EventArgs> frameHook);
        void RegisterLogger(EventHandler<EventArgs> logger);
        void Log(string msg);
    }
}