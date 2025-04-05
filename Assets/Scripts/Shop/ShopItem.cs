using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopItem : MonoBehaviour
{
    // update player stats
    private PlayerController player;

    // Shop item variables
    public float cost;
    public float swordLengthIncrease;
    public float attackSpeedIncrease;
    public float dashLengthIncrease;
    public float swordDamageIncrease;
    public float moveSpeedIncrease;
    public float lifeStealIncrease;

    // Start is called before the first frame update
    void Start()
    {
        GameObject temp = GameObject.Find("Player");
        player = temp.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        // purchase things (uses attack button)
        if (Input.GetKeyDown(KeyCode.X))
        {
            // confirm that the text is visible
            if (transform.GetChild(0).gameObject.activeSelf)
            {
                // pay for the item
                player.RemainingTime -= cost;
                player.getItemSFX.Play();

                // update the players stats
                player.dashDuration += dashLengthIncrease;
                player.maxAttackCooldown += attackSpeedIncrease;
                player.attackDamage -= swordDamageIncrease;
                player.moveSpeed += moveSpeedIncrease;
                player.lifeStealConstant += lifeStealIncrease;

                // update attack hitbox size
                Vector2 attackPointPos = player.transform.GetChild(1).transform.position;
                // need to figure out whether to add or subtract to the position.x
                Vector2 basePoint = player.transform.GetChild(0).transform.position;
                int sign = (int)Mathf.Sign(attackPointPos.x - basePoint.x);
                attackPointPos.x += sign * swordLengthIncrease;
                player.transform.GetChild(1).transform.position = attackPointPos;

                // remove this item from the shop
                gameObject.SetActive(false);

                TextMeshProUGUI keeperText = GetComponentInParent<ItemRandomiser>().keeperText;
                keeperText.text = "Pleasure doing business with you";
                keeperText.GetComponentInParent<TextAnimate>().charsVisible = 0;
            }
        }
    }

    // Handle shop things
    private void OnTriggerStay2D(Collider2D collision)
    {
        string objname = collision.gameObject.name;
        if (objname == "Player")
        {
            // make text visible
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        string objname = collision.gameObject.name;
        if (objname == "Player")
        {
            // turn text off
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
