using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MSBuy : MonoBehaviour
{
    public GameObject player;
    private void Start()
    {
        player = FindObjectOfType<PlayerController>().gameObject;
    }
    // Update is called once per frame
    void Update()
    {
        if (player.GetComponent<PlayerController>().moveSpdBought < player.GetComponent<PlayerController>().maxMSBuy)
        {
            GetComponent<Text>().text = "Buy for " + (5 + 5 * player.GetComponent<PlayerController>().moveSpdBought) + "G";
        }
        else
        {
            GetComponent<Text>().text = "Max Bought";
        }
    }
}
