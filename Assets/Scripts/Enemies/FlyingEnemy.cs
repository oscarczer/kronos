using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FlyingEnemy : FollowingEnemy
{
    // Start is called before the first frame update
    void Start()
    {
        Player = (GameObject.FindGameObjectWithTag("Player"));
        Cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        Anim = GetComponent<Animator>();
        CurrentHealth = maxHealth;
    }

    // Update is called once per frame
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
        transform.position = Vector2.MoveTowards(transform.position, Player.transform.position, moveSpeed * Time.deltaTime);
    }
}
