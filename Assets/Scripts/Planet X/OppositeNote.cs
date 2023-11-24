using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OppositeNote : NoteObj 
{
    public OppositeNote(char p1, char p2, char numType, int num) : base(p1 + "" + p2 + "" + numType + "" + num + "O")
    {

    }
}
