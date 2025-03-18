using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameOverHelper : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Canvas>().worldCamera == null)
        {
            GetComponent<Canvas>().worldCamera = GameObject.FindAnyObjectByType<Camera>();
        }
    }
}
