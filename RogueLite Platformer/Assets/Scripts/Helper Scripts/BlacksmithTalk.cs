using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlacksmithTalk : MonoBehaviour
{
    private GameObject player;
    public GameObject keyPress;
    public GameObject bsShop;

    public float talkDistance = 2;

    private float soundTimer = 1.15f;
    private float currSoundTimer = 0;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //Check distance towards the player, if they press F on range, open the shop
        if(Vector2.Distance(player.transform.position, gameObject.transform.position) < talkDistance)
        {
            keyPress.SetActive(true);
            if (Input.GetKeyDown(KeyCode.F))
            {
                player.GetComponent<PlayerController>().isOnShop = true;
                bsShop.SetActive(true);
            }
        } else
        {
            keyPress.SetActive(false);
        }

        if (Vector2.Distance(player.transform.position, gameObject.transform.position) < talkDistance*5 && currSoundTimer <= 0)
        {
            GetComponent<AudioSource>().Play(0);
            currSoundTimer = soundTimer;
        }
        else if(currSoundTimer > 0)
        {
            currSoundTimer -= Time.deltaTime;
        }
    }
}
