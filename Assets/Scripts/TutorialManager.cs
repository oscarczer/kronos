using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    private GameObject player;

    //  Start  is  called  before  the  first  frame  update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    //  Update  is  called  once  per  frame
    void Update()
    {
        if (player.transform.position.y < -10f)
        {
            SceneManager.LoadScene("Level1", LoadSceneMode.Single);
        }
    }
}
