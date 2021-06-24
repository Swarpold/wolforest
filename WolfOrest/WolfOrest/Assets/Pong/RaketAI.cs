using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaketAI : Rackets
{
    
    Rigidbody2D rb2;
    public Transform ball;

    private void Start()
    {
        rb2 = GetComponent<Rigidbody2D>();
    }
    protected override void FixedUpdate()
    {
        float topRaketFarkY = Mathf.Abs(ball.position.y - transform.position.y);
        if (topRaketFarkY > 2)
        {
            if (ball.position.y > transform.position.y)
             {
                 rb2.velocity = Vector2.up * moveSpeed;
             }
             else
            rb2.velocity = Vector2.down * moveSpeed;

        }
      
    }
  
}
