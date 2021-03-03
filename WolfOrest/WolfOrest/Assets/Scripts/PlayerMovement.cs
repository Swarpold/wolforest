using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    //public float jumpForce;

    //public bool isJumping = false;

    public Rigidbody2D rb;
    private Vector3 velocity = Vector3.zero;
   
    



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
