using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    #region  Variables
    //  General  Operation  Variables
    private GameObject player;
    private bool isDead = false;
    private Animator anim;
    public GameObject healthPopup;

    public GameObject Player
    {
        get => player;
        set => player = value;
    }
    public Animator Anim
    {
        get => anim;
        set => anim = value;
    }
    public global::System.Boolean IsDead
    {
        get => isDead;
        set => isDead = value;
    }

    //  Health  Variables
    public float timeGainedFromKill;
    public float maxHealth;
    private float currentHealth;

    public global::System.Single CurrentHealth
    {
        get => currentHealth;
        set => currentHealth = value;
    }

    //  Attack  Variables
    public float attackDamage;
    private float attackCooldown;
    public float maxAttackCooldown = 0.5f;

    public global::System.Single AttackCooldown
    {
        get => attackCooldown;
        set => attackCooldown = value;
    }

    //  Knockback  Variables
    public bool knockback = true;
    private Vector3 knockDirection;
    private bool isKnocking = false;
    public float knockInitial;
    private float knockCurrent = 0.0f;

    //  KnockUp  Variables
    public bool knockUp = true;
    public bool isKnockingUp = false;
    private float knockUpInitial = 10;
    private float knockUpCurrent = 0.0f;
    private float initialY;

    public Vector3 KnockDirection
    {
        get => knockDirection;
        set => knockDirection = value;
    }
    public global::System.Boolean IsKnocking
    {
        get => isKnocking;
        set => isKnocking = value;
    }
    public global::System.Single KnockCurrent
    {
        get => knockCurrent;
        set => knockCurrent = value;
    }
    #endregion

    //  Start  is  called  before  the  first  frame  update
    void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
    }

    //  Take  damage  equal  to  the  players  damage  (used  for  when  hit  in  PlayerCombat)
    public void TakeDamage(float damage)
    {
        CurrentHealth += damage;
        Anim.SetTrigger("hit");

        if (CurrentHealth < 0)
        {
            Die();
        }

        if (knockback)
        {
            IsKnocking = true;
            KnockDirection = (transform.position - Player.transform.position).normalized;
            KnockCurrent = knockInitial;
        }
        if (knockUp)
        {
            if (!isKnockingUp)
            {
                initialY = transform.position.y;
                isKnockingUp = true;
            }
            knockUpCurrent = knockUpInitial;
        }
    }

    //  Destroy  the  enemy  (and  later  player  a  death  animation)
    public virtual void Die()
    {
        if (!IsDead)
        {
            IsDead = true;
            Player.GetComponent<PlayerController>().AlterTime(timeGainedFromKill);
            //  turn  off  Hitbox,  so  player  cannot  hit  enemy  and  enemy  cannot  hit  player
            transform.GetChild(0).gameObject.SetActive(false);

            Anim.SetTrigger("dead");
            Destroy(gameObject, 0.8f);
        }
    }

    //  Knock  back  enemy  when  hit
    public void KnockbackHandler()
    {
        transform.position += KnockDirection * KnockCurrent * Time.deltaTime;
        KnockCurrent -= Time.deltaTime * 25;
        if (KnockCurrent < 0)
        {
            IsKnocking = false;
        }
    }

    public void KnockUpHandler()
    {
        transform.position += Vector3.up * knockUpCurrent * Time.deltaTime;
        knockUpCurrent -= Time.deltaTime * 50;
        if (transform.position.y <= initialY)
        {
            transform.position = new Vector3(transform.position.x, initialY, transform.position.z);
            isKnockingUp = false;
        }
    }
}
