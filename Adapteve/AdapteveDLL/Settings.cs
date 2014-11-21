// ------------------------------------------------------------------------------
//   <copyright from='2010' to='2015' company='THEHACKERWITHIN.COM'>
//     Copyright (c) TheHackerWithin.COM. All Rights Reserved.
// 
//     Please look in the accompanying license.htm file for the license that 
//     applies to this source code. (a copy can also be found at: 
//     http://www.thehackerwithin.com/license.htm)
//   </copyright>
// -------------------------------------------------------------------------------

namespace AdapteveDLL
{
    using System;
    using System.IO;
    using System.Linq;

    public class Settings
    {
        public ulong TotalPhysRam { get; set; }
        public string WindowsUserLogin { get; set; }
        public string Computername { get; set; }
        public string WindowsKey { get; set; }
        public string NetworkAdapterGuid { get; set; }
        public string NetworkAddress { get; set; }
        public string MacAddress { get; set; }
        public string ProcessorIdent { get; set; }
        public string ProcessorRev { get; set; }
        public string ProcessorCoreAmount { get; set; }
        public string ProcessorLevel { get; set; }
        public string GpuDescription { get; set; }
        public uint GpuDeviceId { get; set; }
        public uint GpuVendorId { get; set; }
        public uint GpuRevision { get; set; }


        public Settings(string iniFile)
        {
            if (!File.Exists(iniFile))
                throw new FileNotFoundException("Couldn't find " + iniFile);

            var index = 0;
            foreach (var line in File.ReadAllLines(iniFile))
            {
                index++;
                var sLine = line.Split(new string[] {"="}, StringSplitOptions.RemoveEmptyEntries);
                if (sLine.Count() != 2)
                    throw new ArgumentException("IniFile not right format at line: " + index);

                switch (sLine[0])
                {
                    case "TotalPhysRam":
                        TotalPhysRam = Convert.ToUInt64(sLine[1]);
                        break;

                    case "WindowsUserLogin":
                        WindowsUserLogin = sLine[1];
                        break;

                    case "Computername":
                        Computername = sLine[1];
                        break;

                    case "WindowsKey":
                        WindowsKey = sLine[1];
                        break;

                    case "NetworkAdapterGuid":
                        NetworkAdapterGuid = sLine[1];
                        break;

                    case "NetworkAddress":
                        NetworkAddress = sLine[1];
                        break;

                    case "MacAddress":
                        MacAddress = sLine[1];
                        break;

                    case "ProcessorIdent":
                        ProcessorIdent = sLine[1];
                        break;

                    case "ProcessorRev":
                        ProcessorRev = sLine[1];
                        break;

                    case "ProcessorCoreAmount":
                        ProcessorRev = sLine[1];
                        break;

                    case "ProcessorLevel":
                        ProcessorLevel = sLine[1];
                        break;

                    case "GpuDescription":
                        GpuDescription = sLine[1];
                        break;

                    case "GpuDeviceId":
                        GpuDeviceId = Convert.ToUInt32(sLine[1]);
                        break;

                    case "VendorId":
                        GpuVendorId = Convert.ToUInt32(sLine[1]);
                        break;

                    case "GpuRevision":
                        GpuRevision = Convert.ToUInt32(sLine[1]);
                        break;
                }
            }

            if (TotalPhysRam == null ||
                WindowsKey == null ||
                WindowsUserLogin == null ||
                MacAddress == null ||
                NetworkAdapterGuid == null ||
                NetworkAddress == null ||
                Computername == null)
                throw new ArgumentException("Not all settings are set in the ini file");

            if (!Directory.Exists("c:/users/" + WindowsUserLogin))
                throw new ArgumentException("Please create the folder: c:/users/" + WindowsUserLogin);
        }
    }
}