using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TextAnimate : MonoBehaviour
{
    public TextMeshProUGUI text;
    public int charsVisible = 0;
    private bool animating = false;

    // Start is called before the first frame update
    void Start()
    {
        text.maxVisibleCharacters = charsVisible;
        transform.GetChild(0).gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        text.maxVisibleCharacters = charsVisible;
    }

    // Text only displays when player is on this screen
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            // Shop has different welcome text based on which level it is
            switch (SceneManager.GetActiveScene().name)
            {
                case "Level2":
                    text.text =
                        "Need more upgrades?\nI have just what you need!\nMake sure to spend your time wisely ;)";
                    break;
                default: // Level1
                    text.text =
                        "Welcome to my shop!\nHere I can provide you with upgrades.\nBut be quick, time is money :)";
                    break;
            }

            animating = true;
            StartCoroutine(TextDelayStart());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            animating = false;
            charsVisible = 0;
        }
    }

    private IEnumerator TextDelayStart()
    {
        // Wait for 0.1 seconds.
        yield return new WaitForSeconds(0.5f);
        // Show a new character
        charsVisible++;
        StartCoroutine(TextDelay());
    }

    private IEnumerator TextDelay()
    {
        // Wait for 0.05 seconds.
        yield return new WaitForSeconds(0.05f);
        // Show a new character
        charsVisible++;
        if (animating)
            StartCoroutine(TextDelay());
        else
            charsVisible = 0;
    }
}
