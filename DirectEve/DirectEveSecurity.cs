namespace DirectEve
{
    using System;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Security;

    internal class DirectEveSecurity
    {
        private const uint CheckOne = 0xBADF00D;
        private const uint CheckTwo = 0xDEADC0DE;
        private const uint Outcome = 0 ^ CheckOne ^ CheckTwo;

        private Version _version;
        private DirectEve _directEve;
        private uint _isInitialized;

        internal DirectEveSecurity(DirectEve directEve)
        {
            _directEve = directEve;
            _isInitialized = 0;

            StartDirectEve();
        }

        private void Check1()
        {
            _version = Assembly.GetExecutingAssembly().GetName().Version;
            using (var pySharp = new PySharp.PySharp(false))
            {
                var machoVersion = (int)pySharp.Import("macho").Attribute("version");
                if (_version.Minor == machoVersion)
                    _isInitialized ^= CheckOne;
            }
        }

        private void Check2()
        {
            try
            {
                var request = (HttpWebRequest) WebRequest.Create("http://www.thehackerwithin.com/version.aspx?version=" + _version);
                var response = request.GetResponse();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    if (reader.ReadToEnd() == _version.ToString())
                        _isInitialized ^= CheckTwo;
                }
            }
            catch { } // Eat the exception
        }

        private void StartDirectEve()
        {
            Check1();
            Check2();
        }

        internal bool IsValid { get { return _isInitialized == Outcome; } }

        internal bool Pulse()
        {
            var machoVersion = (int)_directEve.PySharp.Import("macho").Attribute("version");
            return (_version.Minor == machoVersion && IsValid);
        }

        internal void QuitDirectEve()
        { }
    }
}
