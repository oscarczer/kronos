using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnemy : EnemyController
{
    public bool shoot = false;
    public GameObject bullet;
    private int shotsFired = 0;
    public AudioSource shootSFX;

    // Start is called before the first frame update
    void Start()
    {
        CurrentHealth = maxHealth;
        Player = GameObject.FindGameObjectWithTag("Player");
        AttackCooldown = maxAttackCooldown;
        Anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Player.GetComponent<PlayerController>().IsDead && !isKnockingUp)
        {
            if (shoot)
            {
                Shoot();
            }
            AttackCooldown -= Time.deltaTime;
        }
        if (isKnockingUp)
        {
            KnockUpHandler();
        }
    }

    // Once the attack timer is over, shoot a bullet and start cooldown again
    private void Shoot()
    {
        if (IsDead)
        {
            Anim.SetBool("shoot", false);
            return;
        }

        if (AttackCooldown <= 0)
        {
            // shotsFired is used so that the guy shoots 3 times, takes a break, then shoots again
            shotsFired++;
            if (shotsFired <= 3)
            {
                Instantiate(bullet, transform.position, bullet.transform.rotation);
                shootSFX.Play();
                if (shotsFired == 3)
                    Anim.SetBool("shoot", false);
            }
            if (shotsFired >= 5) // change this number to alter length of break
            {
                shotsFired = 0;
                Anim.SetBool("shoot", true);
            }

            // reset the 1 sec timer
            AttackCooldown = maxAttackCooldown;
        }
    }

    // Only shoot when the player is in vision
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            shoot = true;
            Anim.SetBool("shoot", true);
        }
    }

    // Stop shooting when the player is no longer in vision
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            shoot = false;
            Anim.SetBool("shoot", false);
        }
    }

    // When colliding with an enemy, player takes damage
    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Player.GetComponent<PlayerController>().AlterTime(attackDamage);
            Player.GetComponent<PlayerController>().StartKnockBack(transform.position);
        }
    }
}
