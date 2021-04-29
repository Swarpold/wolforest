using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class Player_Photon : MonoBehaviour
{
    PhotonView myPV;
    GameObject myPlayerAvatar;

    // Start is called before the first frame update
    void Start()
    {
        myPV = GetComponent<PhotonView>();
        if(myPV.IsMine)
        {
            myPlayerAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "AU_Player"), Vector3.zero, Quaternion.identity);
        }
    }
}
