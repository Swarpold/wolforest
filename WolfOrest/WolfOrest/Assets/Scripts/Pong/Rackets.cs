using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Rackets : MonoBehaviour
{
    public float moveSpeed;
    int score;
    public Text racketScore;
    Rigidbody2D rb2;
    public string axisName;
    // Start is called before the first frame update
    void Start()
    {
        rb2 = GetComponent<Rigidbody2D>();
    }
    protected virtual void FixedUpdate()
    {
        float moveAxis = Input.GetAxis(axisName);
        rb2.velocity = new Vector2(0, moveAxis) * moveSpeed;
    }

    public void makeScore()
    {
        score++;
        racketScore.text = score.ToString();
    }
}
