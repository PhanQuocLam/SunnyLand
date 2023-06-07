using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thang : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D coll;
    public float speed;
    public Transform top, bottom;
    private bool isUp=true;
    private float TopY, BotY;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll= GetComponent<Collider2D>();
        TopY = top.position.y;
        BotY = bottom.position.y;
        Destroy(top.gameObject);
        Destroy(bottom.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }
    void Movement()
    {
        if (isUp)
        {
            rb.velocity = new Vector2(rb.velocity.x, speed);
            if(transform.position.y>TopY)
            {
                isUp = false;

            } 

        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x, -speed);
            if (transform.position.y < BotY)
            {
                isUp = true;

            }
        }
    }
}
