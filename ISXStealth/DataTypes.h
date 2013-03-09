#pragma once
#include "DataTypeList.h"

// custom data type declarations 

class StealthType : public LSTypeDefinition
{
public:
	// All data members (retrieving data) should be listed in this enumeration
	enum StealthTypeMembers
	{
		RetrieveData,
	};
	// All data methods (performing actions on or with the object) should be listed in this enumeration
	enum StealthTypeMethods
	{
		PerformAction,
	};

	StealthType() : LSType("stealth")
	{
		// Use the TypeMember macro to activate each member, or use AddMember
		TypeMember(RetrieveData);

		// Use the TypeMethod macro to activate each member, or use AddMethod
		TypeMethod(PerformAction);
	}

	virtual bool GetMember(LSOBJECTDATA ObjectData, PLSTYPEMEMBER Member, int argc, char *argv[], LSOBJECT &Object);
	virtual bool GetMethod(LSOBJECTDATA &ObjectData, PLSTYPEMETHOD pMethod, int argc, char *argv[]);
	virtual bool ToText(LSOBJECTDATA ObjectData, char *buf, unsigned int buflen);
};
