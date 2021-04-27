using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    //public float jumpForce;

    //public bool isJumping = false;

    public Rigidbody2D rb;
    private Vector3 velocity = Vector3.zero;
<<<<<<< HEAD
=======

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }



>>>>>>> ae51f8b644f588d3525294109bac9d47c8f08a14

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    void FixedUpdate()
    {
        float horizontalMovement = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float verticalMovement = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        /*if(Input.GetButtonDown("Jump"))
        {
            isJumping = true;
        }*/

        MovePlayer(horizontalMovement);
        MovePlayer2(verticalMovement);
<<<<<<< HEAD
        rb.MovePosition(rb.position + new Vector2(horizontalMovement,verticalMovement) * Time.fixedDeltaTime);
=======
        rb.MovePosition(rb.position + new Vector2(1f,0f) * Time.fixedDeltaTime);
>>>>>>> ae51f8b644f588d3525294109bac9d47c8f08a14
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
