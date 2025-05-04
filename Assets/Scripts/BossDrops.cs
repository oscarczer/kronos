using UnityEngine;

public class BossDrops : MonoBehaviour
{
    private PlayerController player;
    public bool jump;
    public bool dash;
    public GameObject bossTrans;

    void Start()
    {
        GameObject temp = GameObject.Find("Player");
        player = temp.GetComponent<PlayerController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // confirm that the text is visible
            if (transform.GetChild(0).gameObject.activeSelf)
            {
                if (jump)
                {
                    player.maxJumps += 1;
                }

                if (dash)
                {
                    player.maxDashes += 1;
                }

                if (!bossTrans.activeInHierarchy)
                {
                    bossTrans.SetActive(true);
                }

                GameObject.Find("Boss Drops").SetActive(false);
            }
        }
    }

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
