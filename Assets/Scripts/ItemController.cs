﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Assertions;

public class ItemController : MonoBehaviour
{
    /*
    will draw items
    gets list of items from board manager
    */


    public TileBase noItem;
    public TileBase bomb;
    public TileBase health;
    public TileBase attack;


    public BoundsInt tileArea;
    private Tilemap tileMap;

    private List<Item> items;

    private Dictionary<int, TileBase> tiles;
    TileBase[] tileArray;
    // Start is called before the first frame update

    public void init()
    {
      if (tiles == null)
      {
        tiles = new Dictionary<int, TileBase>();
      }
      tiles.Add(0, noItem);
      tiles.Add(1, bomb);
      tiles.Add(2, health);
      tiles.Add(3, attack);
      tileArray = new TileBase[tileArea.size.x * tileArea.size.y * tileArea.size.z];

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetItems(List<Item> newItems){
      tileMap = GetComponent<Tilemap>();

      var dictionary = new Dictionary<Vector2Int, TileBase>();

      if (tiles == null)
      {
        init();
      }

      foreach ( Item item in newItems) {
        dictionary.Add(item.GetPosition(), tiles[item.getType()]);
      }

      for (int y = 0; y < tileArea.size.y; y++)
      {
          for (int x = 0; x < tileArea.size.x; x++)
          {
            Vector2Int position = new Vector2Int(x,y);
            TileBase tile;
            if (dictionary.TryGetValue(position, out tile))
            {
              tileArray[y * tileArea.size.x + x] = tile;
            }
          }
      }

      tileMap.SetTilesBlock(tileArea, tileArray);
    }

    public void Activated(Vector2Int position)
    {
      tileArray[position.y * tileArea.size.x + position.x] = null;
      tileMap.SetTilesBlock(tileArea, tileArray);

    }

    public void SetTile(int type, Vector2Int position)
    {
      tileArray[position.y * tileArea.size.x + position.x] = bomb;
      tileMap.SetTilesBlock(tileArea, tileArray);
    }
}