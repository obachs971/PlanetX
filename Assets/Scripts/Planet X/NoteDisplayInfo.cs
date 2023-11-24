using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteDisplayInfo 
{
	private DisplayInfo[] displays;
	public NoteDisplayInfo(string code)
	{
		displays = new DisplayInfo[6];
		switch(code[code.Length - 1])
		{
			case 'S':
				displays[0] = new DisplayInfo("S", 0);
				displays[1] = new DisplayInfo(code.Substring(0, 2), 0);
				displays[2] = new DisplayInfo(code.Substring(2, 2), 0);
				displays[3] = new DisplayInfo("", getMatIndex(code[4]));
				displays[4] = new DisplayInfo("E", 0);
				displays[5] = new DisplayInfo(code.Substring(5, 1), 0);
				break;
			case 'A':
				displays[0] = new DisplayInfo("A", 0);
				displays[1] = new DisplayInfo("", getMatIndex(code[0]));
				displays[2] = new DisplayInfo("", getMatIndex(code[1]));
				displays[3] = new DisplayInfo("", 0);
				displays[4] = new DisplayInfo(code.Substring(2, 1), 0);
				displays[5] = new DisplayInfo(code.Substring(3, 1), 0);
				break;
			case 'O':
				displays[0] = new DisplayInfo("O", 0);
				displays[1] = new DisplayInfo("", getMatIndex(code[0]));
				displays[2] = new DisplayInfo("", getMatIndex(code[1]));
				displays[3] = new DisplayInfo("", 0);
				displays[4] = new DisplayInfo(code.Substring(2, 1), 0);
				displays[5] = new DisplayInfo(code.Substring(3, 1), 0);
				break;
			case 'W':
				displays[0] = new DisplayInfo("W", 0);
				displays[1] = new DisplayInfo("", getMatIndex(code[0]));
				displays[2] = new DisplayInfo("", getMatIndex(code[1]));
				displays[3] = new DisplayInfo(code.Substring(2, 1), 0);
				displays[4] = new DisplayInfo(code.Substring(3, 1), 0);
				displays[5] = new DisplayInfo(code.Substring(4, 1), 0);
				break;
			case 'B':
				displays[0] = new DisplayInfo("B", 0);
				displays[1] = new DisplayInfo("", getMatIndex(code[0]));
				displays[2] = new DisplayInfo("", 0);
				displays[3] = new DisplayInfo("", 0);
				displays[4] = new DisplayInfo("F", 0);
				displays[5] = new DisplayInfo(code.Substring(1, 1), 0);
				break;
		}
	}
	public DisplayInfo[] getDisplays()
	{
		return displays;
	}
	private int getMatIndex(char c)
	{
		switch(c)
		{
			case 'A': return 1;
			case 'C': return 2;
			case 'D': return 3;
			case 'G': return 4;
			case 'E': return 5;
			case 'X': return 6;
		}
		return -1;
	}
	
}
