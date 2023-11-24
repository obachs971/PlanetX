using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WithinNote : NoteObj 
{
	public WithinNote(char p1, char p2, int w, char numType, int num) : base(p1 + "" + p2 + "" + w + "" + numType + "" + num + "W")
	{

	}
	
}
