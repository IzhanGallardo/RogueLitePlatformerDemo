using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPool : MonoBehaviour
{
    public int arrowPoolSize = 10;
    public GameObject arrow;
    private GameObject[] arrows;
    public int arrowShootNumber = -1;

    //Create the Arrow Pool and instantiate every arrow on it
    void Start()
    {
        arrows = new GameObject[arrowPoolSize];
        for(int i=0; i<arrowPoolSize; i++)
        {
            arrows[i] = Instantiate(arrow, new Vector3(-20f, -100f), transform.rotation);
        }

        //In case the pool is created after the player, search for it
        PlayerController.SearchForArrowPool();
    }

    //If all the arrows have been shot, start from the bottom of the pool.
    //Move an arrow from the pool to the designated position and asign its shooting object and lifetime
    public void ShootArrow(Vector3 pos, GameObject player, float lifeTime)
    {
        arrowShootNumber++;

        if(arrowShootNumber > arrowPoolSize - 1)
        {
            arrowShootNumber = 0;
        }

        if (arrows[arrowShootNumber] != null)
        {
            arrows[arrowShootNumber].transform.position = pos;
            arrows[arrowShootNumber].GetComponent<ArrowBehaviour>().player = player;
            arrows[arrowShootNumber].GetComponent<ArrowBehaviour>().lifeTime = lifeTime;
            arrows[arrowShootNumber].SetActive(true);
        }
    }
}
