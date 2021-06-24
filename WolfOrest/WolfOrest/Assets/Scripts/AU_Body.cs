using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AU_Body : MonoBehaviour
{
    [SerializeField] SpriteRenderer bodySprite;

    /*public void SetColor(Color newColor)
    {
        bodySprite.color = newColor;
    }*/

    private void OnEnable()
    {
        if (PlayerMovement.allBodies != null)
        {
            PlayerMovement.allBodies.Add(transform);
        }
    }

    public void Report()
    {
        Debug.Log("Reported");
        PhotonNetwork.Destroy(gameObject);
    }
}
