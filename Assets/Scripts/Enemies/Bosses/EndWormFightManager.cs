using UnityEngine;

public class EndWormFightManager : MonoBehaviour
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
