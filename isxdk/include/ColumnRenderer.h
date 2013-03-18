#pragma once
class CColumnRenderer;

#ifndef __CColumnRenderer_h__
#define __CColumnRenderer_h__

struct _ColumnRendererItem
{
	char *Text;
	unsigned int Length;
};

class CColumnRenderer
{
public:
	CColumnRenderer()
	{
		LongestItem=0;
		ItemCount=0;
	}

	~CColumnRenderer()
	{
		foreach(_ColumnRendererItem* pItem,i,Items)
		{
			free(pItem->Text);
		}
	}

	void AddItem(const char *Text)
	{
		if (!Text || !Text[0])
			return;
		_ColumnRendererItem *pItem = new _ColumnRendererItem;
		pItem->Text=strdup(Text);
		unsigned int NewLength=strlen(Text);
		if (const char *pColor=(const char *)strchr(Text,'\a'))
		{
			do
			{
				if (pColor[1]=='-')
					NewLength--;
				NewLength-=2;
			}
			while(pColor=strchr(&pColor[1],'\a'));
		}
		pItem->Length=NewLength;
		if (NewLength>LongestItem)
			LongestItem=NewLength;
		Items+=pItem;
		ItemCount++;
	}

	void Render(class ISInterface *pISInterface)
	{
		if (!LongestItem) 
			return; // wtf.
		unsigned int Width=pISInterface->GetTerminalWidth();
		unsigned int Columns=(Width-LongestItem)/LongestItem;
		unsigned int ColumnWidth;
		if (Columns<1)
		{
			Columns=1;
			ColumnWidth=Width;
		}
		else if (Columns>10)
		{
			Columns=10;
			ColumnWidth=Width/10;
		}
		else
			ColumnWidth=Width/Columns;
		char Row[1024];
		char *pColumn=Row;

		unsigned int Rows=(ItemCount/Columns)+1;

		unsigned int ItemNumber=0;

		//pISInterface->Printf("Items: %d. Rows: %d. Columns: %d.",ItemCount,Rows,Columns);
		for (unsigned int i = 0 ; i < Rows ; i++)
		{
			ItemNumber=i;
			for (unsigned int Column=0 ; Column < Columns ; Column++)
			{
				if (ItemNumber<ItemCount)
				{
					if (_ColumnRendererItem* pItem=Items[ItemNumber])
					{
						pColumn+=sprintf(pColumn,"%-*s",ColumnWidth,pItem->Text);
					}
				}
				
				ItemNumber+=Rows;
			}
			pISInterface->Printf("%s",Row);
			pColumn=Row;
			Row[0]=0;

		}
	}

	void RenderLeftToRight(class ISInterface *pISInterface)
	{
		if (!LongestItem) 
			return; // wtf.
		unsigned int Width=pISInterface->GetTerminalWidth();
		char Row[1024];
		unsigned int Columns=(Width-LongestItem)/LongestItem;
		unsigned int ColumnWidth;
		//pISInterface->Printf("Longest Item: %d. Width: %d. ColWidth: %d. Columns: %d.",LongestItem,Width,ColumnWidth,Columns);
		if (Columns<1)
		{
			Columns=1;
			ColumnWidth=Width;
		}
		else if (Columns>10)
		{
			Columns=10;
			ColumnWidth=Width/10;
		}
		else
			ColumnWidth=Width/Columns;
		unsigned int Column=0;
		char *pColumn=Row;

		foreach(_ColumnRendererItem* pItem,i,Items)
		{
			pColumn+=sprintf(pColumn,"%-*s",ColumnWidth,pItem->Text);
			Column++;
			if (Column>=Columns)
			{
				pISInterface->Printf("%s",Row);
				Column=0;
				pColumn=Row;
				Row[0]=0;
			}
		}
		if (Row[0])
			pISInterface->Printf("%s",Row);
	}


protected:
	unsigned int ItemCount;
	unsigned int LongestItem;
	CIndex<_ColumnRendererItem *> Items;
	unsigned int Widest;
};


#endif

