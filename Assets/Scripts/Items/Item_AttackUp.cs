using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_AttackUp : Item
{
    int type = 3;

    Vector2Int position;
    public void setType(int x)
    {
      type = x;
    }
    public void SetPosition(Vector2Int newPosition){
      position = newPosition;
    }

    public Vector2Int GetPosition() {
      return position;
    }

    public int getType(){
      return type;
    }

    public void Activate(BoardManager boardManager){

      foreach (Character character in boardManager.CharacterList)
      {
        if ( character.GetPosition() == position )
        {
          character.ModifyAttack(1);
          Debug.Log("Player " + character.GetIndex() + " Attackupped on " + 
          position.x + "," + position.y + " Attack: " + character.Attack);
        }
      }
    }
}
