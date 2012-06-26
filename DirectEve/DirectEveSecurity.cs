namespace DirectEve
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Security;
    using System.Threading;
    using System.Xml.Linq;
    using Certs = global::DirectEve.Certificates.Certificates;
    using InnerSpaceAPI;

    internal class DirectEveSecurity
    {
        private DirectEve _directEve;

        private const string _retrieveLicenseUrl = "http://support.thehackerwithin.com/Subscription/GenerateLicense";
        private const string _startupUrl = "http://support.thehackerwithin.com/DirectEve/Startup";
        private const string _keepAliveUrl = "http://support.thehackerwithin.com/DirectEve/KeepAlive";
        private const string _shutdownUrl = "http://support.thehackerwithin.com/DirectEve/Shutdown";
        private const int _pulseInterval = 60;

        private const string _obsoleteDirectEve = "Your DirectEve version is obsolete, please download a new version from http://support.thehackerwithin.com !";
        private const string _invalidSupportLicense = "Invalid support license, please download a new support license from http://support.thehackerwithin.com !";
        private const string _verifyError = "Unable to verify your support license, please try again later !";

        private string _email;
        private Guid _licenseKey;
        private Version _version;
        private Guid _instanceId;
        private int _activeInstances;
        private int _supportInstances;

        private Thread _pulseThread;
        private bool _pulseResult;
        private DateTime _lastPulse;

        internal DirectEveSecurity(DirectEve directEve)
        {            
            _directEve = directEve;
            _directEve.Log("DirectEve: Debug: Starting security");
            _version = Assembly.GetExecutingAssembly().GetName().Version;
            _pulseResult = true;
            _lastPulse = DateTime.Now;

            PerformStartupChecks();
        }

        private void PerformStartupChecks()
        {
            _directEve.Log("DirectEve: Debug: loading license");
            // Load DirectEve license
            LoadLicense();

            _directEve.Log("DirectEve: Debug: startup check");
            // Check DirectEve version on server
            PerformStartupCheck();

            _directEve.Log("DirectEve: Debug: checking version");
            //Check if directeve is obsolete
            CheckVersion();
        }

        /// <summary>
        ///   Load the DirectEve license
        /// </summary>
        private void LoadLicense()
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var licensePath = Path.Combine(path, "DirectEve.lic");
            if (!File.Exists(licensePath))
                RetrieveAnonymousLicense(licensePath);

            var license = XElement.Load(licensePath);
            var email = (string)license.Element("email");
            var licenseKey = (Guid?)license.Element("licensekey") ?? Guid.Empty;
            var signature = (string)license.Element("signature");
            if (!Certs.VerifyData(signature, email, licenseKey))
                throw new SecurityException(_invalidSupportLicense);

            _email = email;
            _licenseKey = licenseKey;
        }

        /// <summary>
        ///   Perform a server call
        /// </summary>
        /// <returns></returns>
        private XElement PerformServerCall(string url, XElement xml)
        {
            try
            {
                if (xml.Element("signature") == null)
                {
                    var signData = xml.Elements().TakeWhile(element => element.Name != "signature").Select(element => (string)element).ToArray();
                    xml.Add(new XElement("signature", Certs.SignData(signData)));
                }


                byte[] buffer;
                using (var ms = new MemoryStream())
                {
                    xml.Save(ms);
                    buffer = ms.ToArray();
                }

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "text/xml";
                request.ContentLength = buffer.Length;

                var stream = request.GetRequestStream();
                stream.Write(buffer, 0, buffer.Length);
                stream.Close();

                XElement result;
                var response = request.GetResponse();
                using (var sr = new StreamReader(response.GetResponseStream()))
                    result = XElement.Parse(sr.ReadToEnd());

                if (result.Name == "error")
                    throw new SecurityException(result.Value);

                var signature = (string)result.Element("signature");
                if (string.IsNullOrEmpty(signature))
                    return null;

                var data = result.Elements().TakeWhile(element => element.Name != "signature").Select(element => (string)element).ToArray();
                if (!Certs.VerifyData(signature, data))
                    return null;

                return result;
            }
            catch(SecurityException)
            {
                throw;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///   Retrieve the anonymous license from the server
        /// </summary>
        private void RetrieveAnonymousLicense(string licensePath)
        {
            var licenseRequest = new XElement("request",
                new XElement("email", "anonymous"),
                new XElement("licensekey", Guid.Empty));

            var license = PerformServerCall(_retrieveLicenseUrl, licenseRequest);
            if (license == null)
                throw new SecurityException(_invalidSupportLicense);

            license.Save(licensePath);
        }

        /// <summary>
        ///   A check to see if 
        /// </summary>
        private void PerformStartupCheck()
        {
            var startupRequest = new XElement("request",
                new XElement("email", _email),
                new XElement("licensekey", _licenseKey),
                new XElement("version", _version.ToString()),
                new XElement("challenge", DateTime.Now));

            var startupResponse = PerformServerCall(_startupUrl, startupRequest);
            if (startupResponse == null)
                throw new SecurityException(_verifyError);

            _instanceId = (Guid)startupResponse.Element("instanceid");
            _activeInstances = (int?)startupResponse.Element("activeinstances") ?? 0;
            _supportInstances = (int?)startupResponse.Element("supportinstances") ?? 0;
        }

        /// <summary>
        ///   A quick check to see if machonet matches minor version of DirectEve
        /// </summary>
        private void CheckVersion()
        {
            /*try
            {
                using (var pySharp = new PySharp.PySharp(false))
                {
                    var machoVersion = (int)pySharp.Import("macho").Attribute("version");
                    if (_version.Minor != machoVersion)
                    {
                        _directEve.Log("DirectEve: Debug: version mismatch. Directeve minor = " + _version.Minor + " Eve version = " + machoVersion);
                        //throw new SecurityException(_obsoleteDirectEve);
                    }
                }
            }
            catch (Exception e)
            {
                _directEve.Log("DirectEve: Debug: Exception during CheckVersion(): " + e.StackTrace);
            }*/
        }

        /// <summary>
        ///   This pulses the server to keep it alive
        /// </summary>
        private void PulseThread()
        {
            try
            {
                var pulseRequest = new XElement("request",
                    new XElement("email", _email),
                    new XElement("licensekey", _licenseKey),
                    new XElement("instanceid", _instanceId),
                    new XElement("challenge", DateTime.Now));

                var result = PerformServerCall(_keepAliveUrl, pulseRequest);
                _pulseResult = result != null;
            }
            catch (Exception)
            {
                // An exception is not good :)
                _pulseResult = false;
            }
        }

        /// <summary>
        ///   This triggers a server pulse (note that pulse will never return false if the character is not in a station)
        /// </summary>
        /// <returns></returns>
        internal bool Pulse()
        {
            var lastPulse = DateTime.Now.Subtract(_lastPulse).TotalSeconds;
            if ((_pulseThread == null || !_pulseThread.IsAlive) && lastPulse >= _pulseInterval)
            {
                _pulseThread = new Thread(PulseThread);
                _pulseThread.Name = "DirectEve.PulseThread";
                _pulseThread.Start();

                _lastPulse = DateTime.Now;
            }

            // We are in space, always return true
            if (!_directEve.Session.IsInStation)
                return true;

            // We're in station, return pulse result
            return _pulseResult;
        }

        /// <summary>
        ///   This tells the server the directeve instance has been closed
        /// </summary>
        internal void QuitDirectEve()
        {
            var shutdownRequest = new XElement("request",
                new XElement("email", _email),
                new XElement("licensekey", _licenseKey),
                new XElement("instanceid", _instanceId),
                new XElement("challenge", DateTime.Now));

            // Fire & forget
            try
            {
                PerformServerCall(_shutdownUrl, shutdownRequest);
            }
            catch
            { }
        }

        internal Version Version { get { return _version; } }
        internal string Email { get { return _email; } }
        internal int ActiveInstances { get { return _activeInstances; } }
        internal int SupportInstances { get { return _supportInstances; } }
    }
}
