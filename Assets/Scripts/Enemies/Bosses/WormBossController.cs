using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WormBossController : MonoBehaviour
{
    private GameObject player;

    public AudioSource bossWin;
    public AudioSource wind;
    public AudioSource music;

    public GameObject bossTitleCard;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(DisplayName());
    }

    void Update()
    {
        GameObject[] wormHeads = GameObject.FindGameObjectsWithTag("WormHead");

        if (wormHeads.Length == 0)
        {
            music.Stop();
            bossWin.Play();

            player.GetComponent<PlayerController>().TimePaused = true;

            wind.Play();

            //  text  that  says  "Boss  defeated"
            bossTitleCard.SetActive(true);
            bossTitleCard.transform.GetChild(0).localScale = new Vector3(29, 3, 1);
            bossTitleCard.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text +=
                "  defeated";
            Destroy(bossTitleCard.gameObject, 3f);
        }
    }

    private IEnumerator DisplayName()
    {
        bossTitleCard.SetActive(true);

        yield return new WaitForSeconds(2f);

        bossTitleCard.SetActive(false);
    }
}
