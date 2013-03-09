#include "ISXStealth.h"

DWORD GetKPROCESS()
{
	DWORD dwKProcess = 0;

	// Return PEB address for current process
	// address is located at FS:0x30
	__asm 
	{
		push eax
		mov eax, FS:[0x124]
		mov eax, [eax+0x50]
		mov [dwKProcess], eax
		pop eax
	}

	return (DWORD)dwKProcess;
}

int CMD_ThreadWalk(int argc, char *argv[])
{	
	DWORD eThread = GetKPROCESS();
	printf("0x%08x", eThread);
	return 0;
}

int CMD_StealthModule(int argc, char *argv[])
{
	if (argc == 1 || argc > 3)
	{
		printf("Syntax: StealthModule -list|-unstealth <module>");
		return -1;
	}

	bool list = false;
	bool unstealth = false;
	char *module = NULL;
	// Skip the first arg (it's "StealthModule")
	for(int i = 1; i < argc; i ++)
	{
		if (strcmp(argv[i], "-list") == 0)
			list = true;
		else if (strcmp(argv[i], "-unstealth") == 0)
			unstealth = true;
		else
			module = argv[i];
	}

	if (list)
	{
		pExtension->ListModules();
	}
	else if (module != NULL && !unstealth)
	{
		pExtension->StealthModule(module);
	}
	else if (module != NULL && unstealth)
	{
		pExtension->UnstealthModule(module);
	}

	return 0;
}