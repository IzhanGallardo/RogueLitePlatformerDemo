using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CaveDoor : MonoBehaviour
{
    public string nextScene;
    public Vector3 playerNextPosition;
    private GameObject player;
    public GameObject keyPress;
    public AudioClip openAudio;

    public float talkDistance = 2;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(player.transform.position, gameObject.transform.position) < talkDistance)
        {
            keyPress.SetActive(true);
            if (Input.GetKeyDown(KeyCode.F) && player.GetComponent<PlayerController>().keys >= 1)
            {
                player.GetComponent<PlayerController>().PlayAudio(openAudio);
                player.GetComponent<PlayerController>().keys--;
                player.transform.position = playerNextPosition;
                SceneManager.LoadScene(nextScene); 
            }
        }
        else
        {
            keyPress.SetActive(false);
        }
    }
}
