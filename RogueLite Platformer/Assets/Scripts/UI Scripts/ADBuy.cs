using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ADBuy : MonoBehaviour
{
    public GameObject player;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Text>().text = "Buy for " + (10 + 10 * player.GetComponent<PlayerController>().attDmgBought) + "G";        
    }
}
