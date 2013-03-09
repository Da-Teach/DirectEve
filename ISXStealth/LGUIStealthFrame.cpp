#include "ISXStealth.h"
#include "LGUIStealthFrame.h"

LGUIFactory<LGUIStealthFrame> StealthFrameFactory("stealthframe");

LGUIStealthFrame::LGUIStealthFrame(char *p_Factory, LGUIElement *p_pParent, char *p_Name):LGUIFrame(p_Factory,p_pParent,p_Name)
{
	pText=0;
	Count=0;
}
LGUIStealthFrame::~LGUIStealthFrame(void)
{
}
bool LGUIStealthFrame::IsTypeOf(char *TestFactory)
{
	return (!stricmp(TestFactory,"stealthframe")) || LGUIFrame::IsTypeOf(TestFactory);
}
bool LGUIStealthFrame::FromXML(class XMLNode *pXML, class XMLNode *pTemplate)
{
	if (!pTemplate)
		pTemplate=g_UIManager.FindTemplate(XMLHelper::GetStringAttribute(pXML,"Template"));
	if (!pTemplate)
		pTemplate=g_UIManager.FindTemplate("stealthframe");
	if (!LGUIFrame::FromXML(pXML,pTemplate))
		return false;

	// custom xml properties
	return true;
}

void LGUIStealthFrame::OnCreate()
{
	// All children of this element are guaranteed to have been created now.
	pText = (LGUIText*)FindUsableChild("Output","text");
}

void LGUIStealthFrame::Render()
{
	Count++;
	if (pText)
	{
		char Temp[256];
		sprintf(Temp,"This frame has been rendered %d times.",Count);
		pText->SetText(Temp);
	}

	LGUIFrame::Render();
}


