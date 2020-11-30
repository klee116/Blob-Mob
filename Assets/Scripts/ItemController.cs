using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ItemController : MonoBehaviour
{
    /*
    will draw items
    gets list of items from board manager
    */

    public TileBase bomb;

    public BoundsInt tileArea;
    private Tilemap tileMap;


    // Start is called before the first frame update
    void Start()
    {
      //SetItems();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetItems(){//List<Item> items){
      tileMap = GetComponent<Tilemap>();
      TileBase[] tileArray = new TileBase[tileArea.size.x * tileArea.size.y * tileArea.size.z];

      for (int y = 0; y < tileArea.size.y; y++)
      {
          for (int x = 0; x < tileArea.size.x; x++)
          {
              tileArray[y * tileArea.size.x + x] = bomb;
          }
      }

      tileMap.SetTilesBlock(tileArea, tileArray);
    }
}
