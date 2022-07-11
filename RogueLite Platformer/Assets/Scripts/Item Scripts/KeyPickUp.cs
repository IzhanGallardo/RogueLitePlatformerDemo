using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPickUp : MonoBehaviour
{
    public GameObject pickUpEffect;
    public AudioClip pickUpAudio;

    //If collides with the player, add the key and summon the pickup effect
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().PlayAudio(pickUpAudio);
            collision.GetComponent<PlayerController>().AddKey();
            Instantiate(pickUpEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
