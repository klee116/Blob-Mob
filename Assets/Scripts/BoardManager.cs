using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour
{
    public TileMapGen TMG;
    public int[,] TileList;
    public Character[] CharacterList;
    //public item[] itemList;

    void Start()
    {
        TMG.Generate();

    }

    void Update()
    {
        
    }
}
