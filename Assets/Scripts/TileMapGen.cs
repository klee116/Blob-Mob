using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapGen : MonoBehaviour
{
    //public static TileMapGen Instance{set;get;}
    public TileBase ground1, ground2, ground3;
    public BoundsInt tileArea;
    private Tilemap tileMap;
    
    public void Generate()
    {
        tileMap = GetComponent<Tilemap>();
        TileBase[] tileArray = new TileBase[tileArea.size.x * tileArea.size.y * tileArea.size.z];
        for (int y = 0; y < tileArea.size.y; y++)
        {
            for (int x = 0; x < tileArea.size.x; x++)
            {
                tileArray[y * tileArea.size.x + x] = ground1;
            }
        }

        tileMap.SetTilesBlock(tileArea, tileArray);
    }
}
