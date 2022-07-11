using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatEnemy : EnemyParent
{
    private int yMovement = 0;
    private bool playerInXSight = false;
    private bool playerInYSight = false;

    public Transform roofCheckerCircle;

    public override void Update()
    {
        //Check distance towards the player
        distancePlayer = new Vector3(transform.position.x - player.transform.position.x, transform.position.y - player.transform.position.y, transform.position.y - player.transform.position.y);
        //Change the facing direction of the sprite
        gameObject.transform.localScale = new Vector3(facingDir, 1, 1);
        //Check the enemy's movement and change the animation
        if ((xMovement != 0 || yMovement != 0) && !anim.GetBool("isWalking") && !anim.GetBool("isAttacking"))
        {
            anim.SetBool("isWalking", true);
        }
        else if (!playerInSight && anim.GetBool("isWalking") && !SearchRoof())
        {
            SearchRoof();
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
    //Override the whole Idle method
    public override void Idle()
    {
        
    }

    public override void Move(float moveSpeed, float moveDirection)
    {
        //Add the movement to the enemy's velocity if it's not attacking
        if (!anim.GetBool("isAttacking"))
        {
            rb.velocity = new Vector2(moveDirection * moveSpeed, yMovement * moveSpeed);
        }

        //Change the facing direction if not attacking depending on the last movement done
        if (!anim.GetBool("isAttacking") && ((moveDirection == 1 && facingDir == -1) || (moveDirection == -1 && facingDir == 1)))
        {
            facingDir *= -1;
        }
    }

    public override void CheckPlayer()
    {
        //If the player is in sight of the enemy, move towards them, if they're not, stand still
        if(Vector2.Distance(player.transform.position, gameObject.transform.position) < sight)
        {
            if (Mathf.Abs(distancePlayer.x) < sight && Mathf.Abs(distancePlayer.x) > 0.5f && !anim.GetBool("isDead") && !CheckWalls())
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
            else if ((Mathf.Abs(distancePlayer.x) < 0.5 && !CheckWalls()) || anim.GetBool("isDead"))
            {
                playerInYSight = true;
                xMovement = 0;
            }
            else
            {
                xMovement = 0;
                playerInSight = false;
            }

            if (Mathf.Abs(distancePlayer.y) < sight && Mathf.Abs(distancePlayer.y) > 0.5f && !anim.GetBool("isDead") && !CheckWalls())
            {
                playerInYSight = true;
                if (transform.position.y > player.transform.position.y)
                {
                    yMovement = -1;
                }
                else
                {
                    yMovement = 1;
                }
            }
            else if ((Mathf.Abs(distancePlayer.y) < 0.5 && !CheckWalls()) || anim.GetBool("isDead"))
            {
                playerInYSight = true;
                yMovement = 0;
            }
            else
            {
                yMovement = 0;
                playerInYSight = false;
            }

            //Update the player checker depending on both positions
            playerInSight = playerInXSight || playerInYSight;
        } else
        {
            playerInSight = false;
        }
        

    }

    //If the bat is searching for a roof move upwards until finding one, then stop moving and hang from it
    private bool SearchRoof()
    {
        Collider2D collider = Physics2D.OverlapCircle(roofCheckerCircle.position, groundCheckerRadius, groundLayer);

        if(collider == null)
        {
            xMovement = 0;
            yMovement = 1;
            return false;
        } else
        {
            xMovement = 0;
            yMovement = 0;
            anim.SetBool("isWalking", false);
            return true;
        }
    }

}
