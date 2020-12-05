using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomLauncher : MonoBehaviourPunCallbacks
{
    public const string SEED_PROP_KEY = "seed";
    public int currRoomSeed;

    [SerializeField]
    private byte maxPlayersPerRoom = 4;
    // Change this when we make modifications that would desync clients
    [SerializeField]
    private string gameVersion = "1";



    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        ConnectToMaster();
    }

    public void ConnectToMaster()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = gameVersion;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server");
        // test join any room
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("failed to join any room! created a room to test in");

        System.Random seedGen = new System.Random();
        int seed = seedGen.Next();

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.CustomRoomPropertiesForLobby = new string[]{ SEED_PROP_KEY };
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
        roomOptions.CustomRoomProperties.Add(SEED_PROP_KEY, seed);

        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Now in room " + PhotonNetwork.CurrentRoom.ToString());
    }

    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.LogFormat("Someone else entered the room: {0}", other.NickName); // not seen if you're the player connecting


        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
            LoadLevel();
        }
        
    }

    private void LoadLevel()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("Trying to Load a level but we are not the master client o no");
            PhotonNetwork.LoadLevel("ProcGenTest");
        }
    }
}
