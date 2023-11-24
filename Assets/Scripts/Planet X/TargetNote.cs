using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetNote : NoteObj
{
	public TargetNote(string sector, char planet, char result) : base(sector + planet + "" + result + "T")
	{

	}
}
