﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardHighlights : MonoBehaviour
{
    public TileBase possible, selected;

    public Tilemap tileMap;
    private int[,] intMap;
    private TileBase[] tileArray;
    public BoundsInt tileArea;
    // Start is called before the first frame update
    void Start()
    {
       tileMap = GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Generate(int W, int H)
    {
        intMap = new int[W, H];
        tileArea.size = new Vector3Int(W,H,1);
        tileArray = new TileBase[tileArea.size.x * tileArea.size.y];
    }

    public void UpdateHighlights(bool[,] possibleMoves, int X, int Y)
    {
        for (int y = 0; y < tileArea.size.y; y++)
        {
            for (int x = 0; x < tileArea.size.x; x++)
            {
                switch (possibleMoves[x, y])
                {
                    case false:
                        tileArray[y * tileArea.size.x + x] = null;
                        break; 
                    case true:
                        tileArray[y * tileArea.size.x + x] = possible;
                        break;                  
                }
            }
        }
    
        if ((X >= 0 && Y >= 0) && (X < tileArea.size.x && Y < tileArea.size.y) && possibleMoves[X,Y])
        {
            tileArray[Y * tileArea.size.x + X] = selected;
        }

        tileMap.SetTilesBlock(tileArea, tileArray);
    }
}