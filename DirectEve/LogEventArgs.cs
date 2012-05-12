namespace DirectEve
{
    using System;

    public class LogEventArgs : EventArgs
    {
        private string msg;

        public LogEventArgs(string messageData)
        {
            msg = messageData;
        }

        public string Message
        { 
            get
            {
                return msg;
            }
            set
            {
                msg = value;
            }
        }
    }
}