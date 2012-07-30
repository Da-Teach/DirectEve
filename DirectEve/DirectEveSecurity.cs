namespace DirectEve
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Security;
    using System.ServiceModel;
    using System.Threading;
    using System.Xml.Linq;
    using global::DirectEve.LicenseServer;
    using Certs = global::DirectEve.Certificates.Certificates;

    internal class DirectEveSecurity
    {
        private DirectEve _directEve;

        private const string _retrieveLicenseUrl = "http://support.thehackerwithin.com/Subscription/GenerateLicense";
        private const string _licenseServer = "http://license.thehackerwithin.com/LicenseV1.svc";
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
#if DEBUG
            _directEve.Log("DirectEve: Debug: Starting security");
#endif
            _version = Assembly.GetExecutingAssembly().GetName().Version;
            _pulseResult = true;
            _lastPulse = DateTime.Now;

            PerformStartupChecks();
        }

        private void PerformStartupChecks()
        {
#if DEBUG
            _directEve.Log("DirectEve: Debug: loading license");
#endif
            // Load DirectEve license
            LoadLicense();

#if DEBUG
            _directEve.Log("DirectEve: Debug: startup check");
#endif
            // Check DirectEve version on server
            PerformStartupCheck();

#if DEBUG
            _directEve.Log("DirectEve: Debug: checking version");
#endif
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
            var version = _version.ToString();
            var challenge = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            var signature = Certs.SignData(_email, _licenseKey, version, challenge);

            try
            {
                StartupResponseV1 response;
                using (var client = new LicenseV1Client(new BasicHttpBinding(), new EndpointAddress(_licenseServer)))
                    response = client.Startup(_email, _licenseKey, version, challenge, signature);

                if (!Certs.VerifyData(response.Signature, response.InstanceId, response.Email, response.LicenseKey, response.ActiveInstances, response.SupportInstances, response.Challenge))
                    throw new Exception();

                _instanceId = response.InstanceId;
                _activeInstances = response.ActiveInstances;
                _supportInstances = response.SupportInstances;
            }
            catch (FaultException<LicenseFault> exception)
            {
                throw new SecurityException(exception.Detail.Fault);
            }
            catch (Exception)
            {
                throw new SecurityException(_verifyError);
            }
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
                var challenge = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                var signature = Certs.SignData(_email, _licenseKey, _instanceId, challenge);

                UpdateResponseV1 response;
                using (var client = new LicenseV1Client(new BasicHttpBinding(), new EndpointAddress(_licenseServer)))
                    response = client.KeepAlive(_email, _licenseKey, _instanceId, challenge, signature);

                _pulseResult = Certs.VerifyData(response.Signature, response.InstanceId, response.Challenge);
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
            // Fire & forget
            try
            {
                var challenge = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                var signature = Certs.SignData(_email, _licenseKey, _instanceId, challenge);

                using (var client = new LicenseV1Client(new BasicHttpBinding(), new EndpointAddress(_licenseServer)))
                    client.Shutdown(_email, _licenseKey, _instanceId, challenge, signature);
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
