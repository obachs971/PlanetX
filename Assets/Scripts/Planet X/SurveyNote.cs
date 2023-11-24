using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurveyNote : NoteObj
{
	public SurveyNote(string s1, string s2, char planet, int result) : base(s1 + s2 + planet + "" + result + "S")
	{

	}
	
}
