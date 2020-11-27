using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour
{
    public TileMapGen TMG;

    public BoardHighlights BoardHighlights;
    public Vector3 PlayerSpriteOffset;
    public Vector2 ClickOffset;
    private Tilemap tileMap;
    public Tilemap Highlights;
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
    public DeathMenu deathMenu;

    void Start()
    {
        tileMap = GetComponent<Tilemap>();
        TMG.Generate();
        SetDefaults();
        BoardHighlights = Highlights.GetComponent<BoardHighlights>();
        BoardHighlights.Generate(W, H);
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
            if (Input.GetKeyDown(KeyCode.R))
            {
                CharacterList[0].SetHealthMax();
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                DamagePlayer(0,50);
            }
        #endif
        UpdateSelection();
        BoardHighlights.UpdateHighlights(CharacterList[0].getMoves(), selectionX, selectionY);
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
        player.SetPosition(x,y); player.SetDimensions(W,H);

        CharacterList.Add(player);
        numPlayers++;
    }
    private Vector3 GetTileCenter(int x, int y)
    {
        
        return tileMap.CellToWorld(new Vector3Int(x,y,0)) + PlayerSpriteOffset;
    }


    private void UpdateSelection()
    {
        if (!Camera.main)
            return;
        
        RaycastHit2D hit;  
        hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), 25.0f, LayerMask.GetMask("Plane"));
        Vector2 clickInput = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tile = tileMap.WorldToCell(clickInput + ClickOffset);  

        if (tile.x < W && tile.y < H)
        {
            selectionX = tile.x;
            selectionY = tile.y; 
        }
        else
        {
            selectionX = -1;
            selectionY = -1;
        }
    }

    private void waitClick()
    { 
        if (Input.GetMouseButtonDown(0) && (selectionX >= 0 && selectionY >= 0) && CharacterList[0].possibleMoves[selectionX,selectionY])
        {
            if (!CharacterList[0].isDead)
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
        CharacterList[index].ModifyHealth(-1 * damageAmount);
        if (CharacterList[index].Health <= 0)
        {
            //insert death function here
            CharacterList[index].isDead = true;
            deathMenu.ToggleDeathMenu(5);
            
        }
    }
}

