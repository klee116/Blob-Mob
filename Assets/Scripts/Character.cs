using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public int Health{set;get;}
    public int Speed{set;get;}
    public GameObject healthBarPrefab;
    private int CurrentX{set;get;} private int CurrentY{set;get;}
    private bool[,] possibleMoves{set;get;}
    public int maxHeight{set;get;} public int maxWidth{set;get;}
    private GameObject healthBar;

    void Start()
    {
       
    }

    public void init()
    {
        if (healthBar == null)
        {
            healthBar = Instantiate(healthBarPrefab) as GameObject;
            healthBar.transform.SetParent(transform, false);
        }
    }
    public void SetHealth(int x)
    {
        healthBar.transform.localScale = new Vector3(x / 100.0f, .1f, 1);
        Health = x;
    }
    public void SetSpeed(int x)
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
    {
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
