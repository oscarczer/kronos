using System.Collections;
using TMPro;
using UnityEngine;

public class TimerDisplay : MonoBehaviour
{
    public TextMeshProUGUI seconds100;
    public TextMeshProUGUI seconds10;
    public TextMeshProUGUI seconds;
    private float time;
    private GameObject player;

    private float previousTime;

    // Animating is 0 for false, 1 for animating up, -1 for animating down
    private int animating = 0;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        time = player.GetComponent<PlayerController>().RemainingTime;
        previousTime = time;
    }

    void Update()
    {
        // If the players time has changed significantly in the last frame, do some visual stuff with the display
        if (animating == 0)
        {
            previousTime = time;
        }
        time = player.GetComponent<PlayerController>().RemainingTime;

        if (animating == 0 && Mathf.Abs(previousTime - time) >= 5)
        {
            // Big change
            animating = (int)Mathf.Sign(time - previousTime);
            StartCoroutine(TimerAnimator());
        }

        UpdateTimerDisplay();
        UpdateTimerColour();
    }

    // Set each of the TMP characters to be the value at each digit of the timer
    void UpdateTimerDisplay()
    {
        if (animating == 0)
        {
            // Display time as normal
            seconds100.text = Mathf.FloorToInt(time / 100).ToString();
            seconds10.text = Mathf.FloorToInt((time / 10) % 10).ToString();
            seconds.text = Mathf.FloorToInt(time % 10).ToString();
        }
        else
        {
            // Display previousTime, letting us animate
            seconds100.text = Mathf.FloorToInt(previousTime / 100).ToString();
            seconds10.text = Mathf.FloorToInt((previousTime / 10) % 10).ToString();
            seconds.text = Mathf.FloorToInt(previousTime % 10).ToString();
        }
    }

    void UpdateTimerColour()
    {
        Color newColour;
        if (time < 10)
        {
            newColour = new Color(1f, 0f, 0f, 1f);
        }
        else
        {
            newColour = new Color(0f, 0f, 0f, 1f);
        }

        if (animating == 1)
        {
            newColour = new Color(0f, 0.3f, 0f, 1f);
        }
        else if (animating == -1)
        {
            newColour = new Color(0.3f, 0f, 0f, 1f);
        }

        seconds100.color = newColour;
        seconds10.color = newColour;
        seconds.color = newColour;
    }

    private IEnumerator TimerAnimator()
    {
        // Change the timer every few frames
        yield return new WaitForSeconds(0.05f);

        if (animating == 1)
        {
            // Animating up
            previousTime++;
            if (previousTime > time)
            {
                // Animation has finished, return to normal
                animating = 0;
            }
            else
            {
                // Call this again if the animation has not finished
                StartCoroutine(TimerAnimator());
            }
        }
        else if (animating == -1)
        {
            // Animating down
            previousTime--;
            if (previousTime < time)
            {
                // Animation has finished, return to normal
                animating = 0;
            }
            else
            {
                // Call this again if the animation has not finished
                StartCoroutine(TimerAnimator());
            }
        }
    }
}
