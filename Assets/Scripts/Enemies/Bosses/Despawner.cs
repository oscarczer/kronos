using UnityEngine;

public class Despawner : MonoBehaviour
{
    private float countdown;

    // Start is called before the first frame update
    void Start()
    {
        countdown = 2f;
    }

    // Update is called once per frame
    void Update()
    {
        if (countdown <= 0)
        {
            Destroy(gameObject);
        }

        countdown -= Time.deltaTime;
    }
}
