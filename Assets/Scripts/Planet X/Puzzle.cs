using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle 
{
    private BoardInfo boardinfo = new BoardInfo();
    public PuzzleInfo generatePuzzle()
    {
        regenerate:
        string board = new BoardConfig().getRandomBoardConfig();
        //Debug.LogFormat("BOARD: {0}", board);
        GenerateNotes gen = new GenerateNotes();
        List<NoteObj> notes = gen.generateAllNotes(board);
        HashSet<int> possX = getPossXPos(notes);
        bool noteXGen = false;
        if(possX.Count > 1)
        {
            noteXGen = true;
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
            goto regenerate;
        List<NoteObj> chosenNotes = new List<NoteObj>();
        List<string> boards = new BoardConfig().getAllValidConfigs();
        if(noteXGen)
        {
            chosenNotes.Add(notes[notes.Count - 1]);
            notes.RemoveAt(notes.Count - 1);
            boards = getValidBoards(boards, chosenNotes[chosenNotes.Count - 1]);
        }
        possX = getPossXPos(boards);
        while(possX.Count > 1)
        {
            int count = boards.Count + 0;
            boards = getValidBoards(boards, notes[0]);
            if(count != boards.Count)
            {
                chosenNotes.Insert(0, notes[0]);
                possX = getPossXPos(boards);
            }
            notes.RemoveAt(0);
        }
        for (int i = 0; i < chosenNotes.Count; i++)
        {
            NoteObj note = chosenNotes[i];
            chosenNotes.RemoveAt(i);
            possX = getPossXPos(chosenNotes);
            if (possX.Count > 1)
                chosenNotes.Insert(i, note);
            else
                i--;
        }
        return new PuzzleInfo(board, chosenNotes);
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
    private HashSet<int> getPossXPos(List<string> boards)
    {
        HashSet<int> possX = new HashSet<int>();
        foreach (string board in boards)
            possX.Add(board.IndexOf("X"));
        return possX;
    }
    private List<string> getValidBoards(List<string> boards, NoteObj note)
    {
        List<string> newBoards = new List<string>();
        foreach(string board in boards)
        {
            if (notePassed(note, board))
                newBoards.Add(board);
        }
        return newBoards;
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
