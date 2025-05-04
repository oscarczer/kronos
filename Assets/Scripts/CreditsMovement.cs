using UnityEngine;

public class CreditsMovement : MonoBehaviour
{
    void Update()
    {
        if (transform.position.y > -38.53)
            transform.position -= Vector3.up * Time.deltaTime * 1.5f;
    }
}
