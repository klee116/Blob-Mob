using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine.Assertions;

public enum Direction { left, right, up, down };
public class Movement
{
    public Vector2Int coordinates;
    public Direction direction;
    public int index;
}

public class BoardManager : MonoBehaviour
{
    public TileMapGen TMG;
    public BoardHighlights BoardHighlights;
    public Vector3 PlayerSpriteOffset;
    public Vector2 ClickOffset;
    private Tilemap tileMap;
    public Tilemap Highlights;
    public Tilemap ItemTilemap;
    public int[,] TileList;
    public GameObject[] CharacterPrefabs;
    public List<Character> CharacterList;
    private List<Movement> moves;
    private ItemController itemController;
    private int numPlayers;
    private int alivePlayers;
    private int ActivePlayer;
    private int W, H;
    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.5f;
    private int selectionX = -1;
    private int selectionY = -1;
    private List<Item> items;
    public DeathMenu deathMenu;
    bool SecondClick; Movement move;
    void Start()
    {
        tileMap = GetComponent<Tilemap>();
        TMG.Generate();
        SetDefaults();
        BoardHighlights = Highlights.GetComponent<BoardHighlights>();
        BoardHighlights.Generate(W, H);
        SpawnAllPlayers();
        items = new List<Item>();
        itemController = ItemTilemap.GetComponent<ItemController>();
        SpawnInitialItems();
    }

    void Update()
    {
        UpdateSelection();
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TMG.Generate();
            MovePlayer(0, 6, 6);
            //insert make character lose health here when ready to test
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (CharacterList[0].SetHealthMax())
            {
                alivePlayers++;
                deathMenu.ToggleDeathMenu(5);
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            DamagePlayer(0, 50);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            SpawnItem(1, new Vector2Int(selectionX, selectionY));
        }
#endif
        waitClick();

        receivePlayersInputs();
    }

    public void SpawnAllPlayers()
    {
        CharacterList = new List<Character>();
        moves = new List<Movement>();
        SpawnPlayer(0, 4, 7);
        SpawnPlayer(1, 3, 3);
        //SpawnPlayer(2, 5, 3);
    }

    private void CycleActivePlayer()
    {
        if (ActivePlayer >= numPlayers - 1 || ActivePlayer < 0)
        {
            ActivePlayer = 0;
        }
        else
        {
            ActivePlayer++;
        }
    }
    public void SpawnPlayer(int index, int x, int y)
    {
        GameObject go = Instantiate(CharacterPrefabs[index], GetTileCenter(x, y), Quaternion.identity) as GameObject;
        go.transform.SetParent(transform);
        Character player = go.GetComponent<Character>();

        player.init(W, H, index);
        player.SetPosition(x, y);

        CharacterList.Add(player);
        numPlayers++;
        alivePlayers++;
    }
    public void SpawnInitialItems()
    {
        //procgen the first set up items on the board (avoid player spawns ig)

        BombItem bomb = new BombItem();
        bomb.SetPosition(new Vector2Int(5, 3));

        items.Add(bomb);

        bomb = new BombItem();
        bomb.SetPosition(new Vector2Int(6, 3));

        items.Add(bomb);

        bomb = new BombItem();
        bomb.SetPosition(new Vector2Int(4, 4));

        items.Add(bomb);

        bomb = new BombItem();
        bomb.SetPosition(new Vector2Int(5, 2));

        items.Add(bomb);

        HealItem health = new HealItem();

        health.SetPosition(new Vector2Int(3, 3));

        items.Add(health);

        AttackItem attack = new AttackItem();

        attack.SetPosition(new Vector2Int(2, 2));

        items.Add(attack);

        itemController.SetItems(items);
    }

    public void SpawnItem(int type, Vector2Int position)
    {
        Item toAdd = new BombItem();
        if (type == 1)
        {
            toAdd = new BombItem();
        }
        else if (type == 2)
        {
            toAdd = new BombItem();
        }

        else //default to bomb
        {

        }

        toAdd.SetPosition(position);
        items.Add(toAdd);
        itemController.SetTile(type, position);
    }
    public void SpawnItemWave(int x)
    {
        // x = 1-4 , 1 = top, 2 = bottom, 3 = left, 4 = right (side of the map that the next wave of items spawns from)

        if (x == 0)
        {
            // execute 'from top' scenario

            // generate W items (or nulls)
            // (have a line of items (floating or on tiles that are separate from board) to show incoming items and which direction everything is going to move)
            // put the items on Row H (top)
            // delete items on Row 1
            // all items move y position to position - 1
        }
        else if (x == 1)
        {
            //execute 'from bottom' scenario

            // generate W items (or nulls)
            // put the items on Row 1 (bottom)
            // delete items on Row H
            // all items move y position to position - 1
        }
        else if (x == 2)
        {
            //execute 'from left' scenario

            // generate H items (or nulls)
            // put the items on Column 1 (left)
            // delete items on Column W
            // all items move x position to position + 1
        }
        else if (x == 3)
        {
            //execute 'from right' scenario

            // generate H items (or nulls)
            // put the items on Column W (right)
            // delete items on Column 1
            // all items move x position to position - 1
        }
        else
        {
            //default to executing from top scenario, this should not execute unless theres an error in code somewhere
        }

    }

    public void ExecuteTurn() // function that drives the turn after receiving all players' turn data (Intended tile (x,y) and intended direction (up/down/left/right)) OR turn timer runs out
    {
        foreach (Movement move in moves)
        {
            MovePlayer(move.index, move.coordinates.x, move.coordinates.y);
            List<Item> toRemove = new List<Item>();
            foreach (Item item in items)
            {
                if (CharacterList[move.index].GetPosition() == item.GetPosition())
                {
                    item.Activate(this);
                    itemController.Activated(item.GetPosition());
                    toRemove.Add(item);
                }
            }
            foreach (Item item in toRemove)
            {
                items.Remove(item);
            }
        }
        // item wave spawns and all items move as turn executes, (to land on item you must aim where it is going to go rather than where it is when you click)
        // Check for player collisions, calculate a winner;
        // Loser gets thrown into the direction the winner chose to face;
        // Draw results in both players being knocked away from the spot (need to think about how to deal with being knocked into another player as a result of this
        //     maybe bump the third player further in the direction)
        // movement anime

        // Once Players all moved, detect if they've landed on an item, trigger item effects using the direction of the players as a parameter
        // Resolve effects/Play animations

        // wait for next turn, generate which direction (up/down/left/right) the next wave of items comes from

    }

    public void FightOverTile(List<int> indices)
    {
        return;
    }
    public void SetDefaults()
    {
        numPlayers = 0;
        ActivePlayer = 0;
        H = TMG.tileArea.size.y; W = TMG.tileArea.size.x;
        SecondClick = false;
        move = new Movement();
    }
    private Vector3 GetTileCenter(int x, int y)
    {
        return tileMap.CellToWorld(new Vector3Int(x, y, 0)) + PlayerSpriteOffset;
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

    private void receivePlayersInputs()
    {
        if (moves.Count >= alivePlayers)
        {
            ExecuteTurn();
            moves.Clear();
        }
    }

    private void waitClick()
    {
        BoardHighlights.UpdatePlayerHighlights(CharacterList[ActivePlayer].getMoves(), selectionX, selectionY, SecondClick, move);
        if (SecondClick)
        {
            waitSecondClick(move);
        }
        else if (!SecondClick)
        {
            move = new Movement();
            if (Input.GetMouseButtonDown(0) && (selectionX >= 0 && selectionY >= 0 && selectionX < W && selectionY < H) &&
                CharacterList[ActivePlayer].possibleMoves[selectionX, selectionY])
            {
                if (!CharacterList[ActivePlayer].isDead)
                {
                    move.coordinates = new Vector2Int(selectionX, selectionY);
                    move.index = ActivePlayer;
                    SecondClick = true;
                    //move.direction = Direction.down;
                    //TODO: send move over photon
                    System.Threading.Thread.Sleep(100);
                }
            }
        }
    }

    private void waitSecondClick(Movement move)
    {
        if (!SecondClick)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if ((selectionX == move.coordinates.x + 1) && (selectionY == move.coordinates.y) && (!CharacterList[ActivePlayer].isDead))
            {
                move.direction = Direction.right;
            }
            else if ((selectionX == move.coordinates.x - 1) && (selectionY == move.coordinates.y))
            {
                move.direction = Direction.left;
            }
            else if ((selectionX == move.coordinates.x) && (selectionY == move.coordinates.y + 1))
            {
                move.direction = Direction.up;
            }
            else if ((selectionX == move.coordinates.x) && (selectionY == move.coordinates.y - 1))
            {
                if (!CharacterList[ActivePlayer].isDead)
                {
                    move.direction = Direction.down;

                }
            }
            else
            {
                SecondClick = false;
                return;
            }
            Debug.Log("moving player " + move.index + " to (" + move.coordinates.x + "," + move.coordinates.y + " facing " + move.direction);
            moves.Add(move);
            CycleActivePlayer();
            SecondClick = false;
            //TODO: send move over photon
            System.Threading.Thread.Sleep(100);
        }

    }

    public void MovePlayer(int index, int x, int y)
    {
        CharacterList[index].SetPosition(x, y);
        CharacterList[index].transform.position = GetTileCenter(x, y);
        //tilelist update ints? when ints are assigned ig
    }

    public void DamagePlayer(int index, int damageAmount)
    {
        CharacterList[index].ModifyHealth(-1 * damageAmount);
        if (CharacterList[index].Health <= 0)
        {
            //insert death function here
            alivePlayers--;
            CharacterList[index].isDead = true;
            // if(PhotonNetwork.PlayerList.IsLocal)
            deathMenu.ToggleDeathMenu(5);

        }
    }
}
