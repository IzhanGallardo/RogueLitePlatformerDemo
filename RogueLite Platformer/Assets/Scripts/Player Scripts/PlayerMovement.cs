using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    private float x_movement;
    public float spd;
    public float runSpd;
    public float attackSpeed;

    public float jumpForce;
    public bool isGrounded = false;
    public Transform groundCheckCircle;
    public float groundCheckerRadius;
    public LayerMask groundLayer;
    public bool touchingWall = false;

    private Rigidbody2D rb;
    private Animator anim;

    public int facingDir = 1;
    public bool isVulnerable = true;
    private bool flyHit = false;

    private bool footstepsLooping = false;

    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody2D>();
        anim = this.gameObject.GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        //Call the movement methods on Fixed Update so the physics are applied before anything else
        if (!anim.GetBool("isDead") && !gameObject.GetComponent<PlayerController>().isOnShop)
        {
            Move();
            Jump();
            Roll();
            CheckGrounded();
        }
    }

    private void Update()
    {
        if (!anim.GetBool("isDead") && !gameObject.GetComponent<PlayerController>().isOnShop)
        {
            //Update stats
            spd = GetComponent<PlayerController>().moveSpeed;
            runSpd = spd * 1.2f;
            attackSpeed = GetComponent<PlayerController>().attackSpeed;

            //Update attack animation depending on attack speed
            anim.SetFloat("animSpeed", attackSpeed);

            //Change the facing direction of the sprite
            gameObject.transform.localScale = new Vector3(facingDir, 1, 1);
            //Check if the player is walking on the ground (and not rolling or attacking), if so and if it isn't true yet, set the walking animation to true
            //If the walking animation is true and the player is no longer moving or on the ground, set it to false
            if (x_movement != 0 && isGrounded && !anim.GetBool("isWalking") && !anim.GetBool("isRolling") && !anim.GetBool("isAttacking"))
            {
                anim.SetBool("isWalking", true);
            }
            else if ((x_movement == 0 || !isGrounded || anim.GetBool("isRolling") || anim.GetBool("isAttacking") || anim.GetBool("isJumping")) && anim.GetBool("isWalking"))
            {
                anim.SetBool("isWalking", false);
            }

            //Check for the Attack trigger and shoot an arrow
            Attack();

        }
    }

    private void Jump()
    {
        //If the Space key is pressed and the player is on the ground, add the jumpForce to the y velocity
        if (Input.GetKey(KeyCode.Space) && isGrounded || flyHit)
        {
            flyHit = false;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    private void Move()
    {
        //Get the horizontal movement input
        x_movement = Input.GetAxisRaw("Horizontal");
        //Create the movement variable to apply the right speed in case shift is pressed (so the player can run)
        float movement;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            movement = x_movement * runSpd;
        }
        else
        {
            movement = x_movement * spd;
        }

        //Add the movement to the player's velocity if it's not attacking
        if (!anim.GetBool("isAttacking"))
        {
            if (isGrounded && x_movement != 0)
            {
                if (!footstepsLooping)
                {
                    footstepsLooping = true;
                    GetComponent<PlayerController>().AudioLoop(true);
                    GetComponent<PlayerController>().PlayAudio(GetComponent<PlayerController>().footstepsSound);
                }
            } else if (x_movement == 0 || !isGrounded)
            {
                GetComponent<PlayerController>().AudioLoop(false);
                footstepsLooping = false;
            }
            rb.velocity = new Vector2(movement, rb.velocity.y);
        }

        //Change the facing direction depending on the last movement done
        if(((x_movement == 1 && facingDir == -1) || (x_movement == -1 && facingDir == 1)) && !anim.GetBool("isAttacking"))
        {
            facingDir *= -1;
        }
    }

    private void CheckGrounded()
    {
        //Create a small circle on the bottom of the player to check its collision with the ground and determine if the player is grounded or not
        Collider2D collider = Physics2D.OverlapCircle(groundCheckCircle.position, groundCheckerRadius, groundLayer);

        if (collider != null)
        {
            isGrounded = true;
        } else
        {
            isGrounded = false;
        }
    }

    private void Roll()
    {
        //Check if the key is pressed and roll towards the moving direction while being invunerable
        if(Input.GetKey(KeyCode.LeftControl) && anim.GetBool("isWalking"))
        {
            anim.SetBool("isRolling", true);

            StartCoroutine(Invulerability());
        }
    }
    IEnumerator Invulerability()
    {
        isVulnerable = false;

        yield return new WaitForSeconds(0.65f);

        isVulnerable = true;
        anim.SetBool("isRolling", false);
    }

    //Set the death animation to true when it's called
    public void Die()
    {
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        GetComponent<Collider2D>().enabled = false;
        anim.SetBool("isDead", true);
        StartCoroutine(DeathDelay());
    }
    IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length + 0.5f);
        transform.position = new Vector3(-21, -3f, 0);
        anim.SetBool("isDead", false);
        GetComponent<Collider2D>().enabled = true;
        rb.isKinematic = false;
        SceneManager.LoadScene("DeadScreen");
    }

    //Check if the attack button has been pressed, and if so and the player is not already attacking, stop and shoot an arrow.
    private void Attack()
    {
        if (Input.GetKeyDown(KeyCode.C) && !anim.GetBool("isAttacking"))
        {
            rb.velocity = new Vector2(0f, 0f);
            anim.SetBool("isAttacking", true);
            StartCoroutine(ShootArrow());
        }
    }
    IEnumerator ShootArrow()
    {
        yield return new WaitForSeconds((1/attackSpeed) * 0.8f);
        GetComponent<PlayerController>().ShootArrow(facingDir);
        yield return new WaitForSeconds((1/attackSpeed) * 0.2f);
        anim.SetBool("isAttacking", false);
    }

    //If an enemy trigger hits the player, make them jump and get hit
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && isVulnerable)
        {
            flyHit = true;
            GetComponent<PlayerController>().GetHit(collision.GetComponent<EnemyParent>().damage, collision.transform);
        } else if (collision.CompareTag("Spike") && isVulnerable)
        {
            GetComponent<PlayerController>().GetHit(1, collision.transform);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Enemy") && isVulnerable)
        {
            GetComponent<PlayerController>().GetHit(collision.gameObject.GetComponent<EnemyParent>().damage, collision.gameObject.transform);
        }
    }
}
