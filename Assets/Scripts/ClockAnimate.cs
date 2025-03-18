using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockAnimate : MonoBehaviour
{ 
    // we need to make the hands rotate
    private Transform hoursHand;
    private Transform secondsHand;

    // rotation speeds
    public float hoursHandSpeed = 20f;
    public float secondsHandSpeed = 240f;

    // Start is called before the first frame update
    void Start()
    {
        hoursHand = transform.GetChild(0).GetChild(0);
        secondsHand = transform.GetChild(0).GetChild(1);

    }

    // Update is called once per frame
    void Update()
    {
        secondsHand.Rotate(Vector3.forward * secondsHandSpeed * Time.deltaTime);
        hoursHand.Rotate(Vector3.forward * hoursHandSpeed * Time.deltaTime);
    }
}
