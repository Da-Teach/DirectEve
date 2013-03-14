#ifndef COMMAND
#define COMMAND_SELF
#define COMMAND(name,cmd,parse,hide) extern int cmd(int argc, char *argv[])
#endif
// ----------------------------------------------------
// commands

COMMAND("StealthModule",CMD_StealthModule,true,false);
COMMAND("BlockMiniDump",CMD_BlockMiniDump,true,false);

// ----------------------------------------------------
#ifdef COMMAND_SELF
#undef COMMAND_SELF
#undef COMMAND
#endif