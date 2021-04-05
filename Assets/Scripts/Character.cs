using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public int Health{set;get;}
    public bool isDead;
    public int Speed; //movement speed
    public int Attack; //attack power
    public int Shield;
    public GameObject healthBarPrefab;
    private int CurrentX{set;get;} private int CurrentY{set;get;}

    public Direction Direction;
    public bool[,] possibleMoves{set;get;}
    public int maxHeight{set;get;} public int maxWidth{set;get;}
    public int Score;

    public int Index;
    private GameObject healthBar;

    public DeathMenu deathMenu;
    void Start()
    {
       isDead = false;
    }

    public void init(int x, int y, int i)
    {
        if (healthBar == null)
        {
            healthBar = Instantiate(healthBarPrefab) as GameObject;
            healthBar.transform.SetParent(transform, false);
        }
        possibleMoves = new bool[x,y];
        maxHeight = x; maxWidth = y;
        Direction = Direction.down;
        Attack = 3; Index = i; Shield = 0;
        SetHealthMax();
        getMoves();
    }
    public bool SetHealthMax()
    {
        bool revived = false;
        if (isDead)
        {
            revived = true;
        }
        healthBar.transform.localScale = new Vector3(100 / 100.0f, .1f, 1);
        Health = 100;
        isDead = false;

        return revived;
    }
    public void ModifyHealth(int x)
    {
        if (Shield >= 1)
        {
            Shield--;
            Debug.Log("Player " + Index + " shield broke, remaining shield: " + Shield);
        }
        else
        {
            Debug.Log("Player " + Index + " got hit for " + x + ", remaining health: " + Health);
            Health+=x;
            if (Health <= 0)
            {
                Health = 0;
            }

            healthBar.transform.localScale = new Vector3(Health/100.0f, .1f, 1);
        }

    }
    public void ModifySpeed(int x)
    {
        Speed += x;

        if (Speed < 1)
        {
            Speed = 1;
        }
    }

    public void ModifyShield(int x)
    {
        Shield += x;
        if (Speed < 1)
        {
            Shield = 0;
        }
    }

    public int GetAttack(){
      return Attack;
    }
    public void ModifyAttack(int x)
    {
        Attack += x;

        if (Attack < 1)
        {
            Attack = 1;
        }
    }
    public void SetPosition(int x, int y)
    {
        CurrentX = x; CurrentY = y;
    }
    public void SetDirection(Direction d)
    {
        Direction = d;
    }
    public string GetDirection()
    {
        if (Direction == Direction.left)
        {
            return "left";
        }
        else if (Direction == Direction.right)
        {
            return "right";
        }
        else if (Direction == Direction.up)
        {
            return "up";
        }
        else if (Direction == Direction.down)
        {
            return "down";
        }
        else return "ERROR";
    }
    public Vector2Int GetPosition() {
      return new Vector2Int(CurrentX,CurrentY);
    }
    public void SetIndex(int x)
    {
        Index = x;
    }
    public int GetIndex()
    {
        return Index;
    }
    public bool[,] getMoves()
    {
        for (int i = 2; i < maxWidth - 2; i++)
        {
            for (int j = 2; j < maxHeight - 2; j++)
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
        return possibleMoves;
    }
}
