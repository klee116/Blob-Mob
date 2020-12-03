using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombItem : MonoBehaviour, Item
{
    int type = 1;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
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
        }
      }

      foreach (Character character in boardManager.CharacterList)
      {
        if ((((character.GetPosition().x == position.x)
          && ( Mathf.Abs(character.GetPosition().y - position.y) < activator.GetAttack())
          ) || ((character.GetPosition().y == position.y)
            && ( Mathf.Abs(character.GetPosition().x - position.x) < activator.GetAttack())
          )) && character != activator && !character.isDead) 
        {
            boardManager.DamagePlayer(character.GetIndex(), 40);
        }
      }
      
    }
}
