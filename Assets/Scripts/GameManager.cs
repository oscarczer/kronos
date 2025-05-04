using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject transistor;

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
