using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fall : MonoBehaviour
{
    public Rigidbody2D rb;
    public float timedeplay;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine(fallll());
        }    
    }
    IEnumerator fallll()
    {
        yield return new WaitForSeconds(timedeplay);
        rb.bodyType = RigidbodyType2D.Dynamic;
        yield return 0;
    }
}
