using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    GameObject myPlayerAvatar;
    [SerializeField] GameObject camera;
    private Vector2 spawn;
    PhotonView PV;
   
    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
        {
            //myPlayerAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), Vector3.zero, Quaternion.identity);
            spawn = new Vector2(-2f, 0f);
            

            //CreateController();
        }
        else
        {
            spawn = new Vector2(2f, 0f);
        }
        var player = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), spawn, Quaternion.identity);
        camera.transform.SetParent(player.transform, false);


    }

    // Update is called once per frame
    /*void CreateController()
    {
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), spawn, Quaternion.identity);
    }*/
}
