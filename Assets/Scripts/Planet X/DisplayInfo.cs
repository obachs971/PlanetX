using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayInfo 
{
	private string text;
	private int matIndex;
	public DisplayInfo(string text, int matIndex)
	{
		this.text = text;
		this.matIndex = matIndex;
	}
	public string getText()
	{
		return text;
	}
	public int getMatIndex()
	{
		return matIndex;
	}
	
}
