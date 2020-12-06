using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine.Assertions;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public enum Direction { left, right, up, down };
public class Movement
{
    public Vector2Int coordinates;
    public Direction direction;
    public int index;
    public int roll = -1;

}

public class BoardManager : MonoBehaviour, IOnEventCallback
{
    public const byte SendMoveEventCode = 1;
    public const byte FinishInitEventCode = 2;
    public TileMapGen TMG;
    public BoardHighlights BoardHighlights;
    public Vector3 PlayerSpriteOffset;
    public Vector2 ClickOffset;
    private Tilemap tileMap;
    public Tilemap Highlights;
    public Tilemap ItemTilemap;
    public int waveDirection;
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
    private List<bool> initializedPlayers;
    public DeathMenu deathMenu;
    bool SecondClick; Movement move;
    void Start()
    {
        tileMap = GetComponent<Tilemap>();
        TMG.Generate();
        // object seed;
        // PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(RoomLauncher.SEED_PROP_KEY, out seed);
        // TMG.GenerateWithSeed((int) seed);
        SetDefaults();
        BoardHighlights = Highlights.GetComponent<BoardHighlights>();
        BoardHighlights.Generate(W, H);
        SpawnAllPlayers();
        initializedPlayers = new List<bool>(numPlayers);
        items = new List<Item>();
        itemController = ItemTilemap.GetComponent<ItemController>();
        SpawnInitialItems();
    }

    void Update()
    {
        UpdateSelection();
#if UNITY_EDITOR
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
    public void SetDefaults()
    {
        numPlayers = 0;
        ActivePlayer = 0;
        H = TMG.tileArea.size.y; W = TMG.tileArea.size.x;
        SecondClick = false;
        move = new Movement();
        waveDirection = 0;
    }

    public void SpawnAllPlayers()
    {
        CharacterList = new List<Character>();
        moves = new List<Movement>();
        SpawnPlayer(0, 4, 7);
        SpawnPlayer(1, 3, 3);
        SpawnPlayer(2, 5, 3);
        SpawnPlayer(3, 6, 8);
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
    public void SpawnItemWave()
    {
        // x: 1 = top, 2 = bottom, 3 = left, 4 = right (side of map that next wave spawns from)
        int x = Random.Range(0,4); //pick new direction, or stay the same. (same has twice the chance)
        if (x != 4)
        {
            waveDirection = x;
        }

        List<int> ItemWave = new List<int>();
                
                //TODO: add item codes into the below fruitloopz

        if (waveDirection == 0)
        {
            // execute 'from top' scenario

            // generate W items (or nulls)
            // (have a line of items (floating or on tiles that are separate from board) to show incoming items and which direction everything is going to move)
            // put the items on Row H (top)
            // delete items on Row 1
            // all items move y position to position - 1
            for (int i = 0; i < W; i++)
            {
                ItemWave.Add(Random.Range(0,100));
            }

        }
        else if (waveDirection == 1)
        {
            for (int i = 0; i < W; i++)
            {
                ItemWave.Add(Random.Range(0,100));
            }
            //execute 'from bottom' scenario

            // generate W items (or nulls)
            // put the items on Row 1 (bottom)
            // delete items on Row H
            // all items move y position to position - 1
        }
        else if (waveDirection == 2)
        {
            for (int i = 0; i < H; i++)
            {
                ItemWave.Add(Random.Range(0,100));
            }
            //execute 'from left' scenario

            // generate H items (or nulls)
            // put the items on Column 1 (left)
            // delete items on Column W
            // all items move x position to position + 1
        }
        else if (waveDirection == 3)
        {
            for (int i = 0; i < H; i++)
            {
                ItemWave.Add(Random.Range(0,100));
            }
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
        bool done = false;
        while (done == false)
        {
            List<Movement> Showdowns = new List<Movement>();
            bool matched = false;
            for (int i = 0; i < moves.Count && !matched; i++)
            {
                 //TODO: something about multiplayer rng
                for (int j = i + 1; j < moves.Count && !matched; j++)
                {
                    if (moves[i].coordinates == moves[j].coordinates)
                    {
                        Showdowns.Add(moves[i]); Showdowns.Add(moves[j]);
                        matched = true;
                        Debug.Log("Added move by player " + moves[i].index);
                        Debug.Log("Added move by player " + moves[j].index);
                        //move i and j go to the showdown
                        for (int k = j + 1; k < moves.Count; k++)
                        {
                            if (moves[i].coordinates == moves[k].coordinates)
                            {
                                Showdowns.Add(moves[k]);
                                Debug.Log("Added move by player " + moves[k].index);
                            }
                        }

                    }
                }
            }

            int winnerIndex = -1; int tempMax = -1;

            foreach (Movement i in Showdowns)
            {
                i.roll = Random.Range(1, 6) + CharacterList[i.index].GetAttack() - 3;
                Debug.Log("Player " + i.index + " rolled a " + i.roll + "!");
                if (i.roll > tempMax)
                {
                    tempMax = i.roll;
                    winnerIndex = i.index;
                }
            }

            if (winnerIndex != -1)
            {
                Debug.Log("Player " + winnerIndex + "Wins!");
            }

            List<Movement> toRemove = new List<Movement>();
            foreach (Movement i in Showdowns)
            {
                if (i.index != winnerIndex)
                {
                    DamagePlayer(i.index, 50);
                    toRemove.Add(i);
                }
            }
            foreach (Movement i in toRemove)
            {
                moves.Remove(i);
            }
            done = true;
            //Debug.Log("done is true");
            for (int i = 0; i < moves.Count; i++)
            {
                for (int j = i + 1; j < moves.Count; j++)
                {
                    if (moves[i].coordinates == moves[j].coordinates)
                    {
                        done = false;
                    }
                }
            }
        }
        
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
        // movement anime
        // wait for next turn, generate which direction (up/down/left/right) the next wave of items comes from

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

    // Sends the moves of the active player to every other player
    private void SendFinishInitEvent()
    {
        object[] content = new object[1];
        content[0] = ActivePlayer;
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
        PhotonNetwork.RaiseEvent(FinishInitEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    // Sends the moves of the active player to every other player
    private void SendMoveEvent()
    {
        object[] content = new object[2];
        content[0] = ActivePlayer;
        content[1] = moves[ActivePlayer];
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
        PhotonNetwork.RaiseEvent(SendMoveEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    // Receive events from other players, used for making sure everyone initialized and receiving turns
    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        object[] data = (object[])photonEvent.CustomData;

        switch (eventCode)
        {
            case SendMoveEventCode:
                int movedPlayer = (int)data[0];
                Movement move = (Movement)data[1];
                moves[movedPlayer] = move;
                break;
            case FinishInitEventCode:
                int initPlayer = (int)data[0];
                initializedPlayers[initPlayer] = true;
                break;
            default:
                Debug.LogError("Unrecognized event received! Code of " + eventCode);
                break;
        }
    }
}
