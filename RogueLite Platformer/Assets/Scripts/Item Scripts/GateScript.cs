using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateScript : MonoBehaviour
{
    private GameObject lever;
    public int gateNumber;

    // Start is called before the first frame update
    void Start()
    {
        for(int i=0; i< FindObjectsOfType<LeverTrigger>().Length; i++)
        {
            if (FindObjectsOfType<LeverTrigger>()[i].leverNumber == gateNumber)
            {
                lever = FindObjectsOfType<LeverTrigger>()[i].gameObject;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (lever.GetComponent<LeverTrigger>().leverState == 1)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
            this.enabled = false;
        }
    }
}
