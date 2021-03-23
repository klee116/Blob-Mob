﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Bomb : Item
{
  int type = 1;

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
    Character activator = boardManager.CharacterList[0];

    foreach (Character character in boardManager.CharacterList)
    {
      if ( character.GetPosition() == position )
      {
        activator = character;
        Debug.Log("Player " + activator.GetIndex() + " activated bomb on " + position.x + "," + position.y);
      }
    }

    foreach (Character character in boardManager.CharacterList)
    {
      if ((((character.GetPosition().x == position.x)
        && ( Mathf.Abs(character.GetPosition().y - position.y) <= activator.GetAttack())
        ) || ((character.GetPosition().y == position.y)
          && ( Mathf.Abs(character.GetPosition().x - position.x) <= activator.GetAttack())
        )) && character != activator && !character.isDead)
      {
          boardManager.DamagePlayer(character.GetIndex(), 25 * activator.GetAttack());
      }
    }

    boardManager.playBomb.Play();

  }
}
