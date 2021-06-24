using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using UnityEngine.InputSystem;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerMovement : MonoBehaviourPun, IPunObservable
{
    
    //public float jumpForce;

    //public bool isJumping = false;

    [SerializeField] bool hasControl;
    public static PlayerMovement localPlayer;

    //Components
    Rigidbody myRB;
    Transform myAvatar;
    Animator myAnim;

    //PlayerMovement
    [SerializeField] InputAction WASD;
    Vector2 movementInput;
    [SerializeField] float moveSpeed;
    float direction = 1;

    public bool SetImposters;
    public bool isImposter;
    [SerializeField] InputAction KILL;
    float killInput;

    List<PlayerMovement> targets;
    [SerializeField] Collider myCollider;

    public bool isDead;


    public GameObject bodyPrefab;

    public static List<Transform> allBodies;

    List<Transform> bodiesFound;

    [SerializeField] InputAction REPORT;
    [SerializeField] LayerMask ignoreForBody;

    //Networking
    PhotonView myPV;
    //[SerializeField] GameObject lightMask;
    //[SerializeField] lightcaster myLightCaster;

    public GameObject[] players;
    public GameObject[] tasks;


    //public Rigidbody2D rb;
    private Vector3 velocity = Vector3.zero;

    //Interaction
    [SerializeField] InputAction MOUSE;
    Vector2 mousePositionInput;
    Camera myCamera;
    [SerializeField] InputAction INTERACTION;
    [SerializeField] LayerMask interactLayer;


    void Awake()
    {
      
        KILL.performed += KillTarget;
        REPORT.performed += ReportBody;
        INTERACTION.performed += Interact;
        //rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        
        WASD.Enable();
        KILL.Enable();
        REPORT.Enable();
        MOUSE.Enable();
        INTERACTION.Enable();
    }

    private void OnDisable()
    {
        
        WASD.Disable();
        KILL.Disable();
        REPORT.Disable();
        MOUSE.Disable();
        INTERACTION.Disable();
    }

    void Start()
    {
        isImposter = false;
        SetImposters = false;

        myPV = GetComponent<PhotonView>();

        targets = new List<PlayerMovement>();

        myCamera = transform.Find("Main Camera").GetComponent<Camera>();
        myRB = GetComponent<Rigidbody>();
        myAvatar = transform.GetChild(0);
        myAnim = GetComponent<Animator>();
        allBodies = new List<Transform>();

        bodiesFound = new List<Transform>();

        


    }

    void Update()
    {
        if (players.Length == 0)
        {
            players = GameObject.FindGameObjectsWithTag("Player");
        }

        if (tasks.Length == 0)
        {
            tasks = GameObject.FindGameObjectsWithTag("Interactable");
        }

        if (!myPV.IsMine)
        {
            return;
        }

        if (isDead == true)
        {
            return;
        }

        if (SetImposters == false)
        {
            int enemyPlayer = Random.Range(0, PhotonNetwork.PlayerList.Length);
            photonView.RPC("setImposter", PhotonNetwork.PlayerList[enemyPlayer]);
            SetImposters = true;
        }
        

        myAvatar.localScale = new Vector2(direction, 1);

        movementInput = WASD.ReadValue<Vector2>();
        myAnim.SetFloat("Speed", movementInput.magnitude);

        if (movementInput.x != 0)
        {
            direction = Mathf.Sign(movementInput.x);
        }

        if (allBodies.Count > 0)
        {
            BodySearch();
        }

        mousePositionInput = MOUSE.ReadValue<Vector2>();

        if (isImposter && Input.GetKeyDown(KeyCode.E))
        {
            players = GameObject.FindGameObjectsWithTag("Player");
            float i = float.MaxValue;
            GameObject playerToKill = null;
            foreach (GameObject player in players)
            {
                if (Vector2.Distance(gameObject.transform.position, player.transform.position) < i && !player.Equals(gameObject))
                {
                    playerToKill = player;
                    i = Vector3.Distance(gameObject.transform.position, player.transform.position);
                }
            }
            Debug.Log(i);
            Debug.Log("tried to kill");
            if (i < 6)
            {

                Debug.Log("killed");
                //playerToKill.GetComponent<Animator>().SetBool("dead", true);
                var photonOfKilledPlayer = playerToKill.GetComponent<PhotonView>();
                if (photonOfKilledPlayer != null)
                    photonView.RPC("killed", photonOfKilledPlayer.Owner);
                gameObject.transform.position = playerToKill.gameObject.transform.position;
                //PhotonNetwork.Instantiate("Blood", gameObject.transform.position, Quaternion.identity);
            }
        }

        

    }

    void FixedUpdate()
    {
        
        if (!myPV.IsMine)
        {
            return;
        }

        myRB.velocity = movementInput * moveSpeed;
        
    }

    public void SetRole(bool newRole)
    {
        
        isImposter = newRole;
    }


    private void OnTriggerEnter(Collider other)
    {
        
        if (other.tag == "Player")
        {
            PlayerMovement tempTarget = other.GetComponent<PlayerMovement>();
            if (isImposter)
            {
                if (tempTarget.isImposter)
                    return;
                else
                {
                    targets.Add(tempTarget);
                    //Debug.Log(target.name);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
        if (other.tag == "Player")
        {
            PlayerMovement tempTarget = other.GetComponent<PlayerMovement>();
            if (targets.Contains(tempTarget))
            {
                targets.Remove(tempTarget);
            }
        }
    }

    void KillTarget(InputAction.CallbackContext context)
    {
        if (!myPV.IsMine) { return; }
        if (!isImposter) { return;  }
        if (context.phase == InputActionPhase.Performed)
        {
            if (targets.Count == 0)
                return;
            else
            {
                if (targets[targets.Count - 1].isDead)
                    return;
                transform.position = targets[targets.Count - 1].transform.position;
                //targets[targets.Count - 1].Die();
                targets[targets.Count - 1].myPV.RPC("RPC_Kill", RpcTarget.All);
                targets.RemoveAt(targets.Count - 1);
            }
        }
    }

    [PunRPC]
    void RPC_Kill()
    {
        Die();
    }

    [PunRPC]
    public void Die()
    {
        if (!myPV.IsMine) { return; }

        //AU_Body tempBody = Instantiate(bodyPrefab, transform.position, transform.rotation).GetComponent<AU_Body>();
        AU_Body tempBody = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "AU_Body"),transform.position , transform.rotation).GetComponent<AU_Body>();

        isDead = true;
        Debug.Log("is Dead");
        myAnim.SetBool("IsDead", isDead);
        gameObject.layer = 9;
        myCollider.enabled = false;

        
    }


    void BodySearch()
    {
        
        foreach (Transform body in allBodies)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, body.position - transform.position);
            //Debug.DrawRay(transform.position, body.position  transform.position, Color.cyan);
            if (Physics.Raycast(ray, out hit, 1000f, ~ignoreForBody))
            {
                if (hit.transform == body)
                {
                    //Debug.Log(hit.transform.name);
                    //Debug.Log(bodiesFound.Count);
                    if (bodiesFound.Contains(body.transform))
                        return;
                    bodiesFound.Add(body.transform);
                }
                else
                {
                    bodiesFound.Remove(body.transform);
                }

            }
        }
    }

    [PunRPC]
    public void ReportBody(InputAction.CallbackContext obj)
    {
        Debug.Log("a body has been found");
        if (bodiesFound == null)
            return;
        if (bodiesFound.Count == 0)
            return;
        Transform tempBody = bodiesFound[bodiesFound.Count - 1];
        allBodies.Remove(tempBody);
        bodiesFound.Remove(tempBody);
        tempBody.GetComponent<AU_Body>().Report();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(direction);
        }
        else
        {
            direction = (float)stream.ReceiveNext();
        }
    }


    [PunRPC]
    void setImposter()
    {
        Hashtable hash = new Hashtable();
        hash.Add("isImposter", true);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        if (photonView.IsMine)
        {
            isImposter = true;
        }
        var testPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in testPlayers)
        {
            if (player.GetPhotonView().IsMine)
            {
                player.GetComponent<PlayerMovement>().isImposter = true;
            }
        }
        Debug.Log("You are the imposter.");
    }

    [PunRPC]
    void killed()
    {
        if (!photonView.IsMine)
            Debug.Log("called killed");
        var testPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in testPlayers)
        {
            //if (player.GetPhotonView().IsMine)
            {
                AU_Body tempBody = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "AU_Body"), transform.position, transform.rotation).GetComponent<AU_Body>();
                
                Debug.Log("is Dead");
                myAnim.SetBool("IsDead", isDead);
                gameObject.layer = 9;
                PhotonNetwork.Destroy(player);
                //player.SetActive(false);

                player.GetComponent<PlayerMovement>().isDead = true;
                Hashtable hash = new Hashtable();
                hash.Add("isDead", true);
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
                //player.GetComponent<Animator>().SetBool("dead", true);
            }
        }

    }

    [PunRPC]
    void Interact(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            RaycastHit hit;
            Ray ray = myCamera.ScreenPointToRay(mousePositionInput);
            if (Physics.Raycast(ray, out hit, interactLayer))
            {
                Debug.Log("2");
                if (hit.transform.tag == "Interactable")
                {
                    Debug.Log("3");
                    if (!hit.transform.GetChild(0).gameObject.activeInHierarchy)
                        return;
                    
                    Debug.Log("4");
                    AU_Interactable temp = hit.transform.GetComponent<AU_Interactable>();
                    temp.PlayMiniGame();
                }
            }


            /*tasks = GameObject.FindGameObjectsWithTag("Interactable");
            float i = float.MaxValue;
            AU_Interactable tasktoachieve = context.transform.GetComponent<AU_Interactable>();
            foreach (GameObject task in tasks)
            {
                if (Vector2.Distance(gameObject.transform.position, task.transform.position) < i && !task.Equals(gameObject))
                {
                    tasktoachieve = task;
                    i = Vector3.Distance(gameObject.transform.position, task.transform.position);
                }
            }
            Debug.Log(i);
            Debug.Log("tried to achieve");
            if (i < 6)
            {

                Debug.Log("achieve");
                tasktoachieve.PlayMiniGame();*/
           
            
        }
        
    }

 
}
