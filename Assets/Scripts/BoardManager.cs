using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Assertions;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
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
    public AudioSource playBomb;
    public AudioSource playShowdown;
    public AudioSource moveClick1;
    public AudioSource moveClick2;
    public AudioSource playPotion;
    public AudioSource playSpeedup;

    public const byte SendMoveEventCode = 1;
    public const byte FinishInitEventCode = 2;
    public TileMapGen TMG;
    public BoardHighlights BoardHighlights;
    public Vector3 PlayerSpriteOffset;
    public Vector2 ClickOffset;
    private Tilemap tileMap;
    public Tilemap Highlights;
    public Tilemap ItemTilemap;
    public GameObject[] CharacterPrefabs;
    public List<Character> CharacterList;
    private List<Movement> moves;
    private List<Item> items;
    private ItemController itemController;
    //xpublic GameObject[] itemSpawner;
    private int numPlayers;
    private int alivePlayers;
    private int ActivePlayer;
    private int W, H;

    private int selectionX = -1;
    private int selectionY = -1;

    private List<bool> initializedPlayers;
    public Direction waveDirection;
    public DeathMenu deathMenu;
    bool SecondClick; Movement move;
    void Start()
    {
        tileMap = GetComponent<Tilemap>();
        if (PhotonNetwork.InRoom)
        {
            ActivePlayer = PhotonNetwork.LocalPlayer.GetPlayerNumber();
            object seed;
            PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(RoomLauncher.SEED_PROP_KEY, out seed);
            TMG.GenerateWithSeed((int)seed);
            initializedPlayers = new List<bool>(PhotonNetwork.CurrentRoom.PlayerCount);
            initializedPlayers[ActivePlayer] = true;
        }
        else
        {
            TMG.Generate();
        }
        SetDefaults();
        BoardHighlights = Highlights.GetComponent<BoardHighlights>();
        BoardHighlights.Generate(W, H);
        SpawnAllPlayers();
        initializedPlayers = new List<bool>(numPlayers);
        items = new List<Item>();
        itemController = ItemTilemap.GetComponent<ItemController>();
        SpawnInitialItems();
        SendFinishInitEvent();
    }

    void Update()
    {
        UpdateSelection();
#if UNITY_EDITOR
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
        waveDirection = Direction.down;
    }

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 1);
        foreach(Item item in items)
        {
            Vector3Int temp = new Vector3Int(item.GetPosition()[0], item.GetPosition()[1], 1);
            Gizmos.DrawSphere(temp, 1);
        }
    }

    public void SpawnAllPlayers()
    {
        CharacterList = new List<Character>();
        moves = new List<Movement>();
        SpawnPlayer(0, 4, 7);
        //SpawnPlayer(1, 3, 4);
        //SpawnPlayer(2, 5, 4);
        //SpawnPlayer(3, 6, 8);
    }

    private void CycleActivePlayer() //This is just for controlling everyone on one client for developing purposes
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
        GameObject go = Instantiate(CharacterPrefabs[0], GetTileCenter(x, y), Quaternion.identity) as GameObject;
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
        
    }

    public void SpawnItem(int type, Vector2Int position)
    {
        if (position.x != 1 && position.x >= 0 && position.x != W - 2 && position.x <= W
        && position.y != 1 && position.y >= 0 && position.y != H - 2 && position.y <= H)
        {
            Item toAdd = new Item_Bomb();
            if (type == 1)
            {
                toAdd = new Item_Bomb();
            }
            else if (type == 2)
            {
                toAdd = new Item_Heal();
            }
            else if (type == 3)
            {
                toAdd = new Item_AttackUp();
            }
            else if (type == 4)
            {
                return;
            }
            else //default to bomb
            {
                toAdd = new Item_Bomb();
            }

            toAdd.SetPosition(position);
            items.Add(toAdd);
            itemController.SetTile(type, position);
            Debug.Log("Spawned item on " + position.x + "," + position.y);
        }
    }
    public void SpawnItemWave()
    {
        // x: 1 = top, 2 = bottom, 3 = left, 4 = right (side of map that next wave spawns from)
        int x = Random.Range(0, 4); //pick new direction, or stay the same. (same has twice the chance)
        if (x != 4)
        {
            if (x == 0)
            {
                waveDirection = Direction.down;
            }
            else if (x == 1)
            {
                waveDirection = Direction.up;
            }
            else if (x == 2)
            {
                waveDirection = Direction.right;
            }
            else if (x == 3)
            {
                waveDirection = Direction.left;
            }
            else{}
        }
        List<int> ItemWave = new List<int>();

        //TODO: add item codes into the below fruitloopz

        for (int i = 0; i < Mathf.Max(H, W); i++)
        {
            ItemWave.Add(Random.Range(0, 5));
        }
        //TODO: add item codes into the below fruitloopz
        if (waveDirection == Direction.down)
        {
            // execute 'from top' scenario
            // generate W items (or nulls)
            // put the items on Row H (top)
            // delete items on Row 1
            // all items move y position to position - 1
            for (int i = 1; i < W-1; i++)
            {
                SpawnItem(ItemWave[i], new Vector2Int(i, H-1));
            }

        }
        else if (waveDirection == Direction.up)
        {
            //execute 'from bottom' scenario

            // generate W items (or nulls)
            // put the items on Row 1 (bottom)
            // delete items on Row H
            // all items move y position to position - 1
            for (int i = 1; i < W-1; i++)
            {
                SpawnItem(ItemWave[i], new Vector2Int(i, 0));
            }
        }
        else if (waveDirection == Direction.right)
        {
            //execute 'from left' scenario

            // generate H items (or nulls)
            // put the items on Column 1 (left)
            // delete items on Column W
            // all items move x position to position + 1
            for (int i = 1; i < H-1; i++)
            {
                SpawnItem(ItemWave[i], new Vector2Int(0, i));
            }
        }
        else if (waveDirection == Direction.left)
        {
            //execute 'from right' scenario

            // generate H items (or nulls)
            // put the items on Column W (right)
            // delete items on Column 1
            // all items move x position to position - 1
            for (int i = 1; i < W-1; i++)
            {
                SpawnItem(ItemWave[i], new Vector2Int(W-1, i));
            }
        }
        else
        {
            //default to executing from top scenario, this should not execute unless theres an error in code somewhere
        }

    }
    public void PercolateItems(Direction direction) // called with direction that goes with item wave spawn direction
    {
        foreach (Item item in items)
        {
            if (direction == Direction.up)
            {
                itemController.SetTile(0, item.GetPosition());
                if (item.GetPosition().y == 0)
                    item.SetPosition(new Vector2Int(item.GetPosition().x, item.GetPosition().y + 1));
                if (item.GetPosition().y >= (H - 3))
                {
                    Debug.Log("Item is supposed to fall off here"); //Todo: delete the item (toosleepytofigureitoutnow)
                }
                else
                    item.SetPosition(new Vector2Int(item.GetPosition().x, item.GetPosition().y + 1));
            }
            else if (direction == Direction.down)
            {
                itemController.SetTile(0, item.GetPosition());
                if (item.GetPosition().y == H - 1)
                    item.SetPosition(new Vector2Int(item.GetPosition().x, item.GetPosition().y - 1));
                if (item.GetPosition().y <= 2)
                {
                    Debug.Log("Item is supposed to fall off here");
                }
                else
                    item.SetPosition(new Vector2Int(item.GetPosition().x, item.GetPosition().y - 1));
            }
            else if (direction == Direction.left)
            {
                itemController.SetTile(0, item.GetPosition());
                if (item.GetPosition().x == W - 1)
                    item.SetPosition(new Vector2Int(item.GetPosition().x - 1, item.GetPosition().y));
                if (item.GetPosition().x <= 2)
                {
                    Debug.Log("Item is supposed to fall off here");
                }
                else
                    item.SetPosition(new Vector2Int(item.GetPosition().x - 1, item.GetPosition().y));
            }
            else if (direction == Direction.right)
            {
                itemController.SetTile(0, item.GetPosition());
                if (item.GetPosition().x == 0)
                    item.SetPosition(new Vector2Int(item.GetPosition().x + 1, item.GetPosition().y));
                if (item.GetPosition().x >= (W - 3))
                {
                    Debug.Log("Item is supposed to fall off here");
                }
                else
                    item.SetPosition(new Vector2Int(item.GetPosition().x + 1, item.GetPosition().y));
            }
        }

        itemController.SetItems(items);
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
                    DamagePlayer(i.index, 100);
                    playShowdown.Play();
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

        PercolateItems(waveDirection);
        

        foreach (Movement move in moves)
        {
            MovePlayer(move.index, move.coordinates.x, move.coordinates.y);
        }
        foreach (Movement move in moves)
        {
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
        SpawnItemWave();
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
                    moveClick1.Play();
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
            if (PhotonNetwork.InRoom) {
                SendMoveEvent();
            }
            else {
                moveClick2.Play();
                CycleActivePlayer();
                SecondClick = false;
                //TODO: send move over photon
                System.Threading.Thread.Sleep(100);
            }
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
            if (index == 0)
                deathMenu.ToggleDeathMenu(5);
            //Destroy(CharacterPrefabs[index]);
        }
    }

    // Sends the moves of the active player to every other player
    private void SendFinishInitEvent()
    {
        if (!PhotonNetwork.InRoom) {
            Debug.LogWarning("Finish Init Event called, but we are not connected to a Room!");
            return;
        }

        object[] content = new object[1];
        content[0] = ActivePlayer;
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
        PhotonNetwork.RaiseEvent(FinishInitEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    // Sends the moves of the active player to every other player
    private void SendMoveEvent()
    {
        if (!PhotonNetwork.InRoom) {
            Debug.LogWarning("Move Event called, but we are not connected to a Room!");
            return;
        }

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
