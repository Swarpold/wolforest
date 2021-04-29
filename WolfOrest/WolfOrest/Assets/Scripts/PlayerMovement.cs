using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using UnityEngine.InputSystem;

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

    [SerializeField] bool isImposter;
    [SerializeField] InputAction KILL;
    float killInput;

    List<PlayerMovement> targets;
    [SerializeField] Collider myCollider;

    bool isDead;


    [SerializeField] GameObject bodyPrefab;

    public static List<Transform> allBodies;

    List<Transform> bodiesFound;

    [SerializeField] InputAction REPORT;
    [SerializeField] LayerMask ignoreForBody;

    //Networking
    PhotonView myPV;
    //[SerializeField] GameObject lightMask;
    //[SerializeField] lightcaster myLightCaster;


    //public Rigidbody2D rb;
    private Vector3 velocity = Vector3.zero;




    void Awake()
    {
      
        KILL.performed += KillTarget;
        REPORT.performed += ReportBody;
        //rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        
        WASD.Enable();
        KILL.Enable();
        REPORT.Enable();
    }

    private void OnDisable()
    {
        
        WASD.Disable();
        KILL.Disable();
        REPORT.Disable();
    }

    void Start()
    {
        myPV = GetComponent<PhotonView>();

        if (myPV.IsMine)
        {
            localPlayer = this;
        }
        targets = new List<PlayerMovement>();
        //myCamera = transform.GetChild(1).GetComponent<Camera>();
        myRB = GetComponent<Rigidbody>();
        myAvatar = transform.GetChild(0);
        myAnim = GetComponent<Animator>();
        allBodies = new List<Transform>();

        bodiesFound = new List<Transform>();

        if (!myPV.IsMine)
        {
            //myCamera.gameObject.SetActive(false);
            //lightMask.SetActive(false);
            return;
        }

        
    }

    void Update()
    {
        myAvatar.localScale = new Vector2(direction, 1);

        if (!myPV.IsMine)
        {
            return;
        }

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

    private void ReportBody(InputAction.CallbackContext obj)
    {
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

    /*void MovePlayer(float _horizontalMovement)
    {
        Vector3 targetVelocity = new Vector2(_horizontalMovement, rb.velocity.y);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, .05f);

        if(isJumping == true)
        {
            rb.AddForce(new Vector2(0f, jumpForce));
            isJumping = false;
        }
    }

    void MovePlayer2(float _verticalMovement)
    {
        Vector3 targetVelocity = new Vector2(rb.velocity.x, _verticalMovement);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, .05f);
    }*/
}
