using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_ : Item
{
    int type = 1;
    Vector2Int position;
    List<Direction> LD;
    public void setType(int x)
    {
        type = x;
    }
    public void SetPosition(Vector2Int newPosition)
    {
        position = newPosition;
    }

    public Vector2Int GetPosition()
    {
        return position;
    }

    public int getType()
    {
        return type;
    }


    public void Activate(BoardManager boardManager)
    {
        Character activator = boardManager.CharacterList[0];

        foreach (Character character in boardManager.CharacterList)
        {
            if (character.GetPosition() == position)
            {
                activator = character;
                Debug.Log("Player " + activator.GetIndex() + " activated bomb on " + position.x + "," + position.y);
            }
        }
        LD = new List<Direction>();
        LD.Add(Direction.up); LD.Add(Direction.down); LD.Add(Direction.left); LD.Add(Direction.right);

        List<Character> toHit = boardManager.FirstInLOS(LD, activator.GetPosition(), activator.Attack * 2, 99);

        Debug.Log("to hit size: " + toHit.Count);

        foreach (Character character in toHit)
        {
            if (character != activator)
            {
                Debug.Log("Bomb hit player " + character.Index);
                character.ModifyHealth(activator.Attack * -25);
            }
        }
        boardManager.playBomb.Play();
    }
}

