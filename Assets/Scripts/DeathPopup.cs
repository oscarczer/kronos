using TMPro;
using UnityEngine;

public class DeathPopup : MonoBehaviour
{
    void Start()
    {
        // Set color based on whether the player gains or loses time
        TextMeshProUGUI textObj = this.GetComponentInChildren<TextMeshProUGUI>();
        if (textObj.text.StartsWith('+'))
        {
            textObj.color = Color.green;
        }
        else
        {
            textObj.color = Color.red;
        }

        // move it up a little
        transform.Translate(Vector3.up);

        // text is displayed for 1 sec
        Destroy(gameObject, 1f);
    }

    void Update()
    {
        // move up every frame
        transform.Translate(Vector3.up * Time.deltaTime);
    }
}
