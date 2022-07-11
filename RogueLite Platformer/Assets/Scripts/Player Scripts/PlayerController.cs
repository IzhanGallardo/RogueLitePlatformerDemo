using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController player = null;

    public int health;
    public int maxHealth = 3;
    public int baseDamage = 1;
    public int damage;
    public float baseMoveSpeed = 5;
    public float moveSpeed;
    public float baseAttackSpeed = 1;
    public float attackSpeed;

    static ArrowPool arrowPool;

    private float hitTimer = 0.5f;
    private float currHitTimer = 0;

    public int coins = 0;
    public int keys = 0;

    public bool isOnShop = false;
    public int attSpdBought = 0;
    public int maxASBuy = 4;
    public int moveSpdBought = 0;
    public int maxMSBuy = 4;
    public int attDmgBought = 0;

    public AudioClip footstepsSound;

    private void Awake()
    {
        if(player == null)
        {
            player = this;
        } else if(player != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        //Initializate variables
        health = maxHealth;
        moveSpeed = baseMoveSpeed;
        attackSpeed = baseAttackSpeed;
        damage = baseDamage;

        arrowPool = FindObjectOfType<ArrowPool>();
    }

    void Update()
    {
        //Update stats taking into account the upgrades bought in the shop
        attackSpeed = baseAttackSpeed * Mathf.Pow(1.1f, attSpdBought);
        moveSpeed = baseMoveSpeed * Mathf.Pow(1.1f, moveSpdBought);
        damage = baseDamage + attDmgBought;

        if (currHitTimer > 0)
        {
            currHitTimer -= Time.deltaTime;
            gameObject.GetComponent<SpriteRenderer>().enabled = Mathf.PingPong(Time.time, 0.1f) > (0.1f / 2f);
        } else if (currHitTimer <= 0)
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = true;
            GetComponent<PlayerMovement>().enabled = true;
            currHitTimer = 0;
        }
    }

    public void GetHit(int damageRecived, Transform attacker)
    {
        if(currHitTimer == 0 && GetComponent<PlayerMovement>().isVulnerable)
        {
            currHitTimer = hitTimer;
            //Add knockback to the player as they get hit 
            GetComponent<PlayerMovement>().enabled = false;
            Vector2 dir = new Vector2(((transform.position - attacker.transform.position).normalized).x * 0.5f, 0.3f);
            GetComponent<Rigidbody2D>().AddForce(dir * 5, ForceMode2D.Impulse);

            health -= damageRecived;
        }  
        
        if (health <= 0) {
            gameObject.GetComponent<PlayerMovement>().Die();
        }
    }

    public static void SearchForArrowPool()
    {
        arrowPool = FindObjectOfType<ArrowPool>();
    }

    public void ShootArrow(int facingDir)
    {
        arrowPool.ShootArrow(transform.position + new Vector3(1.2f * facingDir, 0, 0), gameObject, 2);
    }

    public void AddCoin()
    {
        coins++;
    }

    public void AddKey()
    {
        keys++;
    }

    public void Rest()
    {
        transform.position = new Vector3(-21, -3f, 0);
        health = maxHealth;
    }

    public void PlayAudio(AudioClip clip)
    {
        GetComponent<AudioSource>().clip = clip;
        GetComponent<AudioSource>().Play();
    }

    public void AudioLoop(bool loop)
    {
        GetComponent<AudioSource>().loop = loop;
    }

}
