using System.Collections;
using UnityEngine;

public class SlimeEnemy : EnemyController
{
    public GameObject[] landingLocations;
    private bool inTransition = false;
    private bool canJump = true;
    private int positionIndex;
    private Vector3 jumpDirection;
    private float jumpSpeed;
    private readonly float maxJumpSpeed = 1.6f;
    private Vector3 targetPos;

    void Start()
    {
        positionIndex = 0;
        CurrentHealth = maxHealth;
        Player = GameObject.FindGameObjectWithTag("Player");
        AttackCooldown = maxAttackCooldown;
        Anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (canJump)
        {
            positionIndex++;
            positionIndex = positionIndex % landingLocations.Length;

            targetPos = landingLocations[positionIndex].transform.position;
            jumpDirection = targetPos - transform.position;
            jumpSpeed = maxJumpSpeed;
            canJump = false;
            inTransition = true;
            Anim.SetTrigger("jump");
        }
        if (inTransition)
        {
            if (inTransition)
            {
                Vector3 targetPos = landingLocations[positionIndex].transform.position;
                Vector3 futurePos = transform.position + jumpDirection * jumpSpeed * Time.deltaTime;
                if (jumpDirection.x > 0 && (targetPos.x <= futurePos.x))
                {
                    inTransition = false;
                    transform.position = targetPos;
                    StartCoroutine(JumpDelay());
                }
                else if (jumpDirection.x < 0 && (targetPos.x >= futurePos.x))
                {
                    inTransition = false;
                    transform.position = targetPos;
                    StartCoroutine(JumpDelay());
                }
                else
                {
                    transform.position = futurePos;
                }
                jumpSpeed = Mathf.Max(0.2f, 996 * jumpSpeed / 1000);
            }
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
                playerController.AlterTime(attackDamage);
                playerController.StartKnockBack(transform.position);

                //AttackCooldown = maxAttackCooldown;
            }
        }
    }

    private IEnumerator JumpDelay()
    {
        // Wait for 0.05 seconds.
        yield return new WaitForSeconds(1.2f);
        canJump = true;
    }
}
