using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject transistor;

    //  Update  is  called  once  per  frame
    void Update()
    {
        int count = GameObject.FindGameObjectsWithTag("WormBody").Length;
        count += GameObject.FindGameObjectsWithTag("WormHead").Length;
        if (count == 0)
        {
            transistor.SetActive(true);
        }
    }
}
