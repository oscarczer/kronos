using UnityEngine;

public class FlyingEnemy : FollowingEnemy
{
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        Cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        Anim = GetComponent<Animator>();
        CurrentHealth = maxHealth;
    }

    void Update()
    {
        if (chase && !Player.GetComponent<PlayerController>().IsDead)
        {
            Chase();
            StopChasingOffScreen();
        }
        if (IsKnocking)
        {
            KnockbackHandler();
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
}
