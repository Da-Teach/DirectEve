using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using AdapteveDLL;

namespace Adapteve
{
    public class Utility
    {
        
        public static string GetExefilePath(string path)
        {
            if (path.ToLower().EndsWith("exefile.exe"))
                return path;
            else if (File.Exists(path + "/bin/exefile.exe"))
                return path + "/bin/exefile.exe";
            else if (File.Exists(path + "/exefile.exe"))
                return path + "/exefile.exe";
            else
                throw new ArgumentException("Exefile path not found");
        }

        public static string VerifyIniFile(string iniFile)
        {
            iniFile = Directory.GetCurrentDirectory() + "/" + iniFile;
            if (!File.Exists(iniFile))
                throw new ArgumentException("IniFile not found");

            Settings verify = new Settings(iniFile);
            return iniFile;
        }

    }
}
