using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyParent : MonoBehaviour
{
    public GameObject player;

    public int xMovement;
    public bool playerInSight;
    public float turnTimer;
    public float currTurnTimer;
    public bool isIdle;

    public float hitTimer = 0.5f;
    public float currHitTimer = 0;

    public int health;
    public int maxHealth;
    public int damage;
    public float speed;

    public Animator anim;
    public Rigidbody2D rb;

    public int facingDir = -1;
    public float sight;

    public Transform attackCenter;
    public float attackRadius;
    public LayerMask playerLayer;
    public float attackRange;
    public float attackDelay;
    public float currAttackDelay;

    public Vector3 distancePlayer;

    public Transform groundCheckCircle;
    public Transform wallCheckCircle;
    public float groundCheckerRadius;
    public LayerMask groundLayer;
    public Vector2 groundCheckerPos;

    public GameObject coin;

    public AudioSource enemySound;

    // Initialize variables
    public virtual void Start()
    {
        health = maxHealth;
        anim = gameObject.GetComponent<Animator>();
        rb = gameObject.GetComponent<Rigidbody2D>();

        player = FindObjectOfType<PlayerController>().gameObject;

        enemySound = GetComponent<AudioSource>();
    }

    //Apply physics on FixedUpdate
    public virtual void FixedUpdate()
    {
        CheckPlayer();
        Move(speed, xMovement);
        Idle();
        Attack();
    }

    public virtual void Update()
    {
        //Check distance towards the player
        distancePlayer = new Vector3(transform.position.x - player.transform.position.x, transform.position.y - player.transform.position.y, transform.position.y - player.transform.position.y);
        //Change the facing direction of the sprite
        gameObject.transform.localScale = new Vector3(facingDir, 1, 1);
        //Check the enemy's movement and change the animation
        if (xMovement != 0 && !anim.GetBool("isWalking") && !anim.GetBool("isAttacking"))
        {
            anim.SetBool("isWalking", true);
        }
        else if ((xMovement == 0 || anim.GetBool("isAttacking")) && anim.GetBool("isWalking") && !isIdle)
        {
            anim.SetBool("isWalking", false);
        }

        //Reduce the hit timer and manage the blinking so the enemy is not invulnerable for ever
        if (currHitTimer > 0)
        {
            currHitTimer -= Time.deltaTime;
            gameObject.GetComponent<SpriteRenderer>().enabled = Mathf.PingPong(Time.time, 0.1f) > (0.1f / 2f);
        }
        else if (currHitTimer <= 0)
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = true;
            currHitTimer = 0;
        }
    }

    public virtual void CheckPlayer()
    {
        //If the player is in sight of the enemy, move towards them, if they're not, wander around
        if (Mathf.Abs(distancePlayer.x) < sight && Mathf.Abs(distancePlayer.x) > 1 && Mathf.Abs(distancePlayer.y) < 3 && !anim.GetBool("isDead") && !CheckWalls())
        {
            playerInSight = true;
            if (transform.position.x > player.transform.position.x)
            {
                xMovement = -1;
            }
            else
            {
                xMovement = 1;
            }
        }
        else if ((Mathf.Abs(distancePlayer.x) < 1 && Mathf.Abs(distancePlayer.y) < 3 && !CheckWalls()) || anim.GetBool("isDead"))
        {
            playerInSight = true;
            xMovement = 0;
        }
        else
        {
            xMovement = 0;
            playerInSight = false;
        }
    }

    public virtual void Move(float moveSpeed, float moveDirection)
    {
        //Add the movement to the enemy's velocity if it's not attacking
        if (!anim.GetBool("isAttacking"))
        {
            rb.velocity = new Vector2(moveDirection*moveSpeed, rb.velocity.y);
        }

        //Change the facing direction if not attacking depending on the last movement done
        if (!anim.GetBool("isAttacking") && ((moveDirection == 1 && facingDir == -1) || (moveDirection == -1 && facingDir == 1)))
        {
            facingDir *= -1;
        }
    }

    public virtual void Attack()
    {
        //Check the distance towards the player and attack if close enough
        if (Mathf.Abs(distancePlayer.x) < attackRange && Mathf.Abs(distancePlayer.y) < attackRange && currAttackDelay <= 0)
        {
            currAttackDelay = attackDelay;
            rb.velocity = new Vector2(0f, 0f);
            anim.SetBool("isAttacking", true);
            StartCoroutine(BasicAttack());
        }

        if(currAttackDelay > 0)
        {
            currAttackDelay -= Time.deltaTime;
        } else if(currAttackDelay < 0)
        {
            currAttackDelay = 0;
        }
    }
    public virtual IEnumerator BasicAttack()
    {
        //Wait for the enemy to start attacking, then create a hitbox, if the player is in it, hit them
        yield return new WaitForSeconds(0.5f);
        Collider2D collider = Physics2D.OverlapCircle(attackCenter.position, attackRadius, playerLayer);
        if(collider != null)
        {
            collider.GetComponent<PlayerController>().GetHit(damage, gameObject.transform);
        }
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("isAttacking", false);
    }

    public virtual void Idle()
    {
        if (!playerInSight && !anim.GetBool("isAttacking") && !anim.GetBool("isDead"))
        {
            anim.SetBool("isWalking", true);
            isIdle = true;

            //Check the ground check circle to see if there is ground ahead, if not, turn around and walk that way
            groundCheckerPos = new Vector2(groundCheckCircle.position.x + facingDir * 0.3f, groundCheckCircle.position.y);
            Collider2D collider = Physics2D.OverlapCircle(groundCheckerPos, groundCheckerRadius, groundLayer);

            //Check the timer so it has time to turn around
            if ((collider == null && currTurnTimer <= 0) || (CheckWalls() && currTurnTimer <= 0))
            {
                currTurnTimer = turnTimer;
                Move(speed / 2, facingDir*-1);
            }
            else
            {
                Move(speed / 2, facingDir);
            }

            //Advance the turn timer
            if (currTurnTimer > 0)
            {
                currTurnTimer -= Time.deltaTime;
            }
            else if (currTurnTimer < 0)
            {
                currTurnTimer = 0;
            }
        } else
        {
            isIdle = false;
        }
    }

    //Check the wall collider for information of what's infront of the enemy
    public virtual bool CheckWalls()
    {
        Collider2D collider = Physics2D.OverlapCircle(wallCheckCircle.position, groundCheckerRadius, groundLayer);

        return (collider != null);
    }

    //If a player attack collides with the enemy, reduce its health
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PlayerAttack")
        {
            GetHit(collision.transform);
        }
    }

    //Reduce health and check if the enemy is dead, if so, destroy it
    public virtual void GetHit(Transform attacker)
    {
        if (currHitTimer == 0)
        {
            enemySound.Play(0);
            currHitTimer = hitTimer;
            //Add knockback to the enemy as they get hit 
            Vector2 dir = new Vector2(((transform.position - attacker.transform.position).normalized).x * 0.5f, 0);
            GetComponent<Rigidbody2D>().AddForce(dir * 5, ForceMode2D.Impulse);

            health -= attacker.gameObject.GetComponent<ArrowBehaviour>().getDamage();
            
        }

        if (health <= 0)
        {
            anim.SetBool("isDead", true);
            StartCoroutine(Die());
        }
    }
    public virtual IEnumerator Die()
    {
        enemySound.Play(0);
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        GetComponent<Collider2D>().enabled = false; 
        for (int i = 0; i < Random.Range(1, 4); i++)
        {
            float randomX = Random.Range(-0.8f, 0.8f);
            Instantiate(coin, new Vector3(transform.position.x + randomX, transform.position.y, 0), Quaternion.identity);
        }
        this.enabled = false;
        yield return new WaitForSeconds(4f);

    }

    //Visualize the attack range on the insepctor
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackCenter.position, attackRadius);
    }
}
