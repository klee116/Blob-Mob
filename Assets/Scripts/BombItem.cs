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
      boardManager.DamagePlayer(0, 40);
    }
}
