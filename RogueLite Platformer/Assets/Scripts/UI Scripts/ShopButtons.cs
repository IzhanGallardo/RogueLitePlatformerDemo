using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopButtons : MonoBehaviour
{
    public GameObject player;
    public GameObject shop;
    private void Start()
    {
        player = FindObjectOfType<PlayerController>().gameObject;
    }

    public void BuyAS()
    {
        if(player.GetComponent<PlayerController>().attSpdBought < player.GetComponent<PlayerController>().maxASBuy &&
            player.GetComponent<PlayerController>().coins >= (5 + 5 * player.GetComponent<PlayerController>().attSpdBought))
        {
            player.GetComponent<PlayerController>().coins -= (5 + 5 * player.GetComponent<PlayerController>().attSpdBought);
            player.GetComponent<PlayerController>().attSpdBought++;
        }
    }

    public void BuyMS()
    {
        if(player.GetComponent<PlayerController>().moveSpdBought < player.GetComponent<PlayerController>().maxMSBuy &&
            player.GetComponent<PlayerController>().coins >= (5 + 5 * player.GetComponent<PlayerController>().moveSpdBought))
        {
            player.GetComponent<PlayerController>().coins -= (5 + 5 * player.GetComponent<PlayerController>().moveSpdBought);
            player.GetComponent<PlayerController>().moveSpdBought++;
        }
    }

    public void BuyAD()
    {
        if(player.GetComponent<PlayerController>().coins >= (10 + 10 * player.GetComponent<PlayerController>().attDmgBought))
        {
            player.GetComponent<PlayerController>().coins -= (10 + 10 * player.GetComponent<PlayerController>().attDmgBought);
            player.GetComponent<PlayerController>().attDmgBought++;
        }
    }

    public void ExitShop()
    {
        player.GetComponent<PlayerController>().isOnShop = false;
        shop.SetActive(false);
    }
}
