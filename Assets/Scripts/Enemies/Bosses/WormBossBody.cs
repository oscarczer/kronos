using UnityEngine;

public class WormBossBody : MonoBehaviour
{
    public GameObject front;
    public GameObject behind;
    public float health = 20f;
    private bool isDead = false;
    public Sprite wormHead;

    // TODO: Put some of this logic into separate functions
    void Update()
    {
        if (isDead)
        {
            return;
        }

        Vector2 headDirection = transform
            .parent.GetComponent<Rigidbody2D>()
            .linearVelocity.normalized;
        transform.position = Vector2.MoveTowards(
            transform.position,
            new Vector2(
                front.transform.position.x - 1.2f * headDirection.x,
                front.transform.position.y - 1.2f * headDirection.y
            ),
            0.5f
        );

        Vector3 vectorToTarget = front.transform.position - transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 20f);

        if (health <= 0)
        {
            isDead = true;

            Transform head = transform.parent;
            Transform[] bodies = head.GetComponentsInChildren<Transform>();

            int destroyedIndex = System.Array.IndexOf(bodies, gameObject.transform);

            if (destroyedIndex == bodies.Length - 1)
            {
                Destroy(gameObject);
                return;
            }

            Transform newHead = bodies[destroyedIndex + 1];
            newHead.SetParent(null);

            for (int i = destroyedIndex; i < bodies.Length; i++)
            {
                bodies[i].SetParent(newHead);
            }

            newHead.name = "Head";
            newHead.tag = "WormHead";
            newHead.GetComponent<WormBossHead>().enabled = true;
            newHead.GetComponent<WormBossHead>().health =
                newHead.GetComponent<WormBossBody>().health < 15
                    ? 2 * newHead.GetComponent<WormBossBody>().health
                    : 30;
            newHead.GetComponent<WormBossBody>().enabled = false;

            newHead.GetComponent<Rigidbody2D>().linearVelocity =
                head.GetComponent<Rigidbody2D>().linearVelocity;
            newHead.GetComponent<SpriteRenderer>().sprite = wormHead;

            GameObject.Find("Player").GetComponent<PlayerController>().AlterTime(3f);

            Destroy(gameObject);
        }
    }
}
