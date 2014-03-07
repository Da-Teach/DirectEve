using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AdapteveDLL
{
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
        public uint VendorId { get; set; }        
        public string GpuIdentifier { get; set; }        
        public long GpuDriverVersion { get; set; }        
        public uint GpuRevision { get; set; }
               

        public Settings(string iniFile)
        {
            if (!File.Exists(iniFile))
                throw new FileNotFoundException("Couldn't find " + iniFile);

            int index = 0;
            foreach(var line in File.ReadAllLines(iniFile))
            {
                index++;
                var sLine = line.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (sLine.Count() != 2)
                    throw new ArgumentException("IniFile not right format at line: " + index);

                switch(sLine[0])
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
                        VendorId = Convert.ToUInt32(sLine[1]);
                        break;

                    case "GpuIdentifier":
                        GpuIdentifier = sLine[1];
                        break;

                    case "GpuDriverVersion":
                        GpuDriverVersion = Convert.ToInt64(sLine[1]);
                        break;

                    case "GpuRevision":
                        GpuRevision = Convert.ToUInt32(sLine[1]);
                        break;
                }
            }

            if (this.TotalPhysRam == null ||
                this.WindowsKey == null ||
                this.WindowsUserLogin == null ||
                this.MacAddress == null ||
                this.NetworkAdapterGuid == null ||
                this.NetworkAddress == null ||
                this.Computername == null)
                throw new ArgumentException("Not all settings are set in the ini file");

            if (!Directory.Exists("c:/users/" + this.WindowsUserLogin))
                throw new ArgumentException("Please create the folder: c:/users/" + this.WindowsUserLogin);
        }
    }
}
