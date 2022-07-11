using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBreak : MonoBehaviour
{
    public GameObject coin;
    private GameObject player;
    public AudioClip breakAudio;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>().gameObject;
        GetComponent<Animator>().enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerAttack"))
        {
            player.GetComponent<PlayerController>().PlayAudio(breakAudio);
            GetComponent<Animator>().enabled = true;
            GetComponent<BoxCollider2D>().enabled = false;

            for(int i=0; i<Random.Range(1,4); i++)
            {
                float randomX = Random.Range(-0.8f, 0.8f);
                Instantiate(coin, new Vector3(transform.position.x + randomX, transform.position.y, 0), Quaternion.identity);
            }
        }
    }
}
