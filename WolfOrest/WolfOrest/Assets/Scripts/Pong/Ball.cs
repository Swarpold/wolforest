using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class Ball : MonoBehaviour
{
    Rigidbody2D rb2;
    public float moveSpeed;
    public Rackets solRaket, sagRaket;
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        rb2 = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        rb2.velocity = new Vector2(1, 0) * moveSpeed;

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        audioSource.Play();
        if (collision.gameObject.GetComponent<tagManager>() == null) return;

        Tags tags = collision.gameObject.GetComponent<tagManager>().tags;
        if (tags == Tags.SOL_KALE)
        {
            sagRaket.makeScore();
        }
        if (tags == Tags.SAG_KALE)
        {
            solRaket.makeScore();
        }
        if (tags == Tags.SOL_RAKET)
        {
            SetReturnVelocity(1, collision);
        }
        if (tags == Tags.SAG_RAKET)
        {
            SetReturnVelocity(-1, collision);
        }

    }

    private void SetReturnVelocity(int x, Collision2D collision)
    {
        float a = transform.position.y - collision.collider.bounds.center.y;
        float b = collision.collider.bounds.size.y;
        float y = a / b;
        rb2.velocity = new Vector2(x, y) * moveSpeed;


    }
}
