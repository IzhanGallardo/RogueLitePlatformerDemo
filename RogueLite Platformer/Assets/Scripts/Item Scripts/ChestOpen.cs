using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestOpen : MonoBehaviour
{
    public GameObject key;
    private GameObject player;
    public GameObject keyPress;
    private bool isOpened = false;

    public float talkDistance = 2;
    public AudioClip openAudio;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>().gameObject;
        GetComponent<Animator>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isOpened)
        {
            if (Vector2.Distance(player.transform.position, gameObject.transform.position) < talkDistance)
            {
                keyPress.SetActive(true);
                if (Input.GetKeyDown(KeyCode.F))
                {
                    player.GetComponent<PlayerController>().PlayAudio(openAudio);
                    isOpened = true;
                    GetComponent<Animator>().enabled = true;
                    Instantiate(key, new Vector3(transform.position.x, transform.position.y + 1, 0), Quaternion.identity);
                }        
            }
            else
            {
                keyPress.SetActive(false);
            }
        } else
        {
            keyPress.SetActive(false);
        }
    }
}
