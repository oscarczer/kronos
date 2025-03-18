using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject transistor;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
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
