using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OrbBossController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private PlayerController player;
    public GameObject bossDrops;
    public GameObject projectile;
    public GameObject timePickup;
    public GameObject bossTitleCard;


    private float timeSinceLastTeleport;
    private float cooldown = 5.0f;

    private float maxHealth = 20f;
    private float currentHealth;
    private bool angry = false;
    
    public bool cutscene = true;

    public GameObject healthPopup;
    public AudioSource bossShoot;
    public AudioSource bossWin;
    public AudioSource wind;
    public AudioSource music;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        StartCoroutine(CutScene());
    }

    private IEnumerator CutScene()
    {
        player.immobile = true;

        CircleCollider2D circle = GetComponent<CircleCollider2D>();
        circle.enabled = false;

        while (transform.position != new Vector3(0, 1, 0))
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(0, 1, 0), 1.5f * Time.deltaTime);
            yield return null;
        }
        // Show name here
        bossTitleCard.SetActive(true);

        yield return new WaitForSeconds(2f);

        bossTitleCard.SetActive(false);
        yield return new WaitForSeconds(0.3f);

        // Game on
        circle.enabled = true;

        cutscene = false;
        player.TimePaused = false;
        currentHealth = maxHealth;
        player.immobile = false;

        Teleport();
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(ShotgunAttack());

        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        if (cutscene || player.IsDead)
        {
            player.TimePaused = true;
            return;
        }

        // teleport and attack loop
        if (timeSinceLastTeleport < cooldown)
        {
            timeSinceLastTeleport += Time.deltaTime;
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            Teleport();
            timeSinceLastTeleport = 0f;
            // pick a random attack to do
            int randomAttack = Random.Range(0, 3);
            switch (randomAttack) {
                case 2:
                    StartCoroutine(ExPlusAttack());
                    break;
                case 1:
                    StartCoroutine(SpiralAttack());
                    break;
                default:
                    StartCoroutine(ShotgunAttack());
                    break;
            }
        }

        // when low health, get mad
        if (!angry && currentHealth < maxHealth / 3)
        {
            GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 1);
            angry = true;
            Teleport();
        }
        
    }

    private void Spawn(float angle)
    {
        GameObject shot;
        Vector2 velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        // 1 in 15 of being helpful 
        if (Random.Range(0, 15) == 0)
        {
            shot = Instantiate(timePickup, transform.position, timePickup.transform.rotation);
            shot.GetComponent<Collectible>().thrust = velocity;
            shot.GetComponent<Collectible>().destroyOffScreen = true;
            shot.GetComponent<Collectible>().timeGained = 7f;
        }
        else
        {
            shot = Instantiate(projectile, transform.position, projectile.transform.rotation);
            shot.GetComponent<OrbProjectileController>().thrust = velocity;
        }
        
        //bossShoot.Play();
    }


    private void Teleport()
    {
        rb.position = new Vector2(Random.Range(-19f, 19f), Random.Range(-7f, 6f));
    }


    private IEnumerator ShotgunAttack()
    {
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < 360; i += 33)
        {
            Spawn(Mathf.Deg2Rad * i);
        }
        yield return new WaitForSeconds(0.5f);
        if (!angry) yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < 360; i += 26)
        {
            Spawn(Mathf.Deg2Rad * i);
        }
        if (angry)
        {
            yield return new WaitForSeconds(0.5f);

            for (int i = 0; i < 360; i += 19)
            {
                Spawn(Mathf.Deg2Rad * i);
            }
            yield return new WaitForSeconds(0.5f);
            Teleport();
        }

        yield return null;
    }

    private IEnumerator SpiralAttack()
    {
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < 720; i += 30)
        {
            Spawn(Mathf.Deg2Rad * i);
            if (angry) Spawn(Mathf.Deg2Rad * (i+180));
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.1f);
        if (angry) Teleport();
    }

    private IEnumerator ExPlusAttack()
    {
        // shoots in 4 directions orthogonally, then 4 directions diagonally
        yield return new WaitForSeconds(0.5f);
        for (int j = 0; j < 4; j++)
        {
            for (int i = 20; i < 360; i += 90)
            {
                Spawn(Mathf.Deg2Rad * i);
                if (angry) Spawn(Mathf.Deg2Rad * (i + 5));
            }
            yield return new WaitForSeconds(0.3f);
        }
        yield return new WaitForSeconds(0.1f);
        for (int j = 0; j < 4; j++)
        {
            for (int i = 50; i < 360; i += 90)
            {
                Spawn(Mathf.Deg2Rad * i);
                if (angry) Spawn(Mathf.Deg2Rad * (i + 5));
            }
            yield return new WaitForSeconds(0.3f);
        }
        //yield return new WaitForSeconds(0.1f);
        //for (int j = 0; j < 4; j++)
        //{
        //    for (int i = 80; i < 360; i += 90)
        //    {
        //        Spawn(Mathf.Deg2Rad * i);
        //        if (angry) Spawn(Mathf.Deg2Rad * (i+5));
        //    }
        //    yield return new WaitForSeconds(0.3f);
        //}
        yield return null;
        if (angry) Teleport();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Projectile" || collision.gameObject.tag == "Player" || collision.gameObject.tag == "Platform")
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }

    public void TakeDamage(float damage)
    {
        if (cutscene) { return; }

        currentHealth += damage;
        anim.SetTrigger("isHit");

        if (currentHealth <= 0) { Die(); }
    }

    private void Die()
    {
        rb.linearVelocity = new Vector2(0, 0);

        music.Stop();
        bossWin.Play();

        GameObject[] projectiles = GameObject.FindGameObjectsWithTag("Projectile");
        foreach (GameObject projectile in projectiles)
        {
            GameObject.Destroy(projectile);
        }

        if (!bossDrops.activeInHierarchy)
        {
            bossDrops.SetActive(true);
        }

        wind.Play();
        player.AlterTime(20);
        player.TimePaused = true;
        anim.SetTrigger("isDead");
        Destroy(gameObject, 1f);

        // text that says "hubert defeated"
        bossTitleCard.SetActive(true);
        bossTitleCard.transform.GetChild(0).localScale = new Vector3(25,3,1);
        bossTitleCard.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Hubert defeated";
        Destroy(bossTitleCard.gameObject, 3f);
    }
}
