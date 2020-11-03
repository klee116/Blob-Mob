using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour
{
    public TileMapGen TMG;
    public int[,] TileList;
    public GameObject[] CharacterPrefabs;
    private List<Character> CharacterList;
    private int numPlayers;
    private int W, H;
    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.5f;
    //public item[] itemList;

    void Start()
    {
        TMG.Generate();
        SetDefaults();
        SpawnAllPlayers();
        SpawnPlayer(0,4,7);
    }

    void Update()
    {
        
    }

    public void SpawnAllPlayers()
    {
        CharacterList = new List<Character>();
    }

    public void SetDefaults()
    {
        numPlayers = 0;
        H = TMG.tileArea.size.y; W = TMG.tileArea.size.x;
    }
    public void SpawnPlayer(int index, int x, int y)
    {
        GameObject go = Instantiate(CharacterPrefabs[index],GetTileCenter(x,y),Quaternion.identity) as GameObject;
        go.transform.SetParent(transform);
        Character player = go.GetComponent<Character>();
        player.SetPosition(x,y); player.SetDimensions(W,H); player.SetHealth(100);
        CharacterList.Add(player);
        numPlayers++;
    }

    private Vector3 GetTileCenter(int x, int y)
    {
        Tilemap tileMap = GetComponent<Tilemap>();
        return tileMap.CellToWorld(new Vector3Int(x,y,0));
    }

    public void MovePlayer(int index, int x, int y)
    {
        
    }
}

