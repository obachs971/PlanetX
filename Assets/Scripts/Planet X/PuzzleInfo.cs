using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleInfo 
{
	private string board;
	private List<NoteObj> notes;
	public PuzzleInfo(string board, List<NoteObj> notes)
	{
		this.board = board;
		this.notes = notes;
	}
	public string getBoard()
	{
		return board;
	}
	public List<NoteObj> getNotes()
	{
		return notes;
	}
}
