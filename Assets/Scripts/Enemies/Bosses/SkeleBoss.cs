using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkeleBoss : MonoBehaviour
{
    //  General
    private Rigidbody2D rigidBody;
    public float jumpHeight;
    private GameObject player;
    private bool isGrounded;
    private bool facingRight = false;
    public float health = 50f;
    private Animator anim;
    private bool isAttacking = false;
    public GameObject bossDrops;
    private bool isDead = false;
    public GameObject door;
    public bool cutscene = true;
    public GameObject bossTitleCard;

    //  Audio
    public AudioSource bossWin;
    public AudioSource wind;
    public AudioSource music;

    //  Jumping  Attack
    public float jumpCooldown = 0.4f;
    private float remainingJumpCooldown;
    private int jumpsRemaining = 3;
    public GameObject timerDrop;

    //  Attack  1
    private Transform attackOnePoint1;
    private Transform attackOnePoint2;
    public float attackCooldown = 1f;
    private float remainingAttackCooldown;
    private int attack1Remaining = 2;

    //  Attack  2
    private int attack2Remaining = 1;
    private Transform attackTwoPoint1;
    private Transform attackTwoPoint2;

    //  Start  is  called  before  the  first  frame  update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");

        remainingAttackCooldown = attackCooldown;

        attackOnePoint1 = transform.GetChild(0).transform;
        attackOnePoint2 = transform.GetChild(1).transform;
        attackTwoPoint1 = transform.GetChild(2).transform;
        attackTwoPoint2 = transform.GetChild(3).transform;

        StartCoroutine(CutScene());
    }

    //  Update  is  called  once  per  frame
    void Update()
    {
        if (player.GetComponent<PlayerController>().IsDead || isDead || cutscene)
        {
            return;
        }

        if (health <= 0)
        {
            Die();
            isDead = true;
        }

        isGrounded = rigidBody.linearVelocity.y == 0;

        if (!isGrounded)
        {
            return;
        }

        anim.SetBool("walking", false);

        if (health > 25)
        {
            if (!isAttacking)
            {
                if (jumpsRemaining > 0)
                {
                    if (remainingJumpCooldown <= 0)
                    {
                        JumpAttack();
                        jumpsRemaining -= 1;
                        remainingJumpCooldown = jumpCooldown;
                    }
                    else
                    {
                        remainingJumpCooldown -= Time.deltaTime;
                    }
                }
                else
                {
                    isAttacking = true;
                    attack1Remaining = 2;
                    attack2Remaining = 1;
                }
            }
            else
            {
                if (attack1Remaining > 0)
                {
                    if (remainingAttackCooldown <= 0)
                    {
                        FacePlayer();
                        Attack1Start();
                        remainingAttackCooldown = attackCooldown;
                        attack1Remaining -= 1;
                    }
                    else
                    {
                        remainingAttackCooldown -= Time.deltaTime;
                    }
                }
                else if (attack2Remaining > 0)
                {
                    if (remainingAttackCooldown <= 0)
                    {
                        Attack2Start();
                        isAttacking = false;
                        jumpsRemaining = 3;
                    }
                    else
                    {
                        remainingAttackCooldown -= Time.deltaTime;
                    }
                }
            }
        }
        else
        {
            isAttacking = true;
            GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 1);

            attackCooldown = 0.5f;

            if (remainingAttackCooldown <= 0)
            {
                Attack1Start();
                remainingAttackCooldown = attackCooldown;
                attack1Remaining -= 1;
            }
            else
            {
                remainingAttackCooldown -= Time.deltaTime;
            }

            if (transform.position.x >= 22)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                facingRight = true;
            }
            else if (transform.position.x <= -1)
            {
                transform.rotation = Quaternion.Euler(0, -180, 0);
                facingRight = false;
            }
        }

        if (!isAttacking)
        {
            FacePlayer();
        }
    }

    void JumpAttack()
    {
        float distanceFromPlayer = player.transform.position.x - transform.position.x;
        anim.SetBool("walking", true);

        int randomNum = Random.Range(0, 2);
        if (randomNum == 0)
        {
            Instantiate(
                timerDrop,
                new Vector2(transform.position.x, -7.5f),
                timerDrop.transform.rotation
            );
        }

        rigidBody.AddForce(new Vector2(distanceFromPlayer / 4, jumpHeight), ForceMode2D.Impulse);
    }

    void Attack1Start()
    {
        anim.SetTrigger("attack1");
    }

    public void Attack1()
    {
        Collider2D[] hit = Physics2D.OverlapAreaAll(
            attackOnePoint1.position,
            attackOnePoint2.position
        );

        foreach (Collider2D item in hit)
        {
            if (item.tag == "Player")
            {
                player.GetComponent<PlayerController>().AlterTime(-10);
            }
        }

        if (facingRight)
        {
            transform.position = new Vector2(transform.position.x - 2f, transform.position.y);
        }
        else
        {
            transform.position = new Vector2(transform.position.x + 2f, transform.position.y);
        }
    }

    void Attack2Start()
    {
        anim.SetTrigger("attack2");
    }

    public void Attack2()
    {
        Collider2D[] hit = Physics2D.OverlapAreaAll(
            attackTwoPoint1.position,
            attackTwoPoint2.position
        );

        foreach (Collider2D item in hit)
        {
            if (item.tag == "Player")
            {
                player.GetComponent<PlayerController>().AlterTime(-20);
            }
        }

        if (facingRight)
        {
            transform.position = new Vector2(transform.position.x - 3f, transform.position.y);
        }
        else
        {
            transform.position = new Vector2(transform.position.x + 3f, transform.position.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Platform" || collision.gameObject.tag == "Player")
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }

    private void FacePlayer()
    {
        if (transform.position.x < player.transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            facingRight = false;
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            facingRight = true;
        }
    }

    public void TakeDamage(float damage)
    {
        float distanceFromPlayer = player.transform.position.x - transform.position.x;

        if (distanceFromPlayer < 0 && facingRight || distanceFromPlayer > 0 && !facingRight)
        {
            anim.SetTrigger("defend");
        }
        else
        {
            anim.SetTrigger("isHit");
            health += damage;
        }
    }

    private void Die()
    {
        music.Stop();
        bossWin.Play();

        if (!bossDrops.activeInHierarchy)
        {
            bossDrops.SetActive(true);
        }

        player.GetComponent<PlayerController>().TimePaused = true;

        wind.Play();
        player.GetComponent<PlayerController>().AlterTime(100);
        anim.SetTrigger("isDead");
        Destroy(gameObject, 0.8f);

        //  text  that  says  "Humphrey  defeated"
        bossTitleCard.SetActive(true);
        bossTitleCard.transform.GetChild(0).localScale = new Vector3(29, 3, 1);
        bossTitleCard.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
            "Humphrey defeated";
        Destroy(bossTitleCard.gameObject, 3f);
    }

    private IEnumerator CutScene()
    {
        player.GetComponent<PlayerController>().immobile = true;

        Collider2D collider = GetComponent<Collider2D>();
        rigidBody.bodyType = RigidbodyType2D.Kinematic;
        collider.enabled = false;

        anim.SetBool("walking", true);

        while (transform.position != new Vector3(21.5f, -5.15f, 0))
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                new Vector3(21.5f, -5.15f, 0),
                1.3f * Time.deltaTime
            );
            yield return null;
        }

        anim.SetBool("walking", false);

        while (door.transform.position != new Vector3(9.74f, 0, 0))
        {
            door.transform.position = Vector3.MoveTowards(
                door.transform.position,
                new Vector3(9.74f, 0, 0),
                1.75f * Time.deltaTime
            );
            yield return null;
        }

        Attack1Start();
        yield return new WaitForSeconds(1f);

        bossTitleCard.SetActive(true);

        yield return new WaitForSeconds(2f);

        bossTitleCard.SetActive(false);
        yield return new WaitForSeconds(0.3f);

        collider.enabled = true;
        rigidBody.bodyType = RigidbodyType2D.Dynamic;

        cutscene = false;
        player.GetComponent<PlayerController>().immobile = false;

        yield return null;
    }
}
