using TMPro;
using UnityEngine;

public class ShopItem : MonoBehaviour
{
    private PlayerController player;

    // Shop item variables
    public float cost;

    // TODO: Make these fixed values and also an enum type rather than prefab variants
    public float swordLengthIncrease;
    public float attackSpeedIncrease;
    public float dashLengthIncrease;
    public float swordDamageIncrease;
    public float moveSpeedIncrease;
    public float lifeStealIncrease;

    void Start()
    {
        GameObject temp = GameObject.Find("Player");
        player = temp.GetComponent<PlayerController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // Confirm that the text is visible
            if (transform.GetChild(0).gameObject.activeSelf)
            {
                // Pay for the item
                player.RemainingTime -= cost;
                player.getItemSFX.Play();

                // Update the players stats
                player.dashDuration += dashLengthIncrease;
                player.maxAttackCooldown += attackSpeedIncrease;
                player.attackDamage -= swordDamageIncrease;
                player.moveSpeed += moveSpeedIncrease;
                player.lifeStealConstant += lifeStealIncrease;

                // Update attack hitbox size
                Vector2 attackPointPos = player.transform.GetChild(1).transform.position;
                Vector2 basePoint = player.transform.GetChild(0).transform.position;
                int sign = (int)Mathf.Sign(attackPointPos.x - basePoint.x);
                attackPointPos.x += sign * swordLengthIncrease;
                player.transform.GetChild(1).transform.position = attackPointPos;

                // Remove this item from the shop
                gameObject.SetActive(false);

                TextMeshProUGUI keeperText = GetComponentInParent<ItemRandomiser>().keeperText;
                keeperText.text = "Pleasure doing business with you";
                keeperText.GetComponentInParent<TextAnimate>().charsVisible = 0;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        string objname = collision.gameObject.name;
        if (objname == "Player")
        {
            // Make text visible
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        string objname = collision.gameObject.name;
        if (objname == "Player")
        {
            // Turn text off
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
