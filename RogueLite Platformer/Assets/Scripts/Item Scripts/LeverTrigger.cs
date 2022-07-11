using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverTrigger : MonoBehaviour
{
    public int leverState = 0;
    public int leverNumber;

    private GameObject player;
    public AudioClip openAudio;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>().gameObject;
        GetComponent<Animator>().enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("PlayerAttack") || collision.CompareTag("Player"))
        {
            player.GetComponent<PlayerController>().PlayAudio(openAudio);
            GetComponent<Animator>().enabled = true;
            leverState = 1;
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
