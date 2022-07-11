using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickUp : MonoBehaviour
{
    public GameObject pickUpEffect;
    public AudioClip pickUpAudio;

    //If collides with the player, add the coin and summon the pickup effect
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().PlayAudio(pickUpAudio);
            collision.GetComponent<PlayerController>().AddCoin();
            Instantiate(pickUpEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
