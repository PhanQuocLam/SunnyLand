using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public GameObject enterDialog;
    private Rigidbody2D rb;
    private Animator anim;
    public Collider2D coll;
    public Collider2D DisColl;
    public Transform CellingCheck,GroundCheck;
    // public AudioSource jumpAudio,hurtAudio,cherryAudio;    
    public float speed;
    public float JumpForce;
    public LayerMask ground;
    public LayerMask thang;
    private int Cherry;
    public Text CherryNum;

    //health player
    public int ourHealth;
    public int maxhealth = 5;

    private bool isHurt;
    private bool isGround;
    private int extraJump;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        //Khi qua màn mới thì máu đc hồi lại tối đa
        ourHealth = maxhealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isHurt)
        {
            /*Jump();*/
            newJump();
        }
        isGround = Physics2D.OverlapCircle(GroundCheck.position, 0.2f, ground);
        Crouch();
    
        CherryNum.text = Cherry.ToString();

    }
   /* void Jump()
    {
        if (Input.GetButtonDown("Jump") && coll.IsTouchingLayers(ground))
        {
            rb.velocity = new Vector2(rb.velocity.x, JumpForce);
            jumpAudio.Play();
            anim.SetBool("jumping", true);
        }
    }*/
    void newJump()
    {
        if (isGround)
        {
            extraJump = 1;
        }
        if(Input.GetButtonDown("Jump") && extraJump > 0)
        {
            rb.velocity = Vector2.up * JumpForce;
            extraJump--;
            SoundManager.instance.JumpAudio();
            anim.SetBool("jumping", true);
        }
        if (Input.GetButtonDown("Jump") && extraJump == 0 && isGround)
        {
            rb.velocity = Vector2.up * JumpForce;
            SoundManager.instance.JumpAudio();
            anim.SetBool("jumping", true);
        }
    }
    
    void FixedUpdate()
    {
        if (!isHurt)
        {   
            Movement();
        }
        SwitchAnim();    
        
        //Kiểm tra máu của người chơi 
        if(ourHealth <= 0)
        {
            Death();
        }
    }

    void Movement()
    {
        float horizontalMove = Input.GetAxis("Horizontal"); 
        float facedircetion = Input.GetAxisRaw("Horizontal");
       
        if (horizontalMove !=0)
        {
            rb.velocity = new Vector2(horizontalMove * speed * Time.deltaTime, rb.velocity.y);
            anim.SetFloat("running", Mathf.Abs(horizontalMove));
        }

        if (facedircetion !=0)
        {
            transform.localScale = new Vector3(facedircetion, 1, 1);
        }
    }  

    void SwitchAnim()
    {
        anim.SetBool("idle", false);

        if(rb.velocity.y<0.1f && !coll.IsTouchingLayers(ground))
        {
            anim.SetBool("falling", true);
        }
        if (anim.GetBool("jumping"))
        {
            if(rb.velocity.y < 0)
            {
                anim.SetBool("jumping", false);
                anim.SetBool("falling", true);
            }
        } else if (isHurt)
        {
            anim.SetBool("hurt", true);
            anim.SetFloat("running", 0);
            if (Mathf.Abs(rb.velocity.x) < 0.1f)
            {
                anim.SetBool("hurt", false);
                anim.SetBool("idle", true);
                isHurt = false;
            }
        }
        else if (coll.IsTouchingLayers(ground))
        {
            anim.SetBool("falling", false);
            anim.SetBool("idle", true);
        }
       


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Door"&& Cherry>=8)
        {
           enterDialog.SetActive(true);
        }
            if (collision.tag == "thang")
        {
            anim.SetBool("climb", true);
        }
        else if(collision.tag == "ground")
        {
            anim.SetBool("climb", false);
            anim.SetBool("idle", true);
        }
        //else if (coll.IsTouchingLayers(ground))
        //{
        //    anim.SetBool("climb", false);
        //    anim.SetBool("idle", true);
        //}
        if (collision.tag == "Collection")
        {
            SoundManager.instance.CherryAudio();
            //Destroy(collision.gameObject);
            //Cherry += 1;
            collision.GetComponent<Animator>().Play("isGot");
            //CherryNum.text = Cherry.ToString();
        }
        if (collision.tag == "DeadLine")
        {
            Application.LoadLevel("GameOver");
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (anim.GetBool("falling"))
            {
                enemy.JumpOn();
                rb.velocity = new Vector2(rb.velocity.x, JumpForce);
                anim.SetBool("jumping", true);
            }
            else if (transform.position.x < collision.gameObject.transform.position.x)
            {
                Damage(1);
                rb.velocity = new Vector2(-5, rb.velocity.y);
                SoundManager.instance.HurtAudio();
                isHurt = true;
            }
            else if(transform.position.x > collision.gameObject.transform.position.x)
            {
                Damage(1);
                rb.velocity = new Vector2(5, rb.velocity.y);
                SoundManager.instance.HurtAudio();
                isHurt = true;
            }
        }
    }
   
    void Crouch()
    {
        if (!Physics2D.OverlapCircle(CellingCheck.position,0.2f,ground))
        {
            if (Input.GetButton("Crouch"))
            {
                anim.SetBool("crouching", true);
                DisColl.enabled = false;
            }
            else
            {
                anim.SetBool("crouching", false);
                DisColl.enabled = true;
            }
        }
    }
    
       
    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);    
    }
    public void CherryCount()
    {
        Cherry += 1;
        Scene ActiveScreen = SceneManager.GetActiveScene();
    
        if (ActiveScreen.buildIndex == 2)
        {
            if (Cherry == 9)
            {
                Application.LoadLevel("Win");
            }
        }
    }

    //Khi Player die
    public void Death()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //Hàm mất máu player
    public void Damage(int damage)
    {
        ourHealth -= damage;
    }
}
