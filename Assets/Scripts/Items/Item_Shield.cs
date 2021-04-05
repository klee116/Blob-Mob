using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Shield : Item
{
    int type = 6;
    // Start is called before the first frame update
    Vector2Int position;
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

        foreach (Character character in boardManager.CharacterList)
        {
            if (character.GetPosition() == position)
            {
                character.ModifyShield(1);
                Debug.Log("Player " + character.GetIndex() + " Shielded on " +
                position.x + "," + position.y + " Shield: " + character.Shield);
            }
        }
        boardManager.playShield.Play();
    }
}
