using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ASBuy : MonoBehaviour
{
    public GameObject player;
    private void Start()
    {
        player = FindObjectOfType<PlayerController>().gameObject;
    }
    // Update is called once per frame
    void Update()
    {
        if (player.GetComponent<PlayerController>().attSpdBought < player.GetComponent<PlayerController>().maxASBuy)
        {
            GetComponent<Text>().text = "Buy for " + (5 + 5 * player.GetComponent<PlayerController>().attSpdBought) + "G";
        } else
        {
            GetComponent<Text>().text = "Max Bought";
        }
    }
}
