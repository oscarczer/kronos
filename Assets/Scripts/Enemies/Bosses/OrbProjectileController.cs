using UnityEngine;

public class OrbProjectileController : MonoBehaviour
{
    private Rigidbody2D rb;
    int remainingCollisions = 3;
    private PlayerController player;
    public Vector2 thrust;
    private float speed = 5f;
    public GameObject healthPopup;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        rb.linearVelocity = speed * thrust;
        Physics2D.IgnoreLayerCollision(3, 3); // enemy / enemy
        Physics2D.IgnoreLayerCollision(3, 6); // enemy / ground
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = speed * rb.linearVelocity.normalized;

        if (remainingCollisions == 0)
        {
            Destroy(gameObject);
        }

        if (Mathf.Abs(transform.position.x) > 25 || Mathf.Abs(transform.position.y) > 10)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (!player.invulnerable)
            {
                player.AlterTime(-5);
            }
            Destroy(gameObject);
        }
        else
        {
            remainingCollisions -= 1;
        }
    }
}
