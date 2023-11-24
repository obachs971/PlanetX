using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle 
{
    private BoardInfo boardinfo = new BoardInfo();
    public PuzzleInfo generatePuzzle()
    {
        string board = new BoardConfig().getRandomBoardConfig();
        //Debug.LogFormat("BOARD: {0}", board);
        GenerateNotes gen = new GenerateNotes();
        List<NoteObj> notes = gen.generateAllNotes(board);
        HashSet<int> possX = getPossXPos(notes);
        if(possX.Count > 1)
        {
            List<NoteObj> notesX = gen.generateAllXNotes(board);
            notesX.Shuffle();
            tryagain:
            if (notesX.Count == 0)
                goto skip;
            notes.Add(notesX[0]);
            possX = getPossXPos(notes);
            if(possX.Count > 1)
            {
                notes.RemoveAt(notes.Count - 1);
                notesX.RemoveAt(0);
                goto tryagain;
            }
        }
        
        skip:
        if (possX.Count > 1)
            return null;
        for(int i = 0; i < notes.Count; i++)
        {
            NoteObj note = notes[i];
            notes.RemoveAt(i);
            possX = getPossXPos(notes);
            if (possX.Count > 1)
                notes.Insert(i, note);
            else
                i--;
        }
        return new PuzzleInfo(board, notes);
    }
    private HashSet<int> getPossXPos(List<NoteObj> notes)
    {
        List<string> boards = new BoardConfig().getAllValidConfigs();
        for(int i = 0; i < boards.Count; i++)
        {
            if (!isValidBoard(notes, boards[i]))
                boards.RemoveAt(i--);
        }
        HashSet<int> possX = new HashSet<int>();
        foreach (string board in boards)
            possX.Add(board.IndexOf("X"));
        return possX;
    }
    private bool isValidBoard(List<NoteObj> notes, string board)
    {
        foreach(NoteObj note in notes)
        {
            if (!notePassed(note, board))
                return false;
        }
        return true;
    }
    private bool isValidBoardTest(List<NoteObj> notes, string board)
    {
        foreach (NoteObj note in notes)
        {
            if (!notePassed(note, board))
                Debug.LogFormat("{0}", note.getCode());
        }
        return true;
    }
    private bool notePassed(NoteObj note, string board)
    {
        string code = note.getCode();
        char replace = code.Contains("X") ? 'X' : 'E';
        int sum;
        switch(code[code.Length - 1])
        {
            case 'S':
                int s1 = Int16.Parse(code.Substring(0, 2)), s2 = Int16.Parse(code.Substring(2, 2));
                sum = boardinfo.getSurveySum(board, s1, s2, code[4], replace);
                return passesValueCheck('E', sum, Int16.Parse(code.Substring(5, 1)));
            case 'T':
                int s = Int16.Parse(code.Substring(0, 2)) - 1;
                return ((board[s] == code[2]) == (code[3] == 'O'));
            case 'A':
                sum = boardinfo.getAdjacentSum(board, code[0], code[1], replace);
                return passesValueCheck(code[2], sum, Int16.Parse(code.Substring(3, 1)));
            case 'O':
                sum = boardinfo.getOppositeSum(board, code[0], code[1], replace);
                return passesValueCheck(code[2], sum, Int16.Parse(code.Substring(3, 1)));
            case 'W':
                sum = boardinfo.getWithinSum(board, code[0], code[1], Int16.Parse(code.Substring(2, 1)), replace);
                return passesValueCheck(code[3], sum, Int16.Parse(code.Substring(4, 1)));
            default:
                sum = boardinfo.getSmallestBand(board, code[0], replace);
                return passesValueCheck('F', sum, Int16.Parse(code.Substring(1, 1)));
        }
    }
    private bool passesValueCheck(char numType, int result, int check)
    {
        switch(numType)
        {
            case 'E': return (result == check);
            case 'L': return (result >= check);
            default: return (result <= check);
        }
    }
}
