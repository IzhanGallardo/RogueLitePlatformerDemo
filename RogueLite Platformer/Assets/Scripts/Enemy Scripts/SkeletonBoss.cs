using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkeletonBoss : EnemyParent
{
    public float dashTimer;
    private float currDashTimer;
    public bool notTurned = false;

    public override void Start()
    {
        base.Start();
        currDashTimer = dashTimer;
    }
    public override void FixedUpdate()
    {
        if (!anim.GetBool("isDashing") && !anim.GetBool("isStun"))
        {
            CheckPlayer();
            Move(speed, xMovement);
            Idle();
            Attack();
        }
    }

    public override void Update()
    {
        //Check distance towards the player
        distancePlayer = new Vector3(transform.position.x - player.transform.position.x, transform.position.y - player.transform.position.y, transform.position.y - player.transform.position.y);
        //Change the facing direction of the sprite
        gameObject.transform.localScale = new Vector3(facingDir, 1, 1);
        //Check the enemy's movement and change the animation
        if (xMovement != 0 && !anim.GetBool("isWalking") && !anim.GetBool("isAttacking") && !anim.GetBool("isDashing") && !anim.GetBool("isStun"))
        {
            anim.SetBool("isWalking", true);
        }
        else if ((xMovement == 0 || anim.GetBool("isAttacking") || anim.GetBool("isDashing") || anim.GetBool("isStun")) && anim.GetBool("isWalking") && !isIdle)
        {
            anim.SetBool("isWalking", false);
        }

        //Reduce the hit timer (and dash timer) and manage the blinking so the enemy is not invulnerable for ever
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

        if (currDashTimer > 0)
        {
            currDashTimer -= Time.deltaTime;
        }
        else if (currDashTimer < 0)
        {
            currDashTimer = 0;
        }

        //Control the dashing
        if (anim.GetBool("isDashing"))
        {
            rb.isKinematic = true;
            GetComponent<CapsuleCollider2D>().isTrigger = true;
            if (CheckWalls())
            {
                currDashTimer = dashTimer;
                anim.SetBool("isStun", true);
                anim.SetBool("isDashing", false);
            }
        }

        //Activate the stun effect
        if (anim.GetBool("isStun"))
        {
            StartCoroutine(Stun());
        }
    }

    public override void Move(float moveSpeed, float moveDirection)
    {
        //Add the movement to the enemy's velocity if it's not attacking
        if (!anim.GetBool("isAttacking"))
        {
            rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);
        }

        //Change the facing direction if not attacking depending on the last movement done
        if (!anim.GetBool("isDashing") && !anim.GetBool("isAttacking") && !anim.GetBool("isStun") && ((moveDirection == 1 && facingDir == -1) || (moveDirection == -1 && facingDir == 1)))
        {
            transform.position = new Vector3(transform.position.x + (-2 * facingDir), transform.position.y, 0);
            facingDir *= -1;
        }
    }

    public override void Idle()
    {
        if (!playerInSight && !anim.GetBool("isAttacking") && !anim.GetBool("isDead"))
        {
            anim.SetBool("isWalking", true);
            isIdle = true;

            //Check the ground check circle to see if there is ground ahead, if not, turn around and walk that way
            groundCheckerPos = new Vector2(groundCheckCircle.position.x + facingDir * 0.3f, groundCheckCircle.position.y);
            Collider2D collider = Physics2D.OverlapCircle(groundCheckerPos, groundCheckerRadius, groundLayer);

            //Check the timer so it has time to turn around, move the boss when turned
            if ((collider == null && currTurnTimer <= 0) || (CheckWalls() && currTurnTimer <= 0) || (notTurned && !anim.GetBool("isDashing")))
            {
                notTurned = false;
                currTurnTimer = turnTimer;
                Move(speed / 2, facingDir * -1);
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
        }
        else
        {
            isIdle = false;
        }
    }

    public override void Attack()
    {
        //Check the distance towards the player and attack if close enough
        if (Mathf.Abs(distancePlayer.x) < attackRange && Mathf.Abs(distancePlayer.y) < attackRange && currAttackDelay <= 0 && currDashTimer > 0)
        {
            currAttackDelay = attackDelay;
            rb.velocity = new Vector2(0f, 0f);
            anim.SetBool("isAttacking", true);
            StartCoroutine(BasicAttack());
        }
        else if (Mathf.Abs(distancePlayer.x) < attackRange && Mathf.Abs(distancePlayer.y) < attackRange && currAttackDelay <= 0 && currDashTimer <= 0)
        {
            //If the dash timer is done, dash towards the player if it's on attack range
            anim.SetBool("isDashing", true);
            Dash(-Mathf.Sign(distancePlayer.x));
        }

        if (currAttackDelay > 0)
        {
            currAttackDelay -= Time.deltaTime;
        }
        else if (currAttackDelay < 0)
        {
            currAttackDelay = 0;
        } 
    }

    private void Dash(float dir)
    {
        if (anim.GetBool("isDashing"))
        {
            xMovement = (int)Mathf.Floor(dir);
            Move(speed * 2, dir);
        }
    }

    private IEnumerator Stun()
    {
        notTurned = true;
        GetComponent<CapsuleCollider2D>().isTrigger = false;
        rb.isKinematic = false;
        yield return new WaitForSeconds(this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length * 4);
        anim.SetBool("isStun", false);
        anim.SetBool("isWalking", true);
    }

    public override IEnumerator Die()
    {
        enemySound.Play(0);
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        GetComponent<Collider2D>().enabled = false;
        for (int i = 0; i < Random.Range(5, 10); i++)
        {
            float randomX = Random.Range(-0.8f, 0.8f);
            Instantiate(coin, new Vector3(transform.position.x + randomX, transform.position.y, 0), Quaternion.identity);
        }
        this.enabled = false;
        yield return new WaitForSeconds(2f);
        player.transform.position = new Vector3(-21, -3.5f, 0);
        SceneManager.LoadScene("WinScreen");
    }
}
