using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBehaviour : MonoBehaviour
{
    public GameObject hitEffect;
    public GameObject player;

    public float lifeTime = 2f;
    private float lifeTimer;

    public float prjSpeed = 10f;

    private int facing = 0;

    public LayerMask enemyLayer;
    public LayerMask groundLayer;
    public Transform wallCheckCircle;
    public float wallCheckerRadius;
    private bool inWall = false;

    void Start()
    {
        //Instantiate the life time of the arrow
        lifeTimer = lifeTime;
        facing = player.GetComponent<PlayerMovement>().facingDir;
        hitEffect.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(facing, 1, 1);
        //Check if it's hitting a wall
        CheckWalls();
        //If the timer is not 0, reduce it and move the arrow forwards
        if (lifeTimer > 0f && !inWall)
        {
            lifeTimer -= Time.deltaTime;
            transform.position += new Vector3(prjSpeed * Time.deltaTime * facing, 0, 0);

        } else if(lifeTimer > 0f && inWall)
        {
            lifeTimer -= Time.deltaTime;
        }

        if (lifeTimer <= 0f)
        {
            gameObject.SetActive(false);
            gameObject.transform.position = new Vector3(-20, -100, 0);
        }


        if (player == null)
        {
            gameObject.transform.position = new Vector3(-20, -100, 0);
            gameObject.SetActive(false);
        }
    }

    //If the arrow hits and enemy, instantiate the hit effect and deactivate for later use on the pool
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(player != null)
        {
            if(collision.gameObject.layer == enemyLayer)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);

                gameObject.SetActive(false);
                gameObject.transform.position = new Vector3(-20, -100, 0);
            } 
        }
    }

    //If the arrow becomes invisible, deactivate it
    private void OnBecameInvisible()
    {
        gameObject.SetActive(false);
        gameObject.transform.position = new Vector3(-20, -100, 0);
    }

    //Set the lifeTime and the facing direction every time the arrow is "summoned" from the pool
    private void OnEnable()
    {
        lifeTimer = lifeTime;
        facing = player.GetComponent<PlayerMovement>().facingDir;
    }

    //Get the damage value for the arrow
    public int getDamage()
    {
        return player.GetComponent<PlayerController>().damage;
    }
    
    //Check if the arrow hits a wall
    public void CheckWalls()
    {
        Collider2D collider = Physics2D.OverlapCircle(wallCheckCircle.position, wallCheckerRadius, groundLayer);

        if(collider == null)
        {
            inWall = false;
        } else
        {
            inWall = true;
        }
    }
}
