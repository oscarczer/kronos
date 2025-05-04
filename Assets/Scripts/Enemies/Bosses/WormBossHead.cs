using UnityEngine;

public class WormBossHead : MonoBehaviour
{
    private Transform[] bodies;
    private GameObject player;
    private Rigidbody2D rigidBody;
    public float health = 30f;
    private bool isDead = false;
    private float moveSpeed = 0.001f;
    public GameObject bullet;
    public Sprite wormHead;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            return;
        }

        bodies = GetComponentsInChildren<Transform>();
        CheckChildren();

        if (
            rigidBody.linearVelocity.magnitude < 1
            || transform.position.x > 35
            || transform.position.y > 6
            || transform.position.x < -35
            || transform.position.y < -30
        )
        {
            rigidBody.linearVelocity = new Vector2(0, 0);

            Vector2 playerDirection = player.transform.position - transform.position;
            rigidBody.AddForce(moveSpeed * playerDirection.normalized, ForceMode2D.Impulse);

            Vector3 vectorToTarget = player.transform.position - transform.position;
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            if (transform.childCount == 1)
            {
                moveSpeed = 0.0015f;
            }
        }

        if (health <= 0 & !isDead)
        {
            isDead = true;

            Transform first = transform.GetChild(0);
            first.SetParent(null);

            foreach (Transform body in bodies)
            {
                body.SetParent(first);
            }

            first.name = "Head";
            first.tag = "WormHead";
            first.GetComponent<WormBossHead>().enabled = true;
            first.GetComponent<WormBossHead>().health =
                first.GetComponent<WormBossBody>().health < 15
                    ? 2 * first.GetComponent<WormBossBody>().health
                    : 30;
            first.GetComponent<WormBossBody>().enabled = false;

            first.GetComponent<Rigidbody2D>().linearVelocity = rigidBody.linearVelocity;
            first.GetChild(0).GetComponent<SpriteRenderer>().sprite = wormHead;

            player.GetComponent<PlayerController>().AlterTime(5f);

            Destroy(gameObject);
        }
    }

    void CheckChildren()
    {
        if (transform.childCount == 0)
        {
            Destroy(gameObject);
        }

        WormBossBody first = transform.GetChild(0).gameObject.GetComponent<WormBossBody>();
        first.front = gameObject;
        first.behind = bodies[1].gameObject;

        for (int i = 1; i < bodies.Length - 1; i++)
        {
            WormBossBody body = bodies[i].gameObject.GetComponent<WormBossBody>();
            body.front = bodies[i - 1].gameObject;
            body.behind = bodies[i + 1].gameObject;
        }

        WormBossBody tail = bodies[bodies.Length - 1].gameObject.GetComponent<WormBossBody>();
        tail.front = bodies[bodies.Length - 2].gameObject;
    }
}
