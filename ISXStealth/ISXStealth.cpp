//
// ISXStealth
//

// Version guideline: YYYYMMDD
// Add lettering to the end to indicate a new version on the same date, such as 20060305a, 20060305b, etc
// You can also use a standard version numbering system such as 1.00, 1.01, etc. 
// Be aware that for the versioning system, this text is simply compared to another version text from the 
// same extension to check for new versions -- if this version text comes before the compared text (in a 
// dictionary), then an update is available.  Equal text means the version is up to date.  After means this 
// is newer than the compared version.  With that said, use whatever version numbering system you'd like.
#define EXTENSION_VERSION "20130315"

#include "ISXStealth.h"
#pragma comment(lib,"isxdk.lib")
// The mandatory pre-setup function.  Our name is "ISXStealth", and the class is ISXStealth.
// This sets up a "ModulePath" variable which contains the path to this module in case we want it,
// and a "PluginLog" variable, which contains the path and filename of what we should use for our
// debug logging if we need it.  It also sets up a variable "pExtension" which is the pointer to
// our instanced class.
ISXPreSetup("ISXStealth",ISXStealth);

ISInterface *pISInterface=0;
HISXSERVICE hMemoryService;

char Stealth_Version[]=EXTENSION_VERSION;

// Forward declarations of callbacks
void __cdecl MemoryService(bool Broadcast, unsigned int MSG, void *lpData);

// The constructor of our class.  General initialization cannot be done yet, because we're not given
// the pointer to the Inner Space interface until it is ready for us to initialize.  Just set the
// pointer we have to the interface to 0.  Initialize data members, too.
ISXStealth::ISXStealth(void)
{
}

// Free any remaining resources in the destructor.  This is called when the DLL is unloaded, but
// Inner Space calls the "Shutdown" function first.  Most if not all of the shutdown process should
// be done in Shutdown.
ISXStealth::~ISXStealth(void)
{
}

// Initialize is called by Inner Space when the extension should initialize.
bool ISXStealth::Initialize(ISInterface *p_ISInterface)
{
	/* 
	 * Most of the functionality in Initialize is completely optional and could be removed or
	 * changed if so desired.  The defaults are simply a suggestion that can be easily followed.
	 */

	__try // exception handling. See __except below.
	{
		// Keep a global copy of the ISInterface pointer, which is for calling Inner Space API
		pISInterface=p_ISInterface;

		// Register the extension to make launching and updating the extension easy
		RegisterExtension();

		// Register LavishScript extensions (commands, aliases, data types, objects)
		RegisterCommands();

		// Connect to the memory service
		hMemoryService=pISInterface->ConnectService(this,"Memory",MemoryService);

		// Hook functions
		InitializeHooks();

		printf("ISXStealth version %s Loaded",Stealth_Version);
		return true;
	}

	// Exception handling sample.  Exception handling should at LEAST be used in functions that
	// are suspected of causing user crashes.  This will help users report the crash and hopefully
	// enable the extension developer to locate and fix the crash condition.
	__except(EzCrashFilter(GetExceptionInformation(),"Crash in initialize routine")) 
	{
		TerminateProcess(GetCurrentProcess(),0);
		return 0;
	}
}

DWORD ISXStealth::GetPEB()
{
	DWORD dwPebBase = 0;

	// Return PEB address for current process
	// address is located at FS:0x30
	__asm 
	{
		push eax
		mov eax, FS:[0x30]
		mov [dwPebBase], eax
		pop eax
	}

	return (DWORD)dwPebBase;
}

int ISXStealth::GetModuleList(char ModuleListType, PLIST_ENTRY *moduleListHead, DWORD *dwOffset)
{
	PLIST_ENTRY pUserModuleListHead;

	DWORD PebBaseAddr;
	PPEB_LDR_DATA pLdrData;
	
	PebBaseAddr = GetPEB();
	if (PebBaseAddr == 0)
		return 0;

	pLdrData=(PPEB_LDR_DATA)(DWORD *)(*(DWORD *)(PebBaseAddr + PEB_LDR_DATA_OFFSET)); // PEB.ProcessModuleInfo = PEB + 0x0C
	if(!pLdrData->Initialized) 
		return 0;

		// Init chained list head and offset
	if(ModuleListType == LOAD_ORDER_TYPE)
	{
		// LOAD_ORDER_TYPE
		(*moduleListHead) = (PLIST_ENTRY)(&(pLdrData->ModuleListLoadOrder));
		(*dwOffset) = 0x0;
	} else if(ModuleListType == MEM_ORDER_TYPE)
	{
		// MEM_ORDER_TYPE
		(*moduleListHead) = (PLIST_ENTRY)(&(pLdrData->ModuleListMemoryOrder));
		(*dwOffset) = 0x08;
	} else if(ModuleListType == INIT_ORDER_TYPE)
	{
		// INIT_ORDER_TYPE
		(*moduleListHead) = (PLIST_ENTRY)(&(pLdrData->ModuleListInitOrder));
		(*dwOffset) = 0x10;
	}

	return 0;
}

int ISXStealth::WalkModuleList(char moduleListType, int (*processEntry)(char, PLIST_ENTRY, char*, DWORD, DWORD, DWORD*), DWORD *data)
{
	PLIST_ENTRY moduleListHead;
	DWORD dwOffset;
	GetModuleList(moduleListType, &moduleListHead, &dwOffset);

	PLIST_ENTRY moduleListPtr = moduleListHead;
	PUNICODE_STRING imageName;
	DWORD imageBase, imageSize;
	char szImageName[BUFMAXLEN];

	do
	{
		// Jump to next MODULE_ITEM structure
		moduleListPtr = moduleListPtr->Flink;
		imageName = (PUNICODE_STRING)( ((DWORD)(moduleListPtr)) + (LDR_DATA_PATHFILENAME_OFFSET-dwOffset));
		imageBase = *(DWORD *)(((DWORD)(moduleListPtr)) + (LDR_DATA_IMAGE_BASE-dwOffset));
		imageSize = *(DWORD *)(((DWORD)(moduleListPtr)) + (LDR_DATA_IMAGE_SIZE-dwOffset));

        //Convert string from unicode and to lower case :
		int i;
		for(i = 0; i < (imageName->Length) / 2 && i < BUFMAXLEN; i++) 
              szImageName[i] = LOWCASE(*((imageName->Buffer)+(i)));
		szImageName[i] = '\0';

		if (processEntry(moduleListType, moduleListPtr, szImageName, imageBase, imageSize, data) < 0)
			break;

	}	while(moduleListPtr->Flink != moduleListHead); 

	return 0;
}

char *GetFilename(char *path)
{
	int lastSlash = -1;
	for(int i = 0; i < BUFMAXLEN; i ++)
	{
		if (path[i] == 0)
			break;

		if (path[i] == '\\')
			lastSlash = i;
	}

	return &path[lastSlash + 1];
}

int PrintModuleEntry(char entryType, PLIST_ENTRY entry, char *image_name, DWORD image_base, DWORD image_size, DWORD *data)
{
	printf("Visible: %s [%x][%x]", GetFilename(image_name), image_base, image_size);
	return 0;
}

void ISXStealth::ListModules()
{
	WalkModuleList(LOAD_ORDER_TYPE, &PrintModuleEntry, NULL);

	for (int i = 0; i < modules.size(); i ++)
		printf("Stealthed: %s [%x][%x]", modules[i]->name, modules[i]->image_base, modules[i]->image_size);
}

int RemoveModuleEntry(char entryType, PLIST_ENTRY entry, char *image_name, DWORD image_base, DWORD image_size, DWORD *data)
{
	PStealthModule stealthModule = (PStealthModule)data;
	
	char *filename = GetFilename(stealthModule->name);
	bool hide = false;
	if (filename == stealthModule->name)
		hide = strcmp(filename, GetFilename(image_name)) == 0;
	else
		hide = strcmp(stealthModule->name, image_name) == 0;
	
	if (hide)
	{
		// Hide this dll :
		// throw this module away (out of the double linked list)
		entry->Blink->Flink = entry->Flink;
        entry->Flink->Blink = entry->Blink;

		stealthModule->image_base = image_base;
		stealthModule->image_size = image_size;
		
		if (entryType == LOAD_ORDER_TYPE)
			stealthModule->load_order = entry;
		if (entryType == MEM_ORDER_TYPE)
			stealthModule->memory_order = entry;
		if (entryType == INIT_ORDER_TYPE)
			stealthModule->init_order = entry;

		return -1;
	}

	return 0;
}

void ISXStealth::StealthModule(char *module)
{
	printf("Stealthing %s", GetFilename(module));

	PStealthModule stealthModule = (PStealthModule)malloc(sizeof(_StealthModule));
	ZeroMemory(stealthModule, sizeof(_StealthModule));

	for(int i = 0; i < BUFMAXLEN; i ++)
	{
		stealthModule->name[i] = LOWCASE(module[i]);
		stealthModule->w_name[i] = (wchar_t)stealthModule->name[i];
		if (module[i] == '\0')
			break;
	}

	WalkModuleList(LOAD_ORDER_TYPE, &RemoveModuleEntry, (DWORD *)stealthModule);
	WalkModuleList(MEM_ORDER_TYPE, &RemoveModuleEntry, (DWORD *)stealthModule);
	WalkModuleList(INIT_ORDER_TYPE, &RemoveModuleEntry, (DWORD *)stealthModule);

	if (stealthModule->init_order != NULL)
	{
		printf("Stealthed %s [%x][%x]", GetFilename(stealthModule->name), stealthModule->image_base, stealthModule->image_size);
		modules.push_back(stealthModule);
	}
	else
	{
		printf("Failed to stealth %s", GetFilename(stealthModule->name));
		free(stealthModule);
	}
}

void AddModuleEntry(PLIST_ENTRY head, PLIST_ENTRY entry)
{
	entry->Blink = head;
	entry->Flink = head->Flink;
	head->Flink = entry;
	entry->Flink->Blink = entry;
}

void ISXStealth::UnstealthModule(PStealthModule module)
{
	PLIST_ENTRY list;
	DWORD dwOffset;

	GetModuleList(LOAD_ORDER_TYPE, &list, &dwOffset);
	AddModuleEntry(list, module->load_order);
	GetModuleList(MEM_ORDER_TYPE, &list, &dwOffset);
	AddModuleEntry(list, module->memory_order);
	GetModuleList(INIT_ORDER_TYPE, &list, &dwOffset);
	AddModuleEntry(list, module->init_order);
}

void ISXStealth::UnstealthModule(char *module)
{
	printf("Unstealthing %s", module);

	char *filename = GetFilename(module);
	for(int i = modules.size() -1; i >= 0; i --)
	{
		PStealthModule stealthModule = modules[i];

		bool unhide = false;
		if (filename == module)
			unhide = strcmp(filename, GetFilename(stealthModule->name)) == 0;
		else
			unhide = strcmp(module, stealthModule->name) == 0;

		if (unhide)
		{
			UnstealthModule(stealthModule);
			modules.erase(modules.begin() + i);
		}
	}
}

DETOUR_TRAMPOLINE_EMPTY(BOOL WINAPI MiniDumpWriteDumpTrampoline(HANDLE hProcess, DWORD ProcessId, HANDLE hFile, DWORD DumpType, DWORD ExceptionParam, DWORD UserStreamParam, DWORD CallbackParam));
BOOL WINAPI MiniDumpWriteDumpDetour(HANDLE hProcess, DWORD ProcessId, HANDLE hFile, DWORD DumpType, DWORD ExceptionParam, DWORD UserStreamParam, DWORD CallbackParam)
{
	// Out of memory error
	SetLastError(8);
	return FALSE;
}

void ISXStealth::BlockMiniDump(bool block)
{
	if (block != blockMiniDump)
	{
		blockMiniDump = block;
		if (block)
		{
			EzDetour(GetProcAddress(LoadLibrary("dbghelp.dll"), "MiniDumpWriteDump"), MiniDumpWriteDumpDetour, MiniDumpWriteDumpTrampoline);
		}
		else
		{
			EzUnDetour(GetProcAddress(GetModuleHandle("dbghelp.dll"), "MiniDumpWriteDump"));
		}
	}

	printf("We are currently %s MiniDumpWriteDump", block ? "blocking" : "not blocking");
}

DETOUR_TRAMPOLINE_EMPTY(HMODULE WINAPI GetModuleHandleATrampoline( LPCSTR lpModuleName ));
HMODULE WINAPI GetModuleHandleADetour( LPCSTR lpModuleName )
{
	HMODULE rval = 0;

	vector<PStealthModule> modules = pExtension->modules;

	int i;
	for( i = modules.size() - 1; i >= 0; i-- ) 
	{
		PStealthModule stealthModule = modules[i];
		if( CompareStringA(LOCALE_SYSTEM_DEFAULT,LINGUISTIC_IGNORECASE,stealthModule->name,
			lstrlenA(stealthModule->name),lpModuleName,lstrlenA(lpModuleName)) == CSTR_EQUAL )
		{
			break;
		}
	}

	if ( CompareStringA(LOCALE_SYSTEM_DEFAULT, LINGUISTIC_IGNORECASE, "directeve.dll", lstrlenA("directeve.dll"), lpModuleName, lstrlenA(lpModuleName)) == CSTR_EQUAL)
	{
		i = 1;
	}

	if ( CompareStringA(LOCALE_SYSTEM_DEFAULT, LINGUISTIC_IGNORECASE, "isxstealth.dll", lstrlenA("isxstealth.dll"), lpModuleName, lstrlenA(lpModuleName)) == CSTR_EQUAL)
	{
		i = 1;
	}

	if( i >= 0 )
	{
		printf( "Blocking GetModuleHandleA(%s) call.", lpModuleName );
		SetLastError(ERROR_ACCESS_DENIED);
	}
	else
	{
		rval = GetModuleHandleATrampoline(lpModuleName);
	}

	return rval;
}



DETOUR_TRAMPOLINE_EMPTY(HMODULE WINAPI GetModuleHandleWTrampoline( LPCWSTR lpModuleName ));
HMODULE WINAPI GetModuleHandleWDetour( LPCWSTR lpModuleName )
{
	HMODULE rval = 0;

	vector<PStealthModule> modules = pExtension->modules;

	int i;
	for( i = modules.size() - 1; i >= 0; i-- ) 
	{
		PStealthModule stealthModule = modules[i];
		if( CompareStringW(LOCALE_SYSTEM_DEFAULT,LINGUISTIC_IGNORECASE,stealthModule->w_name,
			lstrlenW(stealthModule->w_name),lpModuleName,lstrlenW(lpModuleName)) == CSTR_EQUAL )
		{
			break;
		}
	}

	if( i >= 0 )
	{
		printf( "Blocking GetModuleHandleW(%ls) call.", lpModuleName );
		SetLastError(ERROR_ACCESS_DENIED);
	}
	else
	{
		rval = GetModuleHandleWTrampoline(lpModuleName);
	}

	return rval;
}


typedef HMODULE (WINAPI *tLoadLibraryA)( LPCSTR lpLibFileName ); 

HMODULE WINAPI LoadLibraryAHook( LPCSTR lpLibFileName, tLoadLibraryA tramp )
{
	HMODULE rval = 0;

	vector<PStealthModule> modules = pExtension->modules;

	int i;
	for( i = modules.size() - 1; i >= 0; i-- ) 
	{
		PStealthModule stealthModule = modules[i];
		if( CompareStringA(LOCALE_SYSTEM_DEFAULT,LINGUISTIC_IGNORECASE,stealthModule->name,
			lstrlenA(stealthModule->name),lpLibFileName,lstrlenA(lpLibFileName)) == CSTR_EQUAL )
		{
			break;
		}
	}

	if ( CompareStringA(LOCALE_SYSTEM_DEFAULT, LINGUISTIC_IGNORECASE, "directeve.dll", lstrlenA("directeve.dll"), lpLibFileName, lstrlenA(lpLibFileName)) == CSTR_EQUAL)
	{
		i = 1;
	}

	if ( CompareStringA(LOCALE_SYSTEM_DEFAULT, LINGUISTIC_IGNORECASE, "isxstealth.dll", lstrlenA("isxstealth.dll"), lpLibFileName, lstrlenA(lpLibFileName)) == CSTR_EQUAL)
	{
		i = 1;
	}

	if( i >= 0 )
	{
		printf( "Blocking LoadLibraryA(%s) call.", lpLibFileName );
		SetLastError(ERROR_ACCESS_DENIED);
	}
	else
	{
		rval = tramp(lpLibFileName);
	}

	return rval;
}

DETOUR_TRAMPOLINE_EMPTY(HMODULE WINAPI LoadLibraryATrampoline( LPCSTR lpLibFileName ));
HMODULE WINAPI LoadLibraryADetour( LPCSTR lpLibFileName )
{
	return LoadLibraryAHook(lpLibFileName,LoadLibraryATrampoline);
}

DETOUR_TRAMPOLINE_EMPTY(HMODULE WINAPI LoadLibraryATrampoline_2( LPCSTR lpLibFileName ));
HMODULE WINAPI LoadLibraryADetour_2( LPCSTR lpLibFileName )
{
	return LoadLibraryAHook(lpLibFileName,LoadLibraryATrampoline_2);
}

DETOUR_TRAMPOLINE_EMPTY(HMODULE WINAPI LoadLibraryWTrampoline( LPCWSTR lpLibFileName ));
HMODULE WINAPI LoadLibraryWDetour( LPCWSTR lpLibFileName )
{
	HMODULE rval = 0;

	vector<PStealthModule> modules = pExtension->modules;

	int i;
	for( i = modules.size() - 1; i >= 0; i-- ) 
	{
		PStealthModule stealthModule = modules[i];
		if( CompareStringW(LOCALE_SYSTEM_DEFAULT,LINGUISTIC_IGNORECASE,stealthModule->w_name,
			lstrlenW(stealthModule->w_name),lpLibFileName,lstrlenW(lpLibFileName)) == CSTR_EQUAL )
		{
			break;
		}
	}

	if( i >= 0 )
	{
		printf( "Blocking LoadLibraryW(%ls) call.", lpLibFileName );
		SetLastError(ERROR_ACCESS_DENIED);
	}
	else
	{
		rval = LoadLibraryWTrampoline(lpLibFileName);
	}

	return rval;
}

DETOUR_TRAMPOLINE_EMPTY(HMODULE WINAPI LoadLibraryExATrampoline( LPCSTR lpLibFileName, HANDLE hFile, DWORD dwFlags ));
HMODULE WINAPI LoadLibraryExADetour( LPCSTR lpLibFileName, HANDLE hFile, DWORD dwFlags )
{
	HMODULE rval = 0;

	vector<PStealthModule> modules = pExtension->modules;

	int i;
	for( i = modules.size() - 1; i >= 0; i-- ) 
	{
		PStealthModule stealthModule = modules[i];
		if( CompareStringA(LOCALE_SYSTEM_DEFAULT,LINGUISTIC_IGNORECASE,stealthModule->name,
			lstrlenA(stealthModule->name),lpLibFileName,lstrlenA(lpLibFileName)) == CSTR_EQUAL )
		{
			break;
		}
	}

	if( i >= 0 )
	{
		printf( "Blocking LoadLibraryExA(%s) call.", lpLibFileName );
		SetLastError(ERROR_ACCESS_DENIED);
	}
	else
	{
		rval = LoadLibraryExATrampoline(lpLibFileName, hFile, dwFlags);
	}

	return rval;
}

DETOUR_TRAMPOLINE_EMPTY(HMODULE WINAPI LoadLibraryExWTrampoline( LPCWSTR lpLibFileName, HANDLE hFile, DWORD dwFlags ));
HMODULE WINAPI LoadLibraryExWDetour( LPCWSTR lpLibFileName, HANDLE hFile, DWORD dwFlags )
{
	HMODULE rval = 0;

	vector<PStealthModule> modules = pExtension->modules;

	int i;
	for( i = modules.size() - 1; i >= 0; i-- ) 
	{
		PStealthModule stealthModule = modules[i];
		if( CompareStringW(LOCALE_SYSTEM_DEFAULT,LINGUISTIC_IGNORECASE,stealthModule->w_name,
			lstrlenW(stealthModule->w_name),lpLibFileName,lstrlenW(lpLibFileName)) == CSTR_EQUAL )
		{
			break;
		}
	}

	if( i >= 0 )
	{
		printf( "Blocking LoadLibraryExW(%ls) call.", lpLibFileName );
		SetLastError(ERROR_ACCESS_DENIED);
	}
	else
	{
		rval = LoadLibraryExWTrampoline(lpLibFileName, hFile, dwFlags);
	}

	return rval;
}

DETOUR_TRAMPOLINE_EMPTY(FARPROC WINAPI GetProcAddressTrampoline( HMODULE hModule, LPCSTR lpProcName ));
FARPROC WINAPI GetProcAddressDetour( HMODULE hModule, LPCSTR lpProcName )
{
	FARPROC rval = 0;

	//printf( "GetProcAddress('%s') call.", lpProcName );
	if( CompareStringA(LOCALE_SYSTEM_DEFAULT,LINGUISTIC_IGNORECASE,"LoadLibraryA",
		lstrlenA("LoadLibraryA"),lpProcName,lstrlenA(lpProcName)) == CSTR_EQUAL )
	{
		printf( "Blocking GetProcAddress('LoadLibraryA') call." );
		rval = GetProcAddressTrampoline(hModule, lpProcName);
	}
	else
	{
		rval = GetProcAddressTrampoline(hModule, lpProcName);
	}

	return rval;
}



DETOUR_TRAMPOLINE_EMPTY(BOOL WINAPI IsDebuggerPresentTrampoline( VOID ));
BOOL WINAPI IsDebuggerPresentDetour( VOID )
{
	printf( "Blocking IsDebuggerPresent() call." );
	return FALSE;
}

DETOUR_TRAMPOLINE_EMPTY(BOOL WINAPI IsDebuggerPresentTrampoline_2( VOID ));
BOOL WINAPI IsDebuggerPresentDetour_2( VOID )
{
	printf( "Blocking IsDebuggerPresent() call." );
	return FALSE;
}

DETOUR_TRAMPOLINE_EMPTY(BOOL WINAPI CheckRemoteDebuggerPresentTrampoline( HANDLE hProcess, PBOOL pbDebuggerPresent ));
BOOL WINAPI CheckRemoteDebuggerPresentDetour( HANDLE hProcess, PBOOL pbDebuggerPresent )
{
	BOOL rval = FALSE;

	if( hProcess == GetCurrentProcess() && pbDebuggerPresent != NULL )
	{
		printf( "Blocking CheckRemoteDebuggerPresent() call." );
		*pbDebuggerPresent = FALSE;
		rval = TRUE;
	}
	else
	{
		SetLastError(ERROR_INVALID_PARAMETER);
	}

	return rval;
}

DETOUR_TRAMPOLINE_EMPTY(NTSTATUS NTAPI LdrGetDllHandleTrampoline( PWORD pwPath, PVOID Unused, PUNICODE_STRING ModuleFileName, PHANDLE pHModule ));
NTSTATUS NTAPI LdrGetDllHandleDetour( PWORD pwPath, PVOID Unused, PUNICODE_STRING ModuleFileName, PHANDLE pHModule )
{
	NTSTATUS rval = 0;

	vector<PStealthModule> modules = pExtension->modules;

	int i;
	for( i = modules.size() - 1; i >= 0; i-- ) 
	{
		PStealthModule stealthModule = modules[i];
		if( CompareStringW(LOCALE_SYSTEM_DEFAULT,LINGUISTIC_IGNORECASE,stealthModule->w_name,
			lstrlenW(stealthModule->w_name),ModuleFileName->Buffer,ModuleFileName->Length) == CSTR_EQUAL )
		{
			break;
		}
	}

	if( i >= 0 )
	{
		printf( "Blocking GetDllHandle(%ls) call.", ModuleFileName->Buffer );
		SetLastError(ERROR_ACCESS_DENIED);
	}
	else
	{
		rval = LdrGetDllHandleTrampoline(pwPath, Unused, ModuleFileName, pHModule);
	}

	return rval;
}

NTSTATUS NTAPI LdrLoadDll( IN PWCHAR PathToFile OPTIONAL, IN ULONG Flags OPTIONAL, IN PUNICODE_STRING ModuleFileName, OUT PHANDLE ModuleHandle ); 

DETOUR_TRAMPOLINE_EMPTY(NTSTATUS NTAPI LdrLoadDllTrampoline( PWCHAR PathToFile, ULONG Flags, PUNICODE_STRING ModuleFileName, PHANDLE ModuleHandle ));
NTSTATUS NTAPI LdrLoadDllDetour( PWCHAR PathToFile, ULONG Flags, PUNICODE_STRING ModuleFileName, PHANDLE ModuleHandle )
{
	NTSTATUS rval = 0;

	vector<PStealthModule> modules = pExtension->modules;

	int i;
	for( i = modules.size() - 1; i >= 0; i-- ) 
	{
		PStealthModule stealthModule = modules[i];
		if( CompareStringW(LOCALE_SYSTEM_DEFAULT,LINGUISTIC_IGNORECASE,stealthModule->w_name,
			lstrlenW(stealthModule->w_name),ModuleFileName->Buffer,ModuleFileName->Length) == CSTR_EQUAL )
		{
			break;
		}
	}

	if( i >= 0 )
	{
		printf( "Blocking LdrLoadDll(%ls) call.", ModuleFileName->Buffer );
		SetLastError(ERROR_ACCESS_DENIED);
	}
	else
	{
		rval = LdrLoadDllTrampoline(PathToFile, Flags, ModuleFileName, ModuleHandle);
	}

	return rval;
}

typedef NTSTATUS (NTAPI *tLdrGetDllHandle)(IN PWORD pwPath OPTIONAL, IN PVOID Unused OPTIONAL, IN PUNICODE_STRING ModuleFileName, OUT PHANDLE pHModule ); 
typedef NTSTATUS (NTAPI *tLdrGetProcedureAddress)( IN HMODULE ModuleHandle, IN PANSI_STRING FunctionName OPTIONAL, IN WORD Oridinal OPTIONAL, OUT PVOID *FunctionAddress ); 

void *GetThunk(char *mod_name_1, char *mod_name_2, char *func_name)
{
	void * rval = NULL;

    HANDLE hMod = GetModuleHandleA(mod_name_1);
    if(hMod != NULL)
    {
        IMAGE_DOS_HEADER *IDH = (IMAGE_DOS_HEADER*)(hMod);
        if(IDH->e_magic == IMAGE_DOS_SIGNATURE)
        {
            IMAGE_NT_HEADERS* INH = (IMAGE_NT_HEADERS*)((char *)hMod + IDH->e_lfanew);
            if(INH->Signature == IMAGE_NT_SIGNATURE)
            {
                if(INH->OptionalHeader.DataDirectory[1].VirtualAddress != 0)
                {
                    IMAGE_IMPORT_DESCRIPTOR * IID = (IMAGE_IMPORT_DESCRIPTOR*)((char *)hMod + INH->OptionalHeader.DataDirectory[1].VirtualAddress);
                    while( IID->Name != 0 )
                    {
                        if( stricmp( mod_name_2, (char *)hMod + IID->Name) == 0 )
                        {
                            IMAGE_THUNK_DATA * ITD = (IMAGE_THUNK_DATA*)((char *)hMod + IID->FirstThunk);
                            IMAGE_THUNK_DATA * OITD = (IMAGE_THUNK_DATA*)((char *)hMod + IID->OriginalFirstThunk);
                            while( ITD != 0 )
                            {
                                IMAGE_IMPORT_BY_NAME * IIN = (IMAGE_IMPORT_BY_NAME*)((char *)hMod + OITD->u1.AddressOfData);
                                if( stricmp( func_name, (char*)(IIN->Name) ) == 0 )
                                {
                                    rval = (void *)ITD->u1.Function;
									break;
                                }
                                OITD++;
                                ITD++;
                            }
                        }
                        IID++;
                    }
                }
            }
        }
    }

    return rval;
}


void ISXStealth::InitializeHooks()
{
	void *func1, *func2;

	//LoadLibraryA
	func1 = GetProcAddress(GetModuleHandleA("kernel32.dll"), "LoadLibraryA");
	func2 = GetThunk("_ctypes.pyd","kernel32.dll","LoadLibraryA");
	EzDetour(func1, LoadLibraryADetour, LoadLibraryATrampoline);
	if( func1 != func2 && func2 != NULL )
	{
		EzDetour(func2, LoadLibraryADetour_2, LoadLibraryATrampoline_2);
	}

	//LoadLibraryW ~ This one doesnt catch it but allready returns a 0 handle from itself
	func1 = GetProcAddress(GetModuleHandleA("kernel32.dll"), "LoadLibraryW");
	func2 = GetThunk("exefile.exe","kernel32.dll","LoadLibraryW");
	if( func1 != func2 && func2 != NULL )
	{
		EzDetour(func2, LoadLibraryWDetour, LoadLibraryWTrampoline);
	}

	//LoadLibraryExA and W not available in exefile.exe

	//GetModuleHandleA
	func1 = GetProcAddress(GetModuleHandleA("kernel32.dll"), "GetModuleHandleA");
	func2 = GetThunk("exefile.exe","kernel32.dll","GetModuleHandleA");
	if( func1 != func2 && func2 != NULL )
	{
		EzDetour(func2, GetModuleHandleADetour, GetModuleHandleATrampoline);
	}

	
	//GetModuleHandleW ~ This one doesnt catch it but allready returns a 0 handle from itself
	func1 = GetProcAddress(GetModuleHandleA("kernel32.dll"), "GetModuleHandleW");
	func2 = GetThunk("exefile.exe","kernel32.dll","GetModuleHandleW");
	if( func1 != func2 && func2 != NULL )
	{
		EzDetour(func2, GetModuleHandleWDetour, GetModuleHandleWTrampoline);
	}

	//IsDebuggerPresent
	func1 = GetProcAddress(GetModuleHandleA("kernel32.dll"), "IsDebuggerPresent");
	func2 = GetThunk("_ctypes.pyd","kernel32.dll","IsDebuggerPresent");
	EzDetour(func1, IsDebuggerPresentDetour, IsDebuggerPresentTrampoline);
	if( func1 != func2 && func2 != NULL )
	{
		EzDetour(func2, IsDebuggerPresentDetour_2, IsDebuggerPresentDetour_2);
	}

	//Still have to hook GetProcAddress, but not sure what intentions are of this detour
	//EzDetour(GetProcAddress(GetModuleHandleA("kernel32.dll"), "GetProcAddress"), GetProcAddressDetour, GetProcAddressTrampoline);

	//CheckRemoteDebuggerPresent ~ Also not loaded into client so no iat hooking into client
	EzDetour(GetProcAddress(GetModuleHandleA("kernel32.dll"), "CheckRemoteDebuggerPresent"), CheckRemoteDebuggerPresentDetour, CheckRemoteDebuggerPresentTrampoline);

	//Ldr ~ also not available in client so no iat hooking here either
	EzDetour(GetProcAddress(GetModuleHandle("ntdll.dll"),"LdrGetDllHandle"), LdrGetDllHandleDetour, LdrGetDllHandleTrampoline);
	EzDetour(GetProcAddress(GetModuleHandle("ntdll.dll"),"LdrLoadDll"), LdrLoadDllDetour, LdrLoadDllTrampoline);

}

void ISXStealth::RemoveHooks()
{
	EzUnDetour(GetProcAddress(GetModuleHandleA("kernel32.dll"), "LoadLibraryA"));
	EzUnDetour(GetProcAddress(GetModuleHandleA("kernel32.dll"), "LoadLibraryW"));
	EzUnDetour(GetProcAddress(GetModuleHandleA("kernel32.dll"), "LoadLibraryExA"));
	EzUnDetour(GetProcAddress(GetModuleHandleA("kernel32.dll"), "LoadLibraryExW"));
	//EzUnDetour(GetProcAddress(GetModuleHandleA("kernel32.dll"), "GetProcAddress"));
	EzUnDetour(GetProcAddress(GetModuleHandleA("kernel32.dll"), "GetModuleHandleA"));
	EzUnDetour(GetProcAddress(GetModuleHandleA("kernel32.dll"), "GetModuleHandleW"));
	EzUnDetour(GetProcAddress(GetModuleHandleA("kernel32.dll"), "IsDebuggerPresent"));
	EzUnDetour(GetProcAddress(GetModuleHandleA("kernel32.dll"), "CheckRemoteDebuggerPresent"));
}


// shutdown sequence
void ISXStealth::Shutdown()
{
	// Remove LavishScript extensions (commands, aliases, data types, objects)
	UnRegisterCommands();

	// Remove our hooks
	RemoveHooks();

	// gracefully disconnect from services
	if (hMemoryService)
	{
		pISInterface->DisconnectService(this,hMemoryService);
		hMemoryService=0;
	}

	for(int i = modules.size() -1; i >= 0; i --)
		UnstealthModule(modules[i]);
}

/*
 * Note that Initialize and Shutdown are the only two REQUIRED functions in your ISXInterface class.
 * All others are for suggested breakdown of routines, and for example purposes.
 */
void ISXStealth::RegisterExtension()
{
	// add this extension to, or update this extension's info in, InnerSpace.xml.
	// This accomplishes a few things.  A) The extension can be loaded by name (ISXStealth)
	// no matter where it resides on the system.  B) A script or extension can
	// check a repository to determine if there is an update available (and update
	// if necessary)

	unsigned int ExtensionSetGUID=pISInterface->GetExtensionSetGUID("ISXStealth");
	if (!ExtensionSetGUID)
	{
		ExtensionSetGUID=pISInterface->CreateExtensionSet("ISXStealth");
		if (!ExtensionSetGUID)
			return;
	}
	pISInterface->SetSetting(ExtensionSetGUID,"Filename",ModuleFileName);
	pISInterface->SetSetting(ExtensionSetGUID,"Path",ModulePath);
	pISInterface->SetSetting(ExtensionSetGUID,"Version",Stealth_Version);
}

void ISXStealth::RegisterCommands()
{
	// add any commands
	//	pISInterface->AddCommand("ISXStealth",CMD_ISXStealth,true,false);
#define COMMAND(name,cmd,parse,hide) pISInterface->AddCommand(name,cmd,parse,hide);
#include "Commands.h"
#undef COMMAND
}

void ISXStealth::UnRegisterCommands()
{
	// remove commands
	//	pISInterface->RemoveCommand("ISXStealth");
#define COMMAND(name,cmd,parse,hide) pISInterface->RemoveCommand(name);
#include "Commands.h"
#undef COMMAND
}

void ISXStealth::UnitTest()
{
	GetModuleHandleA("iSxStEaLtH.DlL");
	GetModuleHandleW(L"iSxStEaLtH.DlL");
	
	LoadLibraryA("iSxStEaLtH.DlL");
	LoadLibraryW(L"iSxStEaLtH.DlL");

	IsDebuggerPresent();

	BOOL bVal;
	CheckRemoteDebuggerPresent(GetCurrentProcess(),&bVal);
}

void __cdecl MemoryService(bool Broadcast, unsigned int MSG, void *lpData)
{
	// no messages are currently associated with this service (other than
	// system messages such as client disconnect), so do nothing.
}