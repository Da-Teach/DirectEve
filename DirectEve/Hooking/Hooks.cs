using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using WhiteMagic;

namespace DirectEve
{
    internal class Hooks
    {
        #region Native structures
        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_DOS_HEADER
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public char[] e_magic;       // Magic number
            public UInt16 e_cblp;    // Bytes on last page of file
            public UInt16 e_cp;      // Pages in file
            public UInt16 e_crlc;    // Relocations
            public UInt16 e_cparhdr;     // Size of header in paragraphs
            public UInt16 e_minalloc;    // Minimum extra paragraphs needed
            public UInt16 e_maxalloc;    // Maximum extra paragraphs needed
            public UInt16 e_ss;      // Initial (relative) SS value
            public UInt16 e_sp;      // Initial SP value
            public UInt16 e_csum;    // Checksum
            public UInt16 e_ip;      // Initial IP value
            public UInt16 e_cs;      // Initial (relative) CS value
            public UInt16 e_lfarlc;      // File address of relocation table
            public UInt16 e_ovno;    // Overlay number
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public UInt16[] e_res1;    // Reserved words
            public UInt16 e_oemid;       // OEM identifier (for e_oeminfo)
            public UInt16 e_oeminfo;     // OEM information; e_oemid specific
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public UInt16[] e_res2;    // Reserved words
            public Int32 e_lfanew;      // File address of new exe header

            private string _e_magic
            {
            get { return new string(e_magic); }
            }

            public bool isValid
            {
            get { return _e_magic == "MZ"; }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_DATA_DIRECTORY
        {
            public UInt32 VirtualAddress;
            public UInt32 Size;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_FILE_HEADER
        {
            public UInt16 Machine;
            public UInt16 NumberOfSections;
            public UInt32 TimeDateStamp;
            public UInt32 PointerToSymbolTable;
            public UInt32 NumberOfSymbols;
            public UInt16 SizeOfOptionalHeader;
            public UInt16 Characteristics;
        }

        public enum MachineType : ushort
        {
            Native = 0,
            I386 = 0x014c,
            Itanium = 0x0200,
            x64 = 0x8664
        }
        public enum MagicType : ushort
        {
            IMAGE_NT_OPTIONAL_HDR32_MAGIC = 0x10b,
            IMAGE_NT_OPTIONAL_HDR64_MAGIC = 0x20b
        }
        public enum SubSystemType : ushort
        {
            IMAGE_SUBSYSTEM_UNKNOWN = 0,
            IMAGE_SUBSYSTEM_NATIVE = 1,
            IMAGE_SUBSYSTEM_WINDOWS_GUI = 2,
            IMAGE_SUBSYSTEM_WINDOWS_CUI = 3,
            IMAGE_SUBSYSTEM_POSIX_CUI = 7,
            IMAGE_SUBSYSTEM_WINDOWS_CE_GUI = 9,
            IMAGE_SUBSYSTEM_EFI_APPLICATION = 10,
            IMAGE_SUBSYSTEM_EFI_BOOT_SERVICE_DRIVER = 11,
            IMAGE_SUBSYSTEM_EFI_RUNTIME_DRIVER = 12,
            IMAGE_SUBSYSTEM_EFI_ROM = 13,
            IMAGE_SUBSYSTEM_XBOX = 14

        }
        public enum DllCharacteristicsType : ushort
        {
            RES_0 = 0x0001,
            RES_1 = 0x0002,
            RES_2 = 0x0004,
            RES_3 = 0x0008,
            IMAGE_DLL_CHARACTERISTICS_DYNAMIC_BASE = 0x0040,
            IMAGE_DLL_CHARACTERISTICS_FORCE_INTEGRITY = 0x0080,
            IMAGE_DLL_CHARACTERISTICS_NX_COMPAT = 0x0100,
            IMAGE_DLLCHARACTERISTICS_NO_ISOLATION = 0x0200,
            IMAGE_DLLCHARACTERISTICS_NO_SEH = 0x0400,
            IMAGE_DLLCHARACTERISTICS_NO_BIND = 0x0800,
            RES_4 = 0x1000,
            IMAGE_DLLCHARACTERISTICS_WDM_DRIVER = 0x2000,
            IMAGE_DLLCHARACTERISTICS_TERMINAL_SERVER_AWARE = 0x8000
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct IMAGE_OPTIONAL_HEADER32
        {
            [FieldOffset(0)]
            public MagicType Magic;

            [FieldOffset(2)]
            public byte MajorLinkerVersion;

            [FieldOffset(3)]
            public byte MinorLinkerVersion;

            [FieldOffset(4)]
            public uint SizeOfCode;

            [FieldOffset(8)]
            public uint SizeOfInitializedData;

            [FieldOffset(12)]
            public uint SizeOfUninitializedData;

            [FieldOffset(16)]
            public uint AddressOfEntryPoint;

            [FieldOffset(20)]
            public uint BaseOfCode;

            // PE32 contains this additional field
            [FieldOffset(24)]
            public uint BaseOfData;

            [FieldOffset(28)]
            public uint ImageBase;

            [FieldOffset(32)]
            public uint SectionAlignment;

            [FieldOffset(36)]
            public uint FileAlignment;

            [FieldOffset(40)]
            public ushort MajorOperatingSystemVersion;

            [FieldOffset(42)]
            public ushort MinorOperatingSystemVersion;

            [FieldOffset(44)]
            public ushort MajorImageVersion;

            [FieldOffset(46)]
            public ushort MinorImageVersion;

            [FieldOffset(48)]
            public ushort MajorSubsystemVersion;

            [FieldOffset(50)]
            public ushort MinorSubsystemVersion;

            [FieldOffset(52)]
            public uint Win32VersionValue;

            [FieldOffset(56)]
            public uint SizeOfImage;

            [FieldOffset(60)]
            public uint SizeOfHeaders;

            [FieldOffset(64)]
            public uint CheckSum;

            [FieldOffset(68)]
            public SubSystemType Subsystem;

            [FieldOffset(70)]
            public DllCharacteristicsType DllCharacteristics;

            [FieldOffset(72)]
            public uint SizeOfStackReserve;

            [FieldOffset(76)]
            public uint SizeOfStackCommit;

            [FieldOffset(80)]
            public uint SizeOfHeapReserve;

            [FieldOffset(84)]
            public uint SizeOfHeapCommit;

            [FieldOffset(88)]
            public uint LoaderFlags;

            [FieldOffset(92)]
            public uint NumberOfRvaAndSizes;

            [FieldOffset(96)]
            public IMAGE_DATA_DIRECTORY ExportTable;

            [FieldOffset(104)]
            public IMAGE_DATA_DIRECTORY ImportTable;

            [FieldOffset(112)]
            public IMAGE_DATA_DIRECTORY ResourceTable;

            [FieldOffset(120)]
            public IMAGE_DATA_DIRECTORY ExceptionTable;

            [FieldOffset(128)]
            public IMAGE_DATA_DIRECTORY CertificateTable;

            [FieldOffset(136)]
            public IMAGE_DATA_DIRECTORY BaseRelocationTable;

            [FieldOffset(144)]
            public IMAGE_DATA_DIRECTORY Debug;

            [FieldOffset(152)]
            public IMAGE_DATA_DIRECTORY Architecture;

            [FieldOffset(160)]
            public IMAGE_DATA_DIRECTORY GlobalPtr;

            [FieldOffset(168)]
            public IMAGE_DATA_DIRECTORY TLSTable;

            [FieldOffset(176)]
            public IMAGE_DATA_DIRECTORY LoadConfigTable;

            [FieldOffset(184)]
            public IMAGE_DATA_DIRECTORY BoundImport;

            [FieldOffset(192)]
            public IMAGE_DATA_DIRECTORY IAT;

            [FieldOffset(200)]
            public IMAGE_DATA_DIRECTORY DelayImportDescriptor;

            [FieldOffset(208)]
            public IMAGE_DATA_DIRECTORY CLRRuntimeHeader;

            [FieldOffset(216)]
            public IMAGE_DATA_DIRECTORY Reserved;
        }
        [StructLayout(LayoutKind.Explicit)]
        public struct IMAGE_OPTIONAL_HEADER64
        {
            [FieldOffset(0)]
            public MagicType Magic;

            [FieldOffset(2)]
            public byte MajorLinkerVersion;

            [FieldOffset(3)]
            public byte MinorLinkerVersion;

            [FieldOffset(4)]
            public uint SizeOfCode;

            [FieldOffset(8)]
            public uint SizeOfInitializedData;

            [FieldOffset(12)]
            public uint SizeOfUninitializedData;

            [FieldOffset(16)]
            public uint AddressOfEntryPoint;

            [FieldOffset(20)]
            public uint BaseOfCode;

            [FieldOffset(24)]
            public ulong ImageBase;

            [FieldOffset(32)]
            public uint SectionAlignment;

            [FieldOffset(36)]
            public uint FileAlignment;

            [FieldOffset(40)]
            public ushort MajorOperatingSystemVersion;

            [FieldOffset(42)]
            public ushort MinorOperatingSystemVersion;

            [FieldOffset(44)]
            public ushort MajorImageVersion;

            [FieldOffset(46)]
            public ushort MinorImageVersion;

            [FieldOffset(48)]
            public ushort MajorSubsystemVersion;

            [FieldOffset(50)]
            public ushort MinorSubsystemVersion;

            [FieldOffset(52)]
            public uint Win32VersionValue;

            [FieldOffset(56)]
            public uint SizeOfImage;

            [FieldOffset(60)]
            public uint SizeOfHeaders;

            [FieldOffset(64)]
            public uint CheckSum;

            [FieldOffset(68)]
            public SubSystemType Subsystem;

            [FieldOffset(70)]
            public DllCharacteristicsType DllCharacteristics;

            [FieldOffset(72)]
            public ulong SizeOfStackReserve;

            [FieldOffset(80)]
            public ulong SizeOfStackCommit;

            [FieldOffset(88)]
            public ulong SizeOfHeapReserve;

            [FieldOffset(96)]
            public ulong SizeOfHeapCommit;

            [FieldOffset(104)]
            public uint LoaderFlags;

            [FieldOffset(108)]
            public uint NumberOfRvaAndSizes;

            [FieldOffset(112)]
            public IMAGE_DATA_DIRECTORY ExportTable;

            [FieldOffset(120)]
            public IMAGE_DATA_DIRECTORY ImportTable;

            [FieldOffset(128)]
            public IMAGE_DATA_DIRECTORY ResourceTable;

            [FieldOffset(136)]
            public IMAGE_DATA_DIRECTORY ExceptionTable;

            [FieldOffset(144)]
            public IMAGE_DATA_DIRECTORY CertificateTable;

            [FieldOffset(152)]
            public IMAGE_DATA_DIRECTORY BaseRelocationTable;

            [FieldOffset(160)]
            public IMAGE_DATA_DIRECTORY Debug;

            [FieldOffset(168)]
            public IMAGE_DATA_DIRECTORY Architecture;

            [FieldOffset(176)]
            public IMAGE_DATA_DIRECTORY GlobalPtr;

            [FieldOffset(184)]
            public IMAGE_DATA_DIRECTORY TLSTable;

            [FieldOffset(192)]
            public IMAGE_DATA_DIRECTORY LoadConfigTable;

            [FieldOffset(200)]
            public IMAGE_DATA_DIRECTORY BoundImport;

            [FieldOffset(208)]
            public IMAGE_DATA_DIRECTORY IAT;

            [FieldOffset(216)]
            public IMAGE_DATA_DIRECTORY DelayImportDescriptor;

            [FieldOffset(224)]
            public IMAGE_DATA_DIRECTORY CLRRuntimeHeader;

            [FieldOffset(232)]
            public IMAGE_DATA_DIRECTORY Reserved;
        }
        
        [StructLayout(LayoutKind.Explicit)]
        public struct IMAGE_NT_HEADERS32
        {
            [FieldOffset(0)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public char[] Signature;

            [FieldOffset(4)]
            public IMAGE_FILE_HEADER FileHeader;

            [FieldOffset(24)]
            public IMAGE_OPTIONAL_HEADER32 OptionalHeader;

            private string _Signature
            {
                get { return new string(Signature); }
            }

            public bool isValid
            {
                get { return _Signature == "PE\0\0" && OptionalHeader.Magic == MagicType.IMAGE_NT_OPTIONAL_HDR32_MAGIC; }
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct IMAGE_NT_HEADERS64
        {
            [FieldOffset(0)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public char[] Signature;

            [FieldOffset(4)]
            public IMAGE_FILE_HEADER FileHeader;

            [FieldOffset(24)]
            public IMAGE_OPTIONAL_HEADER64 OptionalHeader;

            private string _Signature
            {
                get { return new string(Signature); }
            }

            public bool isValid
            {
                get { return _Signature == "PE\0\0" && OptionalHeader.Magic == MagicType.IMAGE_NT_OPTIONAL_HDR64_MAGIC; }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_IMPORT_BY_NAME
        {
            public short Hint;
            public byte Name;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct IMAGE_IMPORT_DESCRIPTOR
        {
            #region union
            /// <summary>
            /// C# doesn't really support unions, but they can be emulated by a field offset 0
            /// </summary>

            [FieldOffset(0)]
            public uint Characteristics;            // 0 for terminating null import descriptor
            [FieldOffset(0)]
            public uint OriginalFirstThunk;         // RVA to original unbound IAT (PIMAGE_THUNK_DATA)
            #endregion

            [FieldOffset(4)]
            public uint TimeDateStamp;
            [FieldOffset(8)]
            public uint ForwarderChain;
            [FieldOffset(12)]
            public uint Name;
            [FieldOffset(16)]
            public uint FirstThunk;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct IMAGE_THUNK_DATA
        {
            [FieldOffset(0)]
            public uint ForwarderString;      // PBYTE 
            [FieldOffset(0)]
            public uint Function;             // PDWORD
            [FieldOffset(0)]
            public uint Ordinal;
            [FieldOffset(0)]
            public uint AddressOfData;        // PIMAGE_IMPORT_BY_NAME
        }

#endregion

#region Native functions
        [DllImport("kernel32.dll", CharSet=CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
#endregion

        static IntPtr GetThunk(IntPtr ModuleHandle,string intermodName,string funcName)
        {
            IntPtr rval = IntPtr.Zero;
            IntPtr iidPtr = IntPtr.Zero;


            IMAGE_DOS_HEADER idh;
            idh = (IMAGE_DOS_HEADER)Marshal.PtrToStructure(ModuleHandle, typeof(IMAGE_DOS_HEADER));
            if (idh.isValid)
            {
                IMAGE_NT_HEADERS32 inh32;
                IMAGE_NT_HEADERS64 inh64;
                inh32 = (IMAGE_NT_HEADERS32)Marshal.PtrToStructure(IntPtr.Add(ModuleHandle, idh.e_lfanew), typeof(IMAGE_NT_HEADERS32));
                inh64 = (IMAGE_NT_HEADERS64)Marshal.PtrToStructure(IntPtr.Add(ModuleHandle, idh.e_lfanew), typeof(IMAGE_NT_HEADERS64));
                if (inh32.isValid && inh32.OptionalHeader.ImportTable.VirtualAddress != 0)
                {
                    iidPtr = IntPtr.Add(ModuleHandle, (int)inh32.OptionalHeader.ImportTable.VirtualAddress);
                }
                else if (inh64.isValid && inh64.OptionalHeader.ImportTable.VirtualAddress != 0)
                {
                    iidPtr = IntPtr.Add(ModuleHandle, (int)inh64.OptionalHeader.ImportTable.VirtualAddress);
                }
            }

            if (iidPtr != IntPtr.Zero)
            {
                IMAGE_IMPORT_DESCRIPTOR iid = (IMAGE_IMPORT_DESCRIPTOR)Marshal.PtrToStructure(iidPtr, typeof(IMAGE_IMPORT_DESCRIPTOR));
                while (iid.Name != 0)
                {
                    string iidName = Marshal.PtrToStringAnsi(IntPtr.Add(ModuleHandle, (int)iid.Name));
                    if (string.Compare(iidName, intermodName, true) == 0)
                    {
                        // this probably won't work for 64-bit processes as the thunk data structure is different
                        IntPtr itdPtr = IntPtr.Add(ModuleHandle, (int)iid.FirstThunk);
                        IntPtr oitdPtr = IntPtr.Add(ModuleHandle, (int)iid.OriginalFirstThunk);
                        while (itdPtr != IntPtr.Zero && oitdPtr != IntPtr.Zero)
                        {
                            IMAGE_THUNK_DATA itd = (IMAGE_THUNK_DATA)Marshal.PtrToStructure(itdPtr, typeof(IMAGE_THUNK_DATA));
                            IMAGE_THUNK_DATA oitd = (IMAGE_THUNK_DATA)Marshal.PtrToStructure(oitdPtr, typeof(IMAGE_THUNK_DATA));

                            IntPtr iibnPtr = IntPtr.Add(ModuleHandle, (int)oitd.AddressOfData);
                            IMAGE_IMPORT_BY_NAME iibn = (IMAGE_IMPORT_BY_NAME)Marshal.PtrToStructure(iibnPtr, typeof(IMAGE_IMPORT_BY_NAME));
                            string iibnName = Marshal.PtrToStringAnsi(IntPtr.Add(iibnPtr, Marshal.OffsetOf(typeof(IMAGE_IMPORT_BY_NAME), "Name").ToInt32()));

                            if (string.Compare(iibnName, funcName, true) == 0)
                            {
                                rval = new IntPtr(itd.Function);
                                break;
                            }

                            itdPtr = IntPtr.Add(itdPtr, Marshal.SizeOf(typeof(IMAGE_THUNK_DATA)));
                            oitdPtr = IntPtr.Add(oitdPtr, Marshal.SizeOf(typeof(IMAGE_THUNK_DATA)));
                        }
                        break;
                    }
                    iidPtr = IntPtr.Add(iidPtr, Marshal.SizeOf(typeof(IMAGE_IMPORT_DESCRIPTOR)));
                    iid = (IMAGE_IMPORT_DESCRIPTOR)Marshal.PtrToStructure(iidPtr, typeof(IMAGE_IMPORT_DESCRIPTOR));
                }
            }

            return rval;
        }

        static DLoadLibraryA _DLoadLibraryA;
        static WhiteMagic.Internals.Detour _LoadLibraryAHook;
        static DLoadLibraryA_2 _DLoadLibraryA_2;
        static WhiteMagic.Internals.Detour _LoadLibraryAHook_2;
        static DGetModuleHandleA _DGetModuleHandleA;
        static WhiteMagic.Internals.Detour _GetModuleHandleAHook;
        static DGetModuleHandleA_2 _DGetModuleHandleA_2;
        static WhiteMagic.Internals.Detour _GetModuleHandleAHook_2;
        static DLoadLibraryW _DLoadLibraryW;
        static WhiteMagic.Internals.Detour _LoadLibraryWHook;
        static DGetModuleHandleW _DGetModuleHandleW;
        static WhiteMagic.Internals.Detour _GetModuleHandleWHook;
        static DBlockMiniDumpWriteDump _DBlockMiniWriteDump;
        static WhiteMagic.Internals.Detour _BlockMiniWriteDumpDetour;
        static DEnumProcesses _DEnumProcesses;
        static WhiteMagic.Internals.Detour _EnumProcessesDetour;

        internal static void InitializeHooks()
        {
            //LoadLibraryA
            IntPtr hMod = GetModuleHandle("_ctypes.pyd");
            if (hMod != IntPtr.Zero)
            {
                IntPtr ptr = GetThunk(hMod, "kernel32.dll", "LoadLibraryA");
                if (ptr != null)
                {
                    _DLoadLibraryA = Magic.Instance.RegisterDelegate<DLoadLibraryA>(ptr);
                    _LoadLibraryAHook = Magic.Instance.Detours.CreateAndApply(_DLoadLibraryA, new DLoadLibraryA(LoadLibraryAHooked), "LoadLibraryA");
                }
            }

            var address = GetProcAddresFunc("kernel32.dll", "LoadLibraryA");
            _DLoadLibraryA_2 = Magic.Instance.RegisterDelegate<DLoadLibraryA_2>(GetProcAddresFunc("kernel32.dll", "LoadLibraryA"));
            _LoadLibraryAHook_2 = Magic.Instance.Detours.CreateAndApply(_DLoadLibraryA_2, new DLoadLibraryA_2(LoadLibraryAHooked_2), "LoadLibraryA2");
                
            //GetmoduleHandleA
            hMod = GetModuleHandle("exefile.exe");
            if (hMod != IntPtr.Zero)
            {
                IntPtr ptr = GetThunk(hMod, "kernel32.dll", "GetModuleHandleA");
                if (ptr != null)
                {
                    _DGetModuleHandleA = Magic.Instance.RegisterDelegate<DGetModuleHandleA>(ptr);
                    _GetModuleHandleAHook = Magic.Instance.Detours.CreateAndApply(_DGetModuleHandleA, new DGetModuleHandleA(GetModuleHandleAHooked), "GetModuleHandleA"); 
                }
            }
            _DGetModuleHandleA_2 = Magic.Instance.RegisterDelegate<DGetModuleHandleA_2>(GetProcAddresFunc("kernel32.dll", "GetModuleHandleA"));
            _GetModuleHandleAHook_2 = Magic.Instance.Detours.CreateAndApply(_DGetModuleHandleA_2, new DGetModuleHandleA_2(GetModuleHandleAHooked_2), "GetModuleHandleA2"); 

            //LoadLibraryW
            _DLoadLibraryW = Magic.Instance.RegisterDelegate<DLoadLibraryW>(GetProcAddresFunc("kernel32.dll", "LoadLibraryW"));
            _LoadLibraryWHook = Magic.Instance.Detours.CreateAndApply(_DLoadLibraryW, new DLoadLibraryW(LoadLibraryWHooked), "LoadLibraryW");

            //GetModuleHandleW
            _DGetModuleHandleW = Magic.Instance.RegisterDelegate<DGetModuleHandleW>(GetProcAddresFunc("kernel32.dll", "GetModuleHandleW"));
            _GetModuleHandleWHook = Magic.Instance.Detours.CreateAndApply(_DGetModuleHandleW, new DGetModuleHandleW(GetModuleHandleWHooked), "GetModuleHandleW"); 

            //Blockminidump
            _DBlockMiniWriteDump = Magic.Instance.RegisterDelegate<DBlockMiniDumpWriteDump>(GetProcAddresFunc("dbghelp.dll", "MiniDumpWriteDump"));
            _BlockMiniWriteDumpDetour = Magic.Instance.Detours.CreateAndApply(_DBlockMiniWriteDump, new DBlockMiniDumpWriteDump(BlockMiniDumpWriteDumpDetour), "MiniDumpWriteDump"); 

            // Doesn't work yet ~ don't know why
            /*
            //EnumProcess/GetExeFilePids
            hMod = GetModuleHandle("psapi.dll");
            if (hMod != IntPtr.Zero)
            {
                IntPtr ptr = GetThunk(hMod, "kernel32.dll", "K32EnumProcesses");
                if (ptr != null)
                {
                    _DEnumProcesses = Magic.Instance.RegisterDelegate<DEnumProcesses>(ptr);
                    _BlockMiniWriteDumpDetour = Magic.Instance.Detours.CreateAndApply(_DEnumProcesses, new DEnumProcesses(EnumProcessesDetour), "K32EnumProcesses");
                }
            }
            */
            //IsDebuggerPresent left out otherwise we can't debug

        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
        private delegate IntPtr DLoadLibraryA(IntPtr lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        private static extern IntPtr LoadLibraryA(IntPtr lpFileName);

        private static IntPtr LoadLibraryAHooked(IntPtr lpFileName)
        {
            string fileName = Marshal.PtrToStringAnsi(lpFileName);
            if (fileName.ToLower().Contains("rgdll") || fileName.ToLower().Contains("directeve") || fileName.ToLower().Contains("questor"))
            {
                IntPtr trash = Marshal.StringToHGlobalAnsi("ajhajshsg.dll");
                return (IntPtr)_LoadLibraryAHook.CallOriginal(trash);
            }
            else return (IntPtr)_LoadLibraryAHook.CallOriginal(lpFileName);
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
        private delegate IntPtr DLoadLibraryA_2(IntPtr lpFileName);
        
        private static IntPtr LoadLibraryAHooked_2(IntPtr lpFileName)
        {
            string fileName = Marshal.PtrToStringAnsi(lpFileName);
            if (fileName.ToLower().Contains("rgdll") || fileName.ToLower().Contains("directeve") || fileName.ToLower().Contains("questor"))
            {
                IntPtr trash = Marshal.StringToHGlobalAnsi("ajhajshsg.dll");
                return (IntPtr)_LoadLibraryAHook_2.CallOriginal(trash);
            }
            else return (IntPtr)_LoadLibraryAHook_2.CallOriginal(lpFileName);
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
        private delegate IntPtr DGetModuleHandleA(IntPtr lpFileName);
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        public static extern IntPtr GetModuleHandleA(IntPtr lpModuleName);

        private static IntPtr GetModuleHandleAHooked(IntPtr lpModuleName)
        {
            string fileName = Marshal.PtrToStringAnsi(lpModuleName);
            if (fileName.ToLower().Contains("rgdll") || fileName.ToLower().Contains("directeve") || fileName.ToLower().Contains("questor"))
            {
                IntPtr trash = Marshal.StringToHGlobalAnsi("ajhajshsg.dll");
                return (IntPtr)_GetModuleHandleAHook.CallOriginal(trash);
            }
            else return (IntPtr)_GetModuleHandleAHook.CallOriginal(lpModuleName);
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
        private delegate IntPtr DGetModuleHandleA_2(IntPtr lpFileName);

        private static IntPtr GetModuleHandleAHooked_2(IntPtr lpModuleName)
        {
            string fileName = Marshal.PtrToStringAnsi(lpModuleName);
            if (fileName.ToLower().Contains("rgdll") || fileName.ToLower().Contains("directeve") || fileName.ToLower().Contains("questor"))
            {
                IntPtr trash = Marshal.StringToHGlobalAnsi("ajhajshsg.dll");
                return (IntPtr)_GetModuleHandleAHook_2.CallOriginal(trash);
            }
            else return (IntPtr)_GetModuleHandleAHook_2.CallOriginal(lpModuleName);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern void SetLastError(int errorCode);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
        private delegate IntPtr DLoadLibraryW(IntPtr lpFileName);

        private static IntPtr LoadLibraryWHooked(IntPtr lpFileName)
        {
            string fileName = Marshal.PtrToStringUni(lpFileName);
            if (fileName.ToLower().Contains("rgdll") || fileName.ToLower().Contains("directeve") || fileName.ToLower().Contains("questor"))
            {
                IntPtr trash = Marshal.StringToHGlobalUni("ajhajshsg.dll");
                return (IntPtr)_LoadLibraryWHook.CallOriginal(trash);
            }
            else return (IntPtr)_LoadLibraryWHook.CallOriginal(lpFileName);
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
        private delegate IntPtr DGetModuleHandleW(IntPtr lpFileName);
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        public static extern IntPtr GetModuleHandleW(IntPtr lpModuleName);

        private static IntPtr GetModuleHandleWHooked(IntPtr lpModuleName)
        {
            string fileName = Marshal.PtrToStringUni(lpModuleName);
            if (fileName.ToLower().Contains("rgdll") || fileName.ToLower().Contains("directeve") || fileName.ToLower().Contains("questor"))
            {
                IntPtr trash = Marshal.StringToHGlobalUni("ajhajshsg.dll");
                return (IntPtr)_GetModuleHandleWHook.CallOriginal(trash);
            }
            else return (IntPtr)_GetModuleHandleWHook.CallOriginal(lpModuleName);
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
        private delegate bool DBlockMiniDumpWriteDump(IntPtr lpModuleName, long processId, IntPtr hFile, long DumpType, long ExceptionParam, long UserStreamParam, long CallbackParam);

        private static bool BlockMiniDumpWriteDumpDetour(IntPtr lpModuleName, long processId, IntPtr hFile, long DumpType, long ExceptionParam, long UserStreamParam, long CallbackParam)
        {
            SetLastError(8);
            return false;
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
        private delegate bool DEnumProcesses([MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U4)] [In][Out] UInt32[] processIds, UInt32 arraySizeBytes,[MarshalAs(UnmanagedType.U4)] out UInt32 bytesCopied);

        private static bool EnumProcessesDetour([MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U4)] [In][Out] UInt32[] processIds, UInt32 arraySizeBytes, [MarshalAs(UnmanagedType.U4)] out UInt32 bytesCopied)
        {
            processIds = new uint[] { (UInt32)System.Diagnostics.Process.GetCurrentProcess().Id };
            bytesCopied = (UInt32)Marshal.SizeOf(processIds);
            return true;
        }


        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        private static IntPtr GetProcAddresFunc(string dllname, string function)
        {
            var hDLL = GetModuleHandle(dllname);
            return GetProcAddress(hDLL, function);
        }
    }
}
