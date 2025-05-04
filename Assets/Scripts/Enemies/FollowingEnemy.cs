using UnityEngine;

public class FollowingEnemy : EnemyController
{
    // Movement Variables
    public float moveSpeed = 8.0f;
    public bool chase = false;
    private Transform cam;
    private float screenWidth = 25f;
    private float screenHeight = 10f;

    public Transform Cam
    {
        get => cam;
        set => cam = value;
    }
    public global::System.Single ScreenWidth
    {
        get => screenWidth;
        set => screenWidth = value;
    }
    public global::System.Single ScreenHeight
    {
        get => screenHeight;
        set => screenHeight = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        CurrentHealth = maxHealth;
        Anim = GetComponent<Animator>();
        Player = GameObject.FindGameObjectWithTag("Player");
        Cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    // Make sure enemy is facing the player
    public virtual void FacePlayer()
    {
        if (transform.position.x < Player.transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    // When colliding with an enemy, player takes damage
    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerController playerController = Player.GetComponent<PlayerController>();
            if (!playerController.invulnerable)
            {
                Anim.SetTrigger("attack");
                playerController.AlterTime(attackDamage);
                playerController.StartKnockBack(transform.position);

                AttackCooldown = maxAttackCooldown;
            }
        }
    }

    // Only start chasing once player is within a certain range of the enemy
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            chase = true;
        }
    }

    // stop following player when they run to a different screen
    public virtual void StopChasingOffScreen()
    {
        if (OusideCamView())
        {
            chase = false;
        }
    }

    public bool OusideCamView()
    {
        float halfWidth = ScreenWidth / 2;
        float halfHeight = ScreenHeight / 2;

        if (Cam == null)
        {
            Cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        }

        return transform.position.x < Cam.position.x - halfWidth
            || transform.position.x > Cam.position.x + halfWidth
            || transform.position.y < Cam.position.y - halfHeight
            || transform.position.y > Cam.position.y + halfHeight;
    }
}
