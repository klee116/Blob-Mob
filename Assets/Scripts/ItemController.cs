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
    public TileBase speed;
    public TileBase snowball;
    public TileBase shield;

    public GameObject bombPrefab;
    public Vector3 BombSpriteOffset;


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
        tiles.Add(4, speed);
        tiles.Add(5, snowball);
        tiles.Add(6, shield);
        tileArray = new TileBase[tileArea.size.x * tileArea.size.y * tileArea.size.z];

    }
    // Update is called once per frame
    void Update()
    {

    }

    public void SetItems(List<Item> newItems)
    {
        tileMap = GetComponent<Tilemap>();

        var dictionary = new Dictionary<Vector2Int, TileBase>();

        if (tiles == null)
        {
            init();
        }

        foreach (Item item in newItems)
        {
            Vector2Int position = item.GetPosition();
            tileArray[position.y * tileArea.size.x + position.x] = tiles[item.getType()];
        }
        tileMap.SetTilesBlock(tileArea, tileArray);
    }



    public void Activated(Vector2Int position)
    {
        if (tileArray[position.y * tileArea.size.x + position.x] == bomb)
        {
            GameObject animBomb = Instantiate(bombPrefab, GetTileCenter(position.x, position.y), Quaternion.identity) as GameObject;
        }

        tileArray[position.y * tileArea.size.x + position.x] = null;
        tileMap.SetTilesBlock(tileArea, tileArray);

    }

  public void SetTile(int type, Vector2Int position)
  { 
    tileArray[position.y * tileArea.size.x + position.x] = tiles[type];
    tileMap.SetTilesBlock(tileArea, tileArray);
  }

    private Vector3 GetTileCenter(int x, int y)
    {
        return tileMap.CellToWorld(new Vector3Int(x, y, 0)) + BombSpriteOffset;
    }
}

