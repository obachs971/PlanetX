using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjacentNote : NoteObj 
{
	public AdjacentNote(char p1, char p2, char numType, int num) : base(p1 + "" + p2 + "" + numType + "" + num + "A")
	{

	}
	
}
