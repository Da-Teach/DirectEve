#include "ISXStealth.h"

int CMD_BlockMiniDump(int argc, char *argv[])
{
	if (argc != 2)
	{
		printf("Syntax: BlockMiniDump true|false");
		return -1;
	}

	pExtension->BlockMiniDump(strnicmp(argv[1], "true", 4) == 0);
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