using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;

    public Rigidbody2D rb;
    private Vector3 velocity = Vector3.zero;
   

    void FixeUpdate()
    {
        float horizontalmouv = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;

        MovePlayer(horizontalmouv);
    }

    void MovePlayer(float _horizontalmouv)
    {
        Vector3 targetVelocity = new Vector2(_horizontalmouv, rb.velocity.y);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, .05f);
    }
}
