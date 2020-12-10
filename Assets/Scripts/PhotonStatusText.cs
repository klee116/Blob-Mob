using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PhotonStatusText : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {  
        Photon.Realtime.ClientState status = PhotonNetwork.NetworkClientState;
        if (status == Photon.Realtime.ClientState.Joined)
        {
            GetComponent<TMPro.TMP_Text>().text = "Waiting for players";
        }
        else
        {
            GetComponent<TMPro.TMP_Text>().text = status.ToString();
        }
    }
}
