using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using EasyHook;

namespace AdapteveDLL
{
    public class NetworkAdapterHook : IDisposable
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
        private delegate int GetAdaptersInfoDelegate(IntPtr AdaptersInfo, IntPtr OutputBuffLen);

        private string _name;
        private LocalHook _hook;

        [DllImport("Iphlpapi.dll", SetLastError = true)]
        public static extern int GetAdaptersInfo(IntPtr AdaptersInfo, IntPtr OutputBuffLen);

        private string _guid;
        private string _mac;
        private string _address;
        public NetworkAdapterHook(IntPtr address, string guid, string mac, string ipaddress)
        {
            _guid = guid;
            _mac = mac;
            _address = ipaddress;

            _name = string.Format("GetAdaptersInfoHook_{0:X}", address.ToInt32());
            _hook = LocalHook.Create(address, new GetAdaptersInfoDelegate(GetAdaptersInfoDetour), this);
            _hook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });
        }

        private int GetAdaptersInfoDetour(IntPtr AdaptersInfo, IntPtr OutputBuffLen)
        {
            var result = GetAdaptersInfo(AdaptersInfo, OutputBuffLen);
            if (AdaptersInfo != IntPtr.Zero)
            {
                IP_ADAPTER_INFO structure = (IP_ADAPTER_INFO)Marshal.PtrToStructure(AdaptersInfo, typeof(IP_ADAPTER_INFO));
                structure.AdapterName = _guid;
                for (int i = 0; i < structure.Address.Length - 1; i = i + 2)
                {
                    var tekst = structure.Address[i].ToString("X2", System.Globalization.CultureInfo.InvariantCulture);
                    if (tekst == "00")
                        tekst = "0";

                    structure.Address[i] = Convert.ToByte(_mac.Replace("-", "")[i].ToString() + _mac.Replace("-", "")[i + 1].ToString(), 16);
                    structure.Next = IntPtr.Zero;
                }
                structure.IpAddressList.IpAddress.Address = _address;
                Marshal.StructureToPtr(structure, AdaptersInfo, true);
            }
            return result;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct IP_ADAPTER_INFO
        {
            public IntPtr Next;
            public Int32 ComboIndex;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256 + 4)]
            public string AdapterName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128 + 4)]
            public string AdapterDescription;
            public UInt32 AddressLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] Address;
            public Int32 Index;
            public UInt32 Type;
            public UInt32 DhcpEnabled;
            public IntPtr CurrentIpAddress;
            public IP_ADDR_STRING IpAddressList;
            public IP_ADDR_STRING GatewayList;
            public IP_ADDR_STRING DhcpServer;
            public bool HaveWins;
            public IP_ADDR_STRING PrimaryWinsServer;
            public IP_ADDR_STRING SecondaryWinsServer;
            public Int32 LeaseObtained;
            public Int32 LeaseExpires;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct IP_ADDR_STRING
        {
            public IntPtr Next;
            public IP_ADDRESS_STRING IpAddress;
            public IP_ADDRESS_STRING IpMask;
            public Int32 Context;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct IP_ADDRESS_STRING
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string Address;
        }


        public void Dispose()
        {
            if (_hook == null)
                return;

            _hook.Dispose();
            _hook = null;
        }
    }
}
