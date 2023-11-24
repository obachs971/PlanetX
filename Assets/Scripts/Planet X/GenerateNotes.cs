using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateNotes
{
	private static BoardInfo boardInfo = new BoardInfo();

	public List<NoteObj> generateAllNotes(string board)
	{
		List<NoteObj> otherNotes = new List<NoteObj>();
		List<NoteObj> surveyNotes = new List<NoteObj>();
		List<NoteObj> factNotes = new List<NoteObj>();
		surveyNotes.AddRange(generateSurveyNotes(board));
		factNotes.AddRange(generateFactNotes(board));
		otherNotes.AddRange(generateAdjacentNotes(board));
		otherNotes.AddRange(generateOppositeNotes(board));
		otherNotes.AddRange(generateWithinNotes(board));
		otherNotes.AddRange(generateBandNotes(board));

		factNotes.Shuffle();
		surveyNotes.Shuffle();
		otherNotes.Shuffle();
		List<NoteObj> notes = new List<NoteObj>();
		notes.AddRange(surveyNotes);
		notes.AddRange(otherNotes);
		notes.AddRange(factNotes);
		return notes;
	}
	public List<NoteObj> generateAllXNotes(string board)
	{
		List<NoteObj> notes = new List<NoteObj>();
		notes.AddRange(generateAdjacentXNotes(board));
		notes.AddRange(generateOppositeXNotes(board));
		notes.AddRange(generateWithinXNotes(board));
		return notes.Shuffle();
	}

	public List<NoteObj> generateTargetNotes(string b)
	{
		string board = b.Replace("X", "E");
		List<NoteObj> targetNotes = new List<NoteObj>();
		for (int i = 0; i < b.Length; i++)
		{
			string sector = (i + 1) + "";
			while (sector.Length < 2)
				sector = "0" + sector;
			targetNotes.Add(new TargetNote(sector, board[i], 'O'));
		}
		return targetNotes;
	}
	private List<NoteObj> generateFactNotes(string b)
	{
		string board = b.Replace("X", "E");
		List<NoteObj> factNotes = new List<NoteObj>();
		for (int i = 0; i < b.Length; i++)
		{
			string sector = (i + 1) + "";
			while (sector.Length < 2)
				sector = "0" + sector;
			string notPlanets = "CAGD".Replace(board[i] + "", "");
			factNotes.Add(new TargetNote(sector, notPlanets[Random.Range(0, notPlanets.Length)], 'X'));
		}
		return factNotes;
	}

	private List<NoteObj> generateSurveyNotes(string board)
	{
		List<NoteObj> surveyNotes = new List<NoteObj>();
		for (int i = 1; i <= board.Length; i++)
		{
			for(int j = i + 1; j < i + 6; j++)
			{
				foreach(char planet in "CAGDE")
				{
					int s2 = j >= 13 ? j - 12 : j;
					string[] sectors = new string[] { i + "", s2 + "" };
					for (int s = 0; s < sectors.Length; s++)
					{
						while (sectors[s].Length < 2)
							sectors[s] = "0" + sectors[s];
					}
					int sum = boardInfo.getSurveySum(board, i, j, planet, 'E');
					surveyNotes.Add(new SurveyNote(sectors[0], sectors[1], planet, sum));
				}
			}
		}
		return surveyNotes;
	}
	private List<NoteObj> generateAdjacentNotes(string board)
	{
		List<NoteObj> adjacentNotes = new List<NoteObj>();
		foreach(char p1 in "CAGD")
		{
			foreach (char p2 in "CAGD")
			{
				int sum = boardInfo.getAdjacentSum(board, p1, p2, 'E');
				int max = getMax(p1);
				adjacentNotes.Add(new AdjacentNote(p1, p2, 'E', sum));
				for(int i = 1; i < sum && i < max; i++)
					adjacentNotes.Add(new AdjacentNote(p1, p2, 'L', i));
				for(int i = max - 1; i > sum && i > 0; i--)
					adjacentNotes.Add(new AdjacentNote(p1, p2, 'F', i));
			}
		}
		return adjacentNotes;
	}
	private List<NoteObj> generateOppositeNotes(string board)
	{
		List<NoteObj> oppositeNotes = new List<NoteObj>();
		foreach (char p1 in "CAGD")
		{
			foreach (char p2 in "CAGD")
			{
				int sum = boardInfo.getOppositeSum(board, p1, p2, 'E');
				int max = getMax(p1);
				oppositeNotes.Add(new OppositeNote(p1, p2, 'E', sum));
				for (int i = 1; i < sum && i < max; i++)
					oppositeNotes.Add(new OppositeNote(p1, p2, 'L', i));
				for (int i = max - 1; i > sum && i > 0; i--)
					oppositeNotes.Add(new OppositeNote(p1, p2, 'F', i));
			}
		}
		return oppositeNotes;
	}
	private List<NoteObj> generateWithinNotes(string board)
	{
		List<NoteObj> withinNotes = new List<NoteObj>();
		foreach (char p1 in "CAGD")
		{
			foreach (char p2 in "CAGD")
			{
				for(int W = 2; W < 5; W++)
				{
					int sum = boardInfo.getWithinSum(board, p1, p2, W, 'E');
					int max = getMax(p1);
					withinNotes.Add(new WithinNote(p1, p2, W, 'E', sum));
					for (int i = 1; i < sum && i < max; i++)
						withinNotes.Add(new WithinNote(p1, p2, W, 'L', i));
					for (int i = max - 1; i > sum && i > 0; i--)
						withinNotes.Add(new WithinNote(p1, p2, W, 'F', i));
				}
			}
		}
		return withinNotes;
	}
	private List<NoteObj> generateBandNotes(string board)
	{
		List<NoteObj> bandNotes = new List<NoteObj>();
		foreach (char p in "CAG")
		{
			int sum = boardInfo.getSmallestBand(board, p, 'E');
			for(int i = sum; i <= 6; i++)
				bandNotes.Add(new BandNote(p, i));
		}
		return bandNotes;
	}

	private List<NoteObj> generateAdjacentXNotes(string board)
	{
		List<NoteObj> adjacentNotes = new List<NoteObj>();
		foreach (char p1 in "X")
		{
			foreach (char p2 in "CAG")
			{
				int sum = boardInfo.getAdjacentSum(board, p1, p2, 'X');
				int max = getMax(p1);
				adjacentNotes.Add(new AdjacentNote(p1, p2, 'E', sum));
				for (int i = 1; i < sum && i < max; i++)
					adjacentNotes.Add(new AdjacentNote(p1, p2, 'L', i));
				for (int i = max - 1; i > sum && i > 0; i--)
					adjacentNotes.Add(new AdjacentNote(p1, p2, 'F', i));
			}
		}
		foreach (char p1 in "CAG")
		{
			foreach (char p2 in "X")
			{
				int sum = boardInfo.getAdjacentSum(board, p1, p2, 'X');
				int max = getMax(p1);
				adjacentNotes.Add(new AdjacentNote(p1, p2, 'E', sum));
				for (int i = 1; i < sum && i < max; i++)
					adjacentNotes.Add(new AdjacentNote(p1, p2, 'L', i));
				for (int i = max - 1; i > sum && i > 0; i--)
					adjacentNotes.Add(new AdjacentNote(p1, p2, 'F', i));
			}
		}
		return adjacentNotes;
	}
	private List<NoteObj> generateOppositeXNotes(string board)
	{
		List<NoteObj> oppositeNotes = new List<NoteObj>();
		foreach (char p1 in "X")
		{
			foreach (char p2 in "CAGD")
			{
				int sum = boardInfo.getOppositeSum(board, p1, p2, 'X');
				int max = getMax(p1);
				oppositeNotes.Add(new OppositeNote(p1, p2, 'E', sum));
				for (int i = 1; i < sum && i < max; i++)
					oppositeNotes.Add(new OppositeNote(p1, p2, 'L', i));
				for (int i = max - 1; i > sum && i > 0; i--)
					oppositeNotes.Add(new OppositeNote(p1, p2, 'F', i));
			}
		}
		foreach (char p1 in "CAGD")
		{
			foreach (char p2 in "X")
			{
				int sum = boardInfo.getOppositeSum(board, p1, p2, 'X');
				int max = getMax(p1);
				oppositeNotes.Add(new OppositeNote(p1, p2, 'E', sum));
				for (int i = 1; i < sum && i < max; i++)
					oppositeNotes.Add(new OppositeNote(p1, p2, 'L', i));
				for (int i = max - 1; i > sum && i > 0; i--)
					oppositeNotes.Add(new OppositeNote(p1, p2, 'F', i));
			}
		}
		return oppositeNotes;
	}
	private List<NoteObj> generateWithinXNotes(string board)
	{
		List<NoteObj> withinNotes = new List<NoteObj>();
		foreach (char p1 in "X")
		{
			foreach (char p2 in "CAGD")
			{
				for (int W = 2; W < 5; W++)
				{
					int sum = boardInfo.getWithinSum(board, p1, p2, W, 'X');
					int max = getMax(p1);
					withinNotes.Add(new WithinNote(p1, p2, W, 'E', sum));
					for (int i = 1; i < sum && i < max; i++)
						withinNotes.Add(new WithinNote(p1, p2, W, 'L', i));
					for (int i = max - 1; i > sum && i > 0; i--)
						withinNotes.Add(new WithinNote(p1, p2, W, 'F', i));
				}
			}
		}
		foreach (char p1 in "CAGD")
		{
			foreach (char p2 in "X")
			{
				for (int W = 2; W < 5; W++)
				{
					int sum = boardInfo.getWithinSum(board, p1, p2, W, 'X');
					int max = getMax(p1);
					withinNotes.Add(new WithinNote(p1, p2, W, 'E', sum));
					for (int i = 1; i < sum && i < max; i++)
						withinNotes.Add(new WithinNote(p1, p2, W, 'L', i));
					for (int i = max - 1; i > sum && i > 0; i--)
						withinNotes.Add(new WithinNote(p1, p2, W, 'F', i));
				}
			}
		}
		return withinNotes;
	}

	private int getMax(char planet)
	{
		switch(planet)
		{
			case 'C': return 2;
			case 'A': return 4;
			case 'G': return 2;
			case 'D': return 1;
			case 'E': return 2;
			case 'X': return 1;
		}
		return -1;
	}
	
}
