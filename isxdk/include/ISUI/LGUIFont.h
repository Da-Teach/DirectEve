#pragma once

struct _CreateFont
{
	char *Name;
	unsigned char Height;
	unsigned int Color;
	bool Bold;
	bool Fixed;
};

class LGUIFont
{
public:
	LGUIFont(LGUIElement *pParent, char *Face, unsigned char Height, bool Fixed, bool Bold);
	LGUIFont(LGUIElement *pParent, const LGUIFont&);
	LGUIFont(LGUIElement *pParent, _CreateFont &CreateFont, XMLNode *pTemplate);
	virtual ~LGUIFont();

//	LGUI_API virtual bool FromXML(class XMLNode *pXML, char *Template="");

	LGUI_API static LGUIFont *New(LGUIElement *pParent,char *Face, unsigned char Height, bool Fixed, bool Bold);
	LGUI_API static LGUIFont *New(LGUIElement *pParent,_CreateFont &CreateFont, class XMLNode *pTemplate=0);
	LGUI_API static LGUIFont *New(LGUIElement *pParent,const LGUIFont&);
	LGUI_API virtual void Delete();
	LGUI_API virtual bool Prepare();
	LGUI_API void Release();

	LGUI_API bool IsFixedFont();
//	LGUI_API float GetBaseHeight();
	LGUI_API unsigned int Draw(const char *Text,int X, int Y,unsigned int ClipLength=0);
	LGUI_API unsigned int DrawCenter(const char *Text,int X, int Y,unsigned int ClipLength);
	LGUI_API unsigned int DrawRight(const char *Text,int X, int Y,unsigned int ClipLength);
	LGUI_API unsigned int GetTextWidth(const char *Text);
	LGUI_API unsigned int GetTextWidth(const char *Text, unsigned int Length);
//	LGUI_API unsigned int GetCharWidth(unsigned int c);
	LGUI_API unsigned int GetCharByOffset(const char *Text, unsigned int Offset);

	LGUI_API void SetName(char *p_Name);
	LGUI_API void SetBold(bool Bold);

	inline char *GetName() {return Name;}
	inline unsigned int GetColor() {return Color;}
	inline unsigned char GetHeight() {return Height;}
	inline bool GetBold() {return Bold;}

	LGUI_API void SetHeight(unsigned char NewHeight);
	inline void SetColor(unsigned int NewColor) {Color=NewColor;}

protected:
	unsigned int FontID;

	char *Name;
	unsigned char Height;
	unsigned int Color;
	bool Bold;
	bool Fixed;

	LGUIElement *pParent;
};

class LGUIFixedFont : public LGUIFont
{
public:
	LGUIFixedFont(LGUIElement *pParent,char *Face, unsigned char Height, bool Bold);
	LGUIFixedFont(LGUIElement *pParent,const LGUIFixedFont&);
	LGUIFixedFont(LGUIElement *pParent,_CreateFont &CreateFont, class XMLNode *pTemplate);
	~LGUIFixedFont();

	LGUI_API static LGUIFixedFont *New(LGUIElement *pParent,char *Face, unsigned char Height, bool Bold);
	LGUI_API static LGUIFixedFont *New(LGUIElement *pParent,const LGUIFixedFont&);
	LGUI_API static LGUIFixedFont *New(LGUIElement *pParent,_CreateFont &CreateFont, class XMLNode *pTemplate=0);
	LGUI_API virtual void Delete();

	LGUI_API virtual bool Prepare();
	LGUI_API unsigned int GetCharWidth();
/*
protected:
	inline class CLavishFontFixed *GetFixedFont()
	{
		return (class CLavishFontFixed *)pFont;
	}
/**/
};
