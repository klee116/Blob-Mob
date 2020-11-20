using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public int Health{set;get;}
    public bool isDead;
    public int Speed{set;get;}
    public GameObject healthBarPrefab;
    private int CurrentX{set;get;} private int CurrentY{set;get;}
    public bool[,] possibleMoves{set;get;}
    public int maxHeight{set;get;} public int maxWidth{set;get;}
    public int Score;
    private GameObject healthBar;

    public DeathMenu deathMenu;
    void Start()
    {
       isDead = false;
    }

    public void init()
    {
        if (healthBar == null)
        {
            healthBar = Instantiate(healthBarPrefab) as GameObject;
            healthBar.transform.SetParent(transform, false);
        }
        SetHealthMax();
        SetSpeed(3);
    }
    public void SetHealthMax()
    {
        healthBar.transform.localScale = new Vector3(100 / 100.0f, .1f, 1);
        Health = 100;
        isDead = false;
    }

    public void ModifyHealth(int x)
    {
        Health+=x;
        if (Health <= 0)
        {
            Health = 0;
        }
        
        healthBar.transform.localScale = new Vector3(Health/100.0f, .1f, 1);
        
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

    public void getMoves()
    {
        possibleMoves = new bool[maxWidth,maxHeight];
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
