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
    private int selectionX = -1;
    private int selectionY = -1;
    //public item[] itemList;

    void Start()
    {
        TMG.Generate();
        SetDefaults();
        SpawnAllPlayers();
    }

    void Update()
    {
        #if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TMG.Generate();
                MovePlayer(0,6,6);
                //insert make character lose health here when ready to test
            }
        #endif
        UpdateSelection();
        waitClick();
    }

    public void SpawnAllPlayers()
    {
        CharacterList = new List<Character>();
        SpawnPlayer(0,4,7);
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
        
        player.init();
        player.SetPosition(x,y); player.SetDimensions(W,H); player.SetHealth(100);

        CharacterList.Add(player);
        numPlayers++;
    }
    private Vector3 GetTileCenter(int x, int y)
    {
        Tilemap tileMap = GetComponent<Tilemap>();
        return tileMap.CellToWorld(new Vector3Int(x,y,0));
    }


    private void UpdateSelection()
    {
        if (!Camera.main)
            return;
        
        RaycastHit hit; 
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit, 25.0f, LayerMask.GetMask("Plane")))
        {
            selectionX = (int)hit.point.x;
            selectionY = (int)hit.point.z; 
        }
        else
        {
            selectionX = -1;
            selectionY = -1;
        }
    }
    private void waitClick()
    {
        //BoardHighlights.Instance.HighlightAllowedMoves(selectedCharacter.PossibleMoves());
        if (Input.GetMouseButtonDown(0) && (selectionX >= 0 && selectionY >= 0))
        {
            MovePlayer(0,selectionX,selectionY);
            System.Threading.Thread.Sleep(100);
        }
    }

    public void MovePlayer(int index, int x, int y)
    {
        CharacterList[index].SetPosition(x,y);
        CharacterList[index].transform.position = GetTileCenter(x,y);
        //tilelist update ints? when ints are assigned ig
    }

    public void DamagePlayer(int index, int damageAmount)
    {
        CharacterList[index].Health -= damageAmount;
        if (CharacterList[index].Health <= 0)
        {
            //insert death function here

        }
    }
}