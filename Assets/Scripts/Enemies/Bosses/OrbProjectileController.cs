using UnityEngine;

public class OrbProjectileController : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerController player;
    public Vector2 thrust;
    private readonly float speed = 5f;
    public GameObject healthPopup;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        rb.linearVelocity = speed * thrust;
        Physics2D.IgnoreLayerCollision(3, 3); // enemy / enemy
        Physics2D.IgnoreLayerCollision(3, 6); // enemy / ground
    }

    void Update()
    {
        rb.linearVelocity = speed * rb.linearVelocity.normalized;

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
    }
}
