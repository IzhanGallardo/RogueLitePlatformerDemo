using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUIText : MonoBehaviour
{
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Text>().text = player.GetComponent<PlayerController>().health + "/" + player.GetComponent<PlayerController>().maxHealth + " HP";
    }
}
