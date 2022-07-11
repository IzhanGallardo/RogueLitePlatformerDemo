using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject player;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>().gameObject;
    }

    //Update the position depending on the players
    void Update()
    {
        if(player == null)
        {
            player = FindObjectOfType<PlayerController>().gameObject;
        } else 
        {

            transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 1, -10);
        }
    }
}
