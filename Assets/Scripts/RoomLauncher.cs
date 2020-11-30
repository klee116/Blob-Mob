using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomLauncher : MonoBehaviourPunCallbacks
{
    public const string SEED_PROP_KEY = "seed";

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
}
