using UnityEngine;

public class Despawner : MonoBehaviour
{
    private float countdown;

    void Start()
    {
        countdown = 2f;
    }

    void Update()
    {
        if (countdown <= 0)
        {
            Destroy(gameObject);
        }

        countdown -= Time.deltaTime;
    }
}
