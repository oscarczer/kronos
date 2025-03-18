using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormBullet : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        rb.AddForce(0.00175f * Vector2.right, ForceMode2D.Impulse);
        Physics2D.IgnoreLayerCollision(3, 6); // enemy / ground
    }

    // Update is called once per frame
    void Update()
    {
        // if (Mathf.Abs(transform.position.x) > 27 || Mathf.Abs(transform.position.y) > 13)
        // {
        //     Destroy(gameObject);
        // }
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
