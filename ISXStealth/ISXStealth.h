#pragma once
#include <isxdk.h>
#include <windows.h>
#include <vector>
#include "PEBStruct.h"

#define LOAD_ORDER_TYPE 0
#define MEM_ORDER_TYPE	1
#define INIT_ORDER_TYPE	2
#define LOWCASE(a) ((a>='A' && a<='Z')?a-('Z'-'z'):a)
#define BUFMAXLEN 512

typedef struct StealthModule
{
	char name[BUFMAXLEN];

	PLIST_ENTRY load_order;
	PLIST_ENTRY memory_order;
	PLIST_ENTRY init_order;

	DWORD image_base;
	DWORD image_size;
} StealthModule, _StealthModule, *PStealthModule;

class ISXStealth :
	public ISXInterface
{
private:
	DWORD GetPEB();
	int GetModuleList(char ModuleListType, PLIST_ENTRY *moduleListHead, DWORD *dwOffset);
	int WalkModuleList(char moduleListType, int (*processEntry)(char, PLIST_ENTRY, char*, DWORD, DWORD, DWORD*), DWORD *data);

public:
	ISXStealth(void);
	~ISXStealth(void);

	virtual bool Initialize(ISInterface *p_ISInterface);
	virtual void Shutdown();

	void RegisterExtension();

	void RegisterCommands();
	void UnRegisterCommands();

	void ListModules();
	void StealthModule(char *module);
	void UnstealthModule(char *module);
	void UnstealthModule(PStealthModule module);

	void BlockMiniDump(bool block);

	vector<PStealthModule> modules;
	bool blockMiniDump;
};

extern ISInterface *pISInterface;
extern ISXStealth *pExtension;
#define printf pISInterface->Printf

#define EzDetour(Address, Detour, Trampoline) IS_Detour(pExtension,pISInterface,hMemoryService,(unsigned int)Address,Detour,Trampoline)
#define EzUnDetour(Address) IS_UnDetour(pExtension,pISInterface,hMemoryService,(unsigned int)Address)
#define EzDetourAPI(_Detour_,_DLLName_,_FunctionName_,_FunctionOrdinal_) IS_DetourAPI(pExtension,pISInterface,hMemoryService,_Detour_,_DLLName_,_FunctionName_,_FunctionOrdinal_)
#define EzUnDetourAPI(Address) IS_UnDetourAPI(pExtension,pISInterface,hMemoryService,(unsigned int)Address)

#define EzModify(Address,NewData,Length,Reverse) Memory_Modify(pExtension,pISInterface,hMemoryService,(unsigned int)Address,NewData,Length,Reverse)
#define EzUnModify(Address) Memory_UnModify(pExtension,pISInterface,hMemoryService,(unsigned int)Address)

#define EzHttpRequest(_URL_,_pData_) IS_HttpRequest(pExtension,pISInterface,hHTTPService,_URL_,_pData_)

#define EzAddTrigger(Text,Callback,pUserData) IS_AddTrigger(pExtension,pISInterface,hTriggerService,Text,Callback,pUserData)
#define EzRemoveTrigger(ID) IS_RemoveTrigger(pExtension,pISInterface,hTriggerService,ID)
#define EzCheckTriggers(Text) IS_CheckTriggers(pExtension,pISInterface,hTriggerService,Text)

static LONG EzCrashFilter(_EXCEPTION_POINTERS *pExceptionInfo,const char *szIdentifier,...)
{
	unsigned int Code=pExceptionInfo->ExceptionRecord->ExceptionCode;
	if (Code==EXCEPTION_BREAKPOINT || Code==EXCEPTION_SINGLE_STEP)
		return EXCEPTION_CONTINUE_SEARCH;

	char szOutput[4096];
	szOutput[0]=0;
    va_list vaList;

    va_start( vaList, szIdentifier );
    vsprintf(szOutput,szIdentifier, vaList);

	IS_SystemCrashLog(pExtension,pISInterface,NULL,pExceptionInfo,szOutput);

	return EXCEPTION_EXECUTE_HANDLER;
}

extern char Stealth_Version[];

#include "Commands.h"