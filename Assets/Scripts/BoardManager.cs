using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour
{
    public TileMapGen TMG;
    public int[,] TileList;
    public GameObject[] CharacterPrefabs;
    public Character[] CharacterList;
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
    }

    void Update()
    {
        
    }

    public void SpawnAllPlayers()
    {

    }

    public void SetDefaults()
    {
        numPlayers = 0;
        H = TMG.tileArea.size.y; W = TMG.tileArea.size.x;
    }
    public void SpawnPlayer(int index, int x, int y)
    {
        GameObject go = Instantiate(CharacterPrefabs[index],GetTileCenter(x,y),Quaternion.identity) as GameObject;
        //go.transform.SetParent(transform); this is commented out for now because idk what it does
        CharacterList[index] = go.GetComponent<Character>();
        CharacterList[index].SetPosition(x,y);
        CharacterList[index].SetDimensions(W,H);
        CharacterList[index].SetHealth(100);
        //CharacterList[index].SetIndex[index]; not sure if need to use index yet
        numPlayers++;
    }

    private Vector3 GetTileCenter(int x, int z)
    {//will need to update the offsets/size
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_OFFSET;
        origin.z += (TILE_SIZE * z) + TILE_OFFSET;

        return origin;
    }
}
