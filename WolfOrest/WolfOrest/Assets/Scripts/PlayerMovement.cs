using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPun
{
    public float moveSpeed;
    //public float jumpForce;

    //public bool isJumping = false;

    public Rigidbody2D rb;
    private Vector3 velocity = Vector3.zero;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        float horizontalMovement = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float verticalMovement = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        /*if(Input.GetButtonDown("Jump"))
        {
            isJumping = true;
        }*/
        rb.MovePosition(rb.position + new Vector2(horizontalMovement, verticalMovement) * Time.fixedDeltaTime);
        MovePlayer(horizontalMovement);
        MovePlayer2(verticalMovement);
    }


    void MovePlayer(float _horizontalMovement)
    {
        Vector3 targetVelocity = new Vector2(_horizontalMovement, rb.velocity.y);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, .05f);

        /*if(isJumping == true)
        {
            rb.AddForce(new Vector2(0f, jumpForce));
            isJumping = false;
        }*/
    }

    void MovePlayer2(float _verticalMovement)
    {
        Vector3 targetVelocity = new Vector2(rb.velocity.x, _verticalMovement);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, .05f);
    }
}
