using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Snowball : Item
{
    int type = 5;

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
                //Debug.Log("Player " + activator.GetIndex() + " activated snowball on " + position.x + "," + position.y + " facing: " + activator.GetDirection() + "\n");
            }
        }
        LD = new List<Direction>(); 
        LD.Add(activator.Direction);

        List<Character> toHit = boardManager.FirstInLOS(LD, activator.GetPosition(), activator.Attack * 2, 1);

        foreach (Character character in toHit)
        {
            Debug.Log("Snowball hit player " + character.Index + "speed - " + activator.Attack/2);
            character.ModifyHealth(activator.Attack * -25);
            character.ModifySpeed(-activator.Attack/2);

        }
        boardManager.playBomb.Play();

    }
}
