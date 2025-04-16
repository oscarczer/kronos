using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightController : MonoBehaviour
{
    private PlayerController player;
    private Light2D lightComponent;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        lightComponent = GetComponent<Light2D>();
    }

    void Update()
    {
        float time = player.RemainingTime;

        Color newColour;
        float newIntensity;

        if (time < 10 && time > 0)
        {
            newColour = new Color(1f, 0f, 0f, 1f);
            newIntensity = 0.3f;
        }
        else
        {
            newColour = new Color(1f, 1f, 1f, 1f);
            newIntensity = 0.1f;
        }
        lightComponent.color = newColour;
        lightComponent.intensity = newIntensity;
    }
}
