using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class Launcher : MonoBehaviourPunCallbacks
{
    void Start()
    {
        Debug.Log("Connecting to Master");
        PhotonNetwork.ConnectUsingSettings();
    }


    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
    }
    
    void Update()
    {
        
    }
}
