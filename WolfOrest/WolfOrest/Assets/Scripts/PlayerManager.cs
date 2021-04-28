using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] GameObject camera;
    private Vector2 spawn;
    PhotonView PV;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (PV.IsMine)
        {
            spawn = new Vector2(-2f, 0f);
           //CreateController();
        }
        else
        {
            spawn = new Vector2(2f, 0f);
        }
        var player = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), spawn, Quaternion.identity);
        if (PV.IsMine)
        {
            camera.transform.SetParent(player.transform, false);
        }
    }

    // Update is called once per frame
    /*void CreateController()
    {
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), spawn, Quaternion.identity);
    }*/
}
