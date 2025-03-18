using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DeathPopup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Set color based on whether the player gains or loses time
        TextMeshProUGUI textObj = this.GetComponentInChildren<TextMeshProUGUI>();
        if (textObj.text.StartsWith('+'))
        {
            textObj.color = Color.green;
        } else
        {
            textObj.color = Color.red;
        }

        // move it up a little
        transform.Translate(Vector3.up);

        // text is displayed for 1 sec
        Destroy(gameObject, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        // move up every frame
        transform.Translate(Vector3.up * Time.deltaTime);
       
    }
}
