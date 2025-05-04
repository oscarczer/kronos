using UnityEngine;

public class Collectible : MonoBehaviour
{
    public float timeGained = 20f;
    public Vector2 thrust = Vector2.zero;
    public bool destroyOffScreen = false;
    public GameObject deathPopup;

    void Update()
    {
        transform.Rotate(new Vector3(0, 0, -90 * Time.deltaTime));

        transform.position = new Vector3(
            transform.position.x + (thrust.x * Time.deltaTime * 5),
            transform.position.y + (thrust.y * Time.deltaTime * 5),
            transform.position.z
        );
        if (destroyOffScreen)
        {
            if (Mathf.Abs(transform.position.x) > 25 || Mathf.Abs(transform.position.y) > 10)
            {
                Destroy(gameObject);
            }
        }
    }

    // Player collects this
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            // Give player some bonus time
            collision.gameObject.GetComponent<PlayerController>().AlterTime(timeGained);
            Destroy(gameObject);
        }
    }
}
