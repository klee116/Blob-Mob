using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public int Health{set;get;}
    public int Speed{set;get;}
    private int CurrentX{set;get;} private int CurrentY{set;get;}
    private bool[,] possibleMoves{set;get;}
    public int maxHeight{set;get;} public int maxWidth{set;get;}


    public void setHealth(int x)
    {
        Health = x;
    }
    public void setSpeed(int x)
    {
        Speed = x;
    }
    public void SetPosition(int x, int y)
    {
        CurrentX = x; CurrentY = y;
    }
    public void SetDimensions(int x, int y)
    {
        maxHeight = x; maxWidth = y;
    }
    public void getMoves ()
    {//check for up down left right for movement possbilities
        /*for (int i = 0; i < maxWidth; i++)
        {
            for (int j = 0; j < maxHeight; j++)
            {
                possibleMoves[i,j] = false;
            }
        }*/
        for (int i = 0; i < maxWidth; i++)
        {
            for (int j = 0; j < maxHeight; j++)
            {
                if (Mathf.Abs(CurrentX - i) + Mathf.Abs(CurrentY - j) <= Speed)
                {
                    possibleMoves[i,j] = true;
                } 
                else
                {
                    possibleMoves[i,j] = false;
                }
            }
        }
    }
}
