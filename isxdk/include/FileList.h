#pragma once
struct _FileListEntry
{
	char Filename[512];
	char FilenameOnly[128];
	FILETIME ftCreationTime;
	FILETIME ftLastWriteTime;
	FILETIME ftLastAccessTime;
	unsigned int FileSize;
};

class CFileList
{
public:
	CFileList() 
	{
		nFiles=0;
	}
	~CFileList()
	{
		List.Cleanup();
	}

	void AddFile(LPWIN32_FIND_DATA pFile, char *Path)
	{
		_FileListEntry *pNew  = new _FileListEntry;
		sprintf(pNew->Filename,"%s%s",Path,pFile->cFileName);
		strcpy(pNew->FilenameOnly,pFile->cFileName);
		pNew->ftCreationTime=pFile->ftCreationTime;
		pNew->ftLastAccessTime=pFile->ftLastAccessTime;
		pNew->ftLastWriteTime=pFile->ftLastWriteTime;
		pNew->FileSize=pFile->nFileSizeLow;
		List+=pNew;
		nFiles++;
	}
	char *GetPath(char *Wildcard)
	{
		static char Path[512];
		if (char *pSlash=strrchr(Wildcard,'\\'))
		{
			strcpy(Path,Wildcard);
			Path[(pSlash-Wildcard)+1]=0;
		}
		else
			Path[0]=0;
		return Path;
	}
	unsigned int EnumDirectories(char *Wildcard)
	{
		WIN32_FIND_DATA file;
		HANDLE hSearch=FindFirstFile(Wildcard,&file);
		if (hSearch==INVALID_HANDLE_VALUE)
			return 0;
		char *Path=GetPath(Wildcard);
		do
		{
			if (_stricmp(file.cFileName,".") && _stricmp(file.cFileName,".."))
			if (file.dwFileAttributes&FILE_ATTRIBUTE_DIRECTORY)
				AddFile(&file,Path);
		} while (FindNextFile(hSearch,&file));

		FindClose(hSearch);
		return nFiles;
	}
	unsigned int EnumFiles(char *Wildcard)
	{
		WIN32_FIND_DATA file;
		HANDLE hSearch=FindFirstFile(Wildcard,&file);
		if (hSearch==INVALID_HANDLE_VALUE)
			return 0;
		char *Path=GetPath(Wildcard);
		do
		{
			if (!(file.dwFileAttributes&FILE_ATTRIBUTE_DIRECTORY))
				AddFile(&file,Path);
		} while (FindNextFile(hSearch,&file));

		FindClose(hSearch);
		return nFiles;
	}
	unsigned int EnumFilesAfter(FILETIME &filetime, char *Wildcard)
	{
		WIN32_FIND_DATA file;
		HANDLE hSearch=FindFirstFile(Wildcard,&file);
		if (hSearch==INVALID_HANDLE_VALUE)
			return 0;
		char *Path=GetPath(Wildcard);
		do
		{
			if (!(file.dwFileAttributes&FILE_ATTRIBUTE_DIRECTORY) && 
				CompareFileTime(&file.ftLastWriteTime,&filetime)>0)
			{
					AddFile(&file,Path);
			}
		} while (FindNextFile(hSearch,&file));

		FindClose(hSearch);
		return nFiles;
	}

	unsigned int nFiles;
	CIndex <_FileListEntry *> List;
};
