using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightController : MonoBehaviour
{
    private PlayerController player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        float time = player.RemainingTime;

        if (time < 10 && time > 0)
        {
            GetComponent<Light2D>().color = new Color(1f, 0f, 0f, 1f);
        }
        else
        {
            GetComponent<Light2D>().color = new Color(1f, 1f, 1f, 1f);
        }
    }
}
