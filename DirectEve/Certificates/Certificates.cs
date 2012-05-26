namespace DirectEve.Certificates
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;

    internal static class Certificates
    {
        private const string Namespace = "DirectEve.Certificates";

        private static X509Certificate2 _clientToServer;
        private static X509Certificate2 _serverToClient;

        internal static X509Certificate2 ServerToClient
        {
            get { return _serverToClient ?? (_serverToClient = GetCertificate("ServerToClient.cer")); }
        }

        internal static X509Certificate2 ClientToServer
        {
            get { return _clientToServer ?? (_clientToServer = GetCertificate("ClientToServer.pfx", "!eenlangwachtwoord.")); }
        }

        private static X509Certificate2 GetCertificate(string filename, string password = null)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(Namespace + "." + filename))
            {
                var data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                return string.IsNullOrEmpty(password) ? new X509Certificate2(data) : new X509Certificate2(data, password);
            }
        }

        internal static string SignData(params object[] data)
        {
            return SignData(data.Aggregate("", (d, n) => d + "|" + n));
        }

        internal static string SignData(string data)
        {
            var buffer = Encoding.UTF8.GetBytes(data);
            var signature = ((RSACryptoServiceProvider)ClientToServer.PrivateKey).SignData(buffer, new SHA1CryptoServiceProvider());
            return Convert.ToBase64String(signature);
        }

        internal static bool VerifyData(string signature, string data)
        {
            try
            {
                var buffer = Encoding.UTF8.GetBytes(data);
                var signatureBuffer = Convert.FromBase64String(signature);
                return ((RSACryptoServiceProvider)ServerToClient.PublicKey.Key).VerifyData(buffer, new SHA1CryptoServiceProvider(), signatureBuffer);
            }
            catch
            {
                return false;
            }
        }

        internal static bool VerifyData(string signature, params object[] data)
        {
            return VerifyData(signature, data.Aggregate("", (d, n) => d + "|" + n));
        }
    }
}
