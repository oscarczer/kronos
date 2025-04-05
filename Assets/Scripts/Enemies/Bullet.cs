using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Bullet : FollowingEnemy
{
    private float maxAliveTime = 5.0f;
    private float aliveTime;
    public AudioSource explodeSFX;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        Cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        Anim = GetComponent<Animator>();
        aliveTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Player.GetComponent<PlayerController>().IsDead)
        {
            StopChasingOffScreen();

            // Once bullet is alive for a certain period it should be destroyed
            if (aliveTime < maxAliveTime)
            {
                Chase();
                aliveTime += Time.deltaTime;
            }
            else
            {
                Die();
            }
        }
    }

    // Constantly move towards the Player
    private void Chase()
    {
        FacePlayer();
        transform.position = Vector2.MoveTowards(
            transform.position,
            Player.transform.position,
            moveSpeed * Time.deltaTime
        );
    }

    // Destroy the bullet and deal damage if it comes into contact with the player
    public override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerController playerController = Player.GetComponent<PlayerController>();

            if (playerController.invulnerable) // dashing or something
            {
                // don't want to play the explode animation
                Destroy(gameObject);
            }
            else // do damage as per normal
            {
                playerController.AlterTime(attackDamage);
                playerController.StartKnockBack(transform.position);
                Die();
            }
        }
    }

    // Make sure bullet is facing the player
    public override void FacePlayer()
    {
        Vector2 direction = Player.transform.position - transform.position;
        float angle = Vector2.SignedAngle(Vector2.right, direction);
        transform.eulerAngles = new Vector3(0, 0, angle);
    }

    // Destroy the bullet
    public override void Die()
    {
        Anim.SetTrigger("explode");
        explodeSFX.Play();
        Destroy(gameObject, 0.2f);
        GetComponent<CircleCollider2D>().enabled = false;
    }

    // delete bullet if the player runs to a different screen
    public override void StopChasingOffScreen()
    {
        if (OusideCamView())
        {
            Destroy(gameObject);
        }
    }
}
