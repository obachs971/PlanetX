using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardInfo
{
    public int getSurveySum(string b, int s1, int s2, char planet, char replace)
    {
        string board = b.Replace('X', replace);
        int sum = 0;
        if (s2 < s1)
            s2 += 12;
        for(int i = (s1 - 1); i < s2; i++)
        {
            if (board[mod(i, board.Length)] == planet)
                sum++;
        }
        return sum;
    }
    public int getAdjacentSum(string b, char p1, char p2, char replace)
    {
        string board = b.Replace('X', replace);
        int index = board.IndexOf(p1), sum = 0;
        while(index >= 0)
        {
            if (board[mod(index - 1, board.Length)] == p2 || board[mod(index + 1, board.Length)] == p2)
                sum++;
            index = board.IndexOf(p1, index + 1);
        }
        return sum;
    }
    public int getOppositeSum(string b, char p1, char p2, char replace)
    {
        string board = b.Replace('X', replace);
        int index = board.IndexOf(p1), sum = 0;
        while (index >= 0)
        {
            if (board[mod(index + (board.Length / 2), board.Length)] == p2)
                sum++;
            index = board.IndexOf(p1, index + 1);
        }
        return sum;
    }
    public int getWithinSum(string b, char p1, char p2, int w, char replace)
    {
        string board = b.Replace('X', replace);
        int index = board.IndexOf(p1), sum = 0;
        while (index >= 0)
        {
            bool flag = false;
            for(int i = 1; i <= w; i++)
            {
                if(board[mod(index - i, board.Length)] == p2)
                {
                    flag = true;
                    break;
                }
                else if (board[mod(index + i, board.Length)] == p2)
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
                sum++;
            index = board.IndexOf(p1, index + 1);
        }
        return sum;
    }
    public int getSmallestBand(string b, char p, char replace)
    {
        string board = b.Replace('X', replace);
        int index = board.IndexOf(p), band = b.Length;
        while(index >= 0)
        {
            int result = getBand(board, p, index);
            if (band > result)
                band = result;
            index = board.IndexOf(p, index + 1);
        }
        return (band + 1);
    }
    private int getBand(string b, char p, int index)
    {
        int lastIndex = index;
        for(int i = 1; i < b.Length; i++)
        {
            if (b[mod(index + i, b.Length)] == p)
                lastIndex = mod(index + i, b.Length);
        }
        return mod(lastIndex - index, b.Length);
    }
    private int mod(int n, int m)
    {
        while (n < 0)
            n += m;
        return (n % m);
    }
}
