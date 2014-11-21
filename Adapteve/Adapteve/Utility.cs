// ------------------------------------------------------------------------------
//   <copyright from='2010' to='2015' company='THEHACKERWITHIN.COM'>
//     Copyright (c) TheHackerWithin.COM. All Rights Reserved.
// 
//     Please look in the accompanying license.htm file for the license that 
//     applies to this source code. (a copy can also be found at: 
//     http://www.thehackerwithin.com/license.htm)
//   </copyright>
// -------------------------------------------------------------------------------

namespace Adapteve
{
    using System;
    using System.IO;
    using AdapteveDLL;

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

            var verify = new Settings(iniFile);
            return iniFile;
        }
    }
}