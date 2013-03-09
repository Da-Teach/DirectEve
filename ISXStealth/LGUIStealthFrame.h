#pragma once

class LGUIStealthFrame :
	public LGUIFrame
{
public:
	LGUIStealthFrame(char *p_Factory, LGUIElement *p_pParent, char *p_Name);
	~LGUIStealthFrame(void);
	bool IsTypeOf(char *TestFactory);
	bool FromXML(class XMLNode *pXML, class XMLNode *pTemplate=0);
	void OnCreate();
	void Render();

	LGUIText *pText;
	unsigned int Count;
};

extern LGUIFactory<LGUIStealthFrame> StealthFrameFactory;

