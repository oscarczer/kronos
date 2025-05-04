using UnityEngine;

public class ClockAnimate : MonoBehaviour
{
    private Transform hoursHand;
    private Transform secondsHand;

    // Rotation speeds
    public float hoursHandSpeed = 20f;
    public float secondsHandSpeed = 240f;

    void Start()
    {
        hoursHand = transform.GetChild(0).GetChild(0);
        secondsHand = transform.GetChild(0).GetChild(1);
    }

    void Update()
    {
        secondsHand.Rotate(Vector3.forward * secondsHandSpeed * Time.deltaTime);
        hoursHand.Rotate(Vector3.forward * hoursHandSpeed * Time.deltaTime);
    }
}
