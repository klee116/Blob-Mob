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

    private List<Item> items;

    private Dictionary<int, TileBase> tiles;

    // Start is called before the first frame update
    void Start()
    {
      tiles = new Dictionary<int, TileBase>();
      tiles.Add(1, bomb);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetItems(List<Item> newItems){
      tileMap = GetComponent<Tilemap>();
      TileBase[] tileArray = new TileBase[tileArea.size.x * tileArea.size.y * tileArea.size.z];

      var dictionary = new Dictionary<Vector2Int, TileBase>();

      foreach ( Item item in newItems) {
        dictionary.Add(item.GetPosition(), tiles[item.getType()]);
      }

      for (int y = 0; y < tileArea.size.y; y++)
      {
          for (int x = 0; x < tileArea.size.x; x++)
          {
            Vector2Int position = new Vector2Int(x,y);
            TileBase tile;
            if (dictionary.TryGetValue(position, out tile)){
              tileArray[y * tileArea.size.x + x] = tile;
            }
          }
      }

      tileMap.SetTilesBlock(tileArea, tileArray);
    }
}
