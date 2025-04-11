using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    #region Global Variables
    // Main Player Variables
    [SerializeField]
    PlayerData data;

    private Rigidbody2D playerRb;
    private float remainingTime;
    private bool timePaused = false;
    private bool isDead = false;
    public bool immobile = true;
    public bool gamePaused = false;
    public bool invulnerable = false;

    public float RemainingTime
    {
        get => remainingTime;
        set => remainingTime = value;
    }
    public bool TimePaused
    {
        get => timePaused;
        set => timePaused = value;
    }
    public bool IsDead
    {
        get => isDead;
        set => isDead = value;
    }

    // Interface Variables
    private Animator anim;
    public GameObject gameOverParent;
    private AudioSource normalMusic;
    public AudioSource shopMusic;
    public bool isTutorial = false;
    private GameObject pauseOverlay;
    public GameObject healthPopup;

    // Jump Specific Variables
    public int maxJumps = 2;
    private int remainingJumps;
    public float jumpSpeed;
    private bool isJumping = false;
    private float upSpeed = 0;

    // Dash Specific Variables
    public int maxDashes = 1;
    private int remainingDashes;
    public float dashLength = 13.0f;
    private bool isDashing = false;
    private bool dashOnCooldown = false;
    public float dashSpeed = 25.0f;
    public float dashDuration = 0.05f;
    public float dashCooldown = 0.1f;
    private bool canBufferDash = true;

    // Player Movement States
    private bool facingRight = true;
    private bool onPlatform = false;
    private bool isGrounded = false;
    private bool platformFalling = false;
    private bool canGoLeft = false;
    private bool canGoRight = false;

    // Other Movement Variables
    public float moveSpeed = 8.0f;
    public float gravAcc;
    public float maxGrav;
    public LayerMask groundLayer;
    public LayerMask platformLayer;
    private DashDirection dashDirection;

    private enum DashDirection
    {
        Up,
        Down,
        Left,
        Right,
        UpRight,
        DownRight,
        DownLeft,
        UpLeft,
    }

    // Attack Variables
    public float attackDamage;
    private Transform attackPoint1;
    private Transform attackPoint2;
    public LayerMask enemyLayers;
    private float attackCooldown;
    public float maxAttackCooldown = 0.5f;
    public float lifeStealConstant = 0;

    // Sound Effects
    public AudioSource dashSFX;
    public AudioSource attackSFX;
    public AudioSource damagedSFX;
    public AudioSource killedSFX;
    public AudioSource gameOverSFX;
    public AudioSource getItemSFX;

    private Vector3 knockDireciton;
    private bool isKnocking = false;
    public float knockInitial;
    private float knockCurrent = 0.0f;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        GetCurrentDataFields();

        playerRb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        normalMusic = GameObject.Find("Music").GetComponent<AudioSource>();
        pauseOverlay = transform.GetChild(4).gameObject;

        remainingJumps = maxJumps;
        remainingDashes = maxDashes;

        attackCooldown = 0;

        attackPoint1 = transform.GetChild(0).transform;
        attackPoint2 = transform.GetChild(1).transform;

        if (!isTutorial)
        {
            StartCoroutine(CutScene());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead && !immobile)
        {
            // Constantly reduce time until 0 at which point hit a game over point
            if (remainingTime < 0)
            {
                isDead = true;
                normalMusic = GameObject.Find("Music").GetComponent<AudioSource>();
                normalMusic.Stop();

                GameObject shopMusicGO = GameObject.Find("Shop Music");

                if (shopMusicGO != null)
                {
                    shopMusicGO.GetComponent<AudioSource>().Stop();
                }

                gameOverSFX.Play();
                anim.SetTrigger("dead");
                anim.SetBool("isDead", true);
                GameObject.Find("GUI").SetActive(false);
                GameObject gameOverText = gameOverParent.transform.Find("GameOverText").gameObject;
                gameOverText.transform.localScale = transform.localScale;
                gameOverText.SetActive(true);
                StartCoroutine(GameOver());
            }
            else if (!timePaused && !isTutorial)
            {
                remainingTime -= Time.deltaTime;
            }

            Move();
            Jump();
            DownPlatform();
            Dash();
            Attack();
            CheckGround();
            CheckWalls();
            KnockBack();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
            HandlePause();

        // shop audio handling
        if (shopMusic != null)
        {
            normalMusic.volume = 1f - 2 * shopMusic.volume;
            if (timePaused)
            {
                if (shopMusic.volume <= 0.5f)
                    shopMusic.volume += 0.05f;
            }
            else if (shopMusic.volume >= 0f)
                shopMusic.volume -= 0.05f;
        }
    }

    // general movement handler
    private void Move()
    {
        bool right = Input.GetKey(KeyCode.RightArrow);
        bool left = Input.GetKey(KeyCode.LeftArrow);

        if (right || left)
        {
            anim.SetBool("walking", true);
        }
        else
        {
            anim.SetBool("walking", false);
        }

        if (right)
        {
            facingRight = true;
            transform.localScale = new Vector2(1, 1);
        }
        else if (left)
        {
            facingRight = false;
            transform.localScale = new Vector2(-1, 1);
        }

        // Dashing code:
        if (isDashing)
        {
            if (dashDirection == DashDirection.Right && canGoRight)
            {
                Vector3 newPos = transform.position + Vector3.right * dashSpeed * Time.deltaTime;
                if (FutureWallCheck(newPos, true))
                {
                    transform.position = newPos;
                }
            }

            if (dashDirection == DashDirection.Left && canGoLeft)
            {
                Vector3 newPos = transform.position + Vector3.left * dashSpeed * Time.deltaTime;
                if (FutureWallCheck(newPos, false))
                {
                    transform.position = newPos;
                }
            }
        }
        else
        {
            if (right && canGoRight)
            {
                Vector3 newPos = transform.position + Vector3.right * moveSpeed * Time.deltaTime;
                if (FutureWallCheck(newPos, true))
                {
                    transform.position = newPos;
                }
            }
            else if (left && canGoLeft)
            {
                Vector3 newPos = transform.position + Vector3.left * moveSpeed * Time.deltaTime;
                if (FutureWallCheck(newPos, false))
                {
                    transform.position = newPos;
                }
            }
        }
    }

    // jump action handler
    private void Jump()
    {
        if (isDashing)
        {
            upSpeed = 0;
        }

        if (!isGrounded)
        {
            if (upSpeed <= 0)
            {
                isJumping = false;
            }
            // Jump higher if we keep holding c
            if (isJumping && Input.GetKey(KeyCode.Z) && !isKnocking)
            {
                upSpeed -= Time.deltaTime * 0.54f * gravAcc;
                // going up still:
                Vector3 newPos = transform.position + Vector3.up * upSpeed * Time.deltaTime;
                if (FutureCheckCeiling(newPos))
                {
                    transform.position = newPos;
                }
            }
            // Fall faster if we have stopped holding jump
            else
            {
                if (upSpeed > maxGrav)
                {
                    upSpeed -= Time.deltaTime * gravAcc;
                }
                Vector3 newPos = transform.position + Vector3.up * upSpeed * Time.deltaTime;
                if (upSpeed < 0 && FutureCheckGround(newPos))
                {
                    transform.position = newPos;
                }
                else if (upSpeed > 0 && FutureCheckCeiling(newPos))
                {
                    transform.position = newPos;
                }
            }
        }

        if (remainingJumps > 0 && Input.GetKeyDown(KeyCode.Z) && !isKnocking)
        {
            anim.SetTrigger("jump");
            upSpeed = jumpSpeed;
            playerRb.AddForce(Vector2.up * upSpeed);
            isGrounded = false;
            isJumping = true;
            remainingJumps--;
        }
    }

    // dash action handler
    private void Dash()
    {
        // Reset dash buffer when dash button is released
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            canBufferDash = true;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            // Dash code:
            if (remainingDashes > 0 && !dashOnCooldown && canBufferDash)
            {
                dashSFX.Play();
                // make invulnerable while dashing
                invulnerable = true;

                canBufferDash = false;
                remainingDashes--;
                isDashing = true;
                anim.SetTrigger("dash");

                //float vertical = Input.GetAxis("Vertical");

                bool right = Input.GetKey(KeyCode.RightArrow);

                if (right || facingRight)
                {
                    dashDirection = DashDirection.Right;
                }
                else
                {
                    dashDirection = DashDirection.Left;
                }

                StartCoroutine(DashDelay());
            }
        }
    }

    // down platform action handler
    private void DownPlatform()
    {
        if (onPlatform && (Input.GetKey(KeyCode.DownArrow)))
        {
            isGrounded = false;
            onPlatform = false;
            platformFalling = true;
            StartCoroutine(PlatformFallDelay());
        }
    }

    // ground collision check
    private void CheckGround()
    {
        if (platformFalling)
        {
            return;
        }

        if (!isJumping)
        {
            bool previouslyGrounded = isGrounded || onPlatform;
            Vector2 rayStartLeft = new Vector2(
                transform.position.x - 0.2f,
                transform.position.y - 0.8f
            );
            Vector2 rayStartRight = new Vector2(
                transform.position.x + 0.2f,
                transform.position.y - 0.8f
            );

            // Check we aren't inside a platform:
            //if (Physics2D.Raycast(rayStartLeft, Vector2.up, 0.15f, platformLayer).collider != null ||
            //    Physics2D.Raycast(rayStartRight, Vector2.up, 0.15f, platformLayer).collider != null)
            //{
            //    return;
            //}

            RaycastHit2D groundLeft = Physics2D.Raycast(
                rayStartLeft,
                Vector2.down,
                0.35f,
                groundLayer
            );
            RaycastHit2D groundRight = Physics2D.Raycast(
                rayStartRight,
                Vector2.down,
                0.35f,
                groundLayer
            );
            RaycastHit2D platformLeft = Physics2D.Raycast(
                rayStartLeft,
                Vector2.down,
                0.35f,
                platformLayer
            );
            RaycastHit2D platformRight = Physics2D.Raycast(
                rayStartRight,
                Vector2.down,
                0.35f,
                platformLayer
            );

            isGrounded = groundLeft.collider != null || groundRight.collider != null;

            onPlatform = platformLeft.collider != null || platformRight.collider != null;

            // If we fell off a platform, don't allow double jump:
            if (!isGrounded && previouslyGrounded)
            {
                remainingJumps = maxJumps - 1;
            }

            if (isGrounded)
            {
                landPosition(groundLeft, groundRight);
            }
            else if (onPlatform)
            {
                landPosition(platformLeft, platformRight);
            }
        }
        // Checking ceiling:
        else
        {
            Vector2 rayStartLeft = new Vector2(
                transform.position.x - 0.2f,
                transform.position.y + 0.2f
            );
            Vector2 rayStartRight = new Vector2(
                transform.position.x + 0.2f,
                transform.position.y + 0.2f
            );

            if (
                Physics2D.Raycast(rayStartLeft, Vector2.up, 0.15f, groundLayer).collider != null
                || Physics2D.Raycast(rayStartRight, Vector2.up, 0.15f, groundLayer).collider != null
            )
            {
                // Hit head on ceiling
                upSpeed = 0;
            }
        }
    }

    // wall collision check
    private void CheckWalls()
    {
        Vector2 rayStartBottomLeft = new Vector2(
            transform.position.x - 0.2f,
            transform.position.y - 0.9f
        );
        Vector2 rayStartBottomRight = new Vector2(
            transform.position.x + 0.2f,
            transform.position.y - 0.9f
        );
        Vector2 rayStartTopLeft = new Vector2(
            transform.position.x - 0.2f,
            transform.position.y + 0.2f
        );
        Vector2 rayStartTopRight = new Vector2(
            transform.position.x + 0.2f,
            transform.position.y + 0.2f
        );

        canGoLeft = !(
            Physics2D.Raycast(rayStartBottomLeft, Vector2.left, 0.15f, groundLayer).collider != null
            || Physics2D.Raycast(rayStartTopLeft, Vector2.left, 0.15f, groundLayer).collider != null
        );

        canGoRight = !(
            Physics2D.Raycast(rayStartBottomRight, Vector2.right, 0.15f, groundLayer).collider
                != null
            || Physics2D.Raycast(rayStartTopRight, Vector2.right, 0.15f, groundLayer).collider
                != null
        );
    }

    private bool FutureWallCheck(Vector3 newPosition, bool right)
    {
        if (right)
        {
            Vector2 rayStartTopRight = new Vector2(
                transform.position.x + 0.2f,
                transform.position.y + 0.2f
            );
            Vector2 rayStartBottomRight = new Vector2(
                transform.position.x + 0.2f,
                transform.position.y - 0.9f
            );

            return !(
                Physics2D.Raycast(rayStartBottomRight, Vector2.right, 0.15f, groundLayer).collider
                    != null
                || Physics2D.Raycast(rayStartTopRight, Vector2.right, 0.15f, groundLayer).collider
                    != null
            );
        }
        else
        {
            Vector2 rayStartBottomLeft = new Vector2(
                transform.position.x - 0.2f,
                transform.position.y - 0.9f
            );
            Vector2 rayStartTopLeft = new Vector2(
                transform.position.x - 0.2f,
                transform.position.y + 0.2f
            );

            return !(
                Physics2D.Raycast(rayStartBottomLeft, Vector2.left, 0.15f, groundLayer).collider
                    != null
                || Physics2D.Raycast(rayStartTopLeft, Vector2.left, 0.15f, groundLayer).collider
                    != null
            );
        }
    }

    private bool FutureCheckGround(Vector3 newPosition)
    {
        if (platformFalling)
        {
            return true;
        }

        Vector2 rayStartLeft = new Vector2(
            transform.position.x - 0.2f,
            transform.position.y - 0.8f
        );
        Vector2 rayStartRight = new Vector2(
            transform.position.x + 0.2f,
            transform.position.y - 0.8f
        );

        RaycastHit2D groundLeft = Physics2D.Raycast(rayStartLeft, Vector2.down, 0.35f, groundLayer);
        RaycastHit2D groundRight = Physics2D.Raycast(
            rayStartRight,
            Vector2.down,
            0.35f,
            groundLayer
        );
        RaycastHit2D platformLeft = Physics2D.Raycast(
            rayStartLeft,
            Vector2.down,
            0.35f,
            platformLayer
        );
        RaycastHit2D platformRight = Physics2D.Raycast(
            rayStartRight,
            Vector2.down,
            0.35f,
            platformLayer
        );

        return !(
            (groundLeft.collider != null || groundRight.collider != null)
            || (platformLeft.collider != null || platformRight.collider != null)
        );
    }

    private bool FutureCheckCeiling(Vector3 newPosition)
    {
        Vector2 rayStartLeft = new Vector2(
            transform.position.x - 0.2f,
            transform.position.y + 0.2f
        );
        Vector2 rayStartRight = new Vector2(
            transform.position.x + 0.2f,
            transform.position.y + 0.2f
        );

        if (
            Physics2D.Raycast(rayStartLeft, Vector2.up, 0.15f, groundLayer).collider != null
            || Physics2D.Raycast(rayStartRight, Vector2.up, 0.15f, groundLayer).collider != null
        )
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    // helper function for calculating landing position
    private void landPosition(RaycastHit2D hitLeft, RaycastHit2D hitRight)
    {
        float updatedYPos;
        upSpeed = 0;
        if (hitLeft.collider != null && hitRight.collider != null)
        {
            updatedYPos = Mathf.Min(hitLeft.point.y, hitRight.point.y) + 1.0f;
        }
        else if (hitLeft.collider == null)
        {
            updatedYPos = hitRight.point.y + 1.0f;
        }
        else
        {
            updatedYPos = hitLeft.point.y + 1.0f;
        }

        transform.position = new Vector3(transform.position.x, updatedYPos, transform.position.z);
        remainingJumps = maxJumps;
        remainingDashes = maxDashes;
    }

    // If attack is availabe, deal damage to any enemies inside the hurtbox
    private void Attack()
    {
        if (Input.GetKeyDown(KeyCode.X) && attackCooldown <= 0)
        {
            anim.SetTrigger("attack");
            attackSFX.Play();

            Collider2D[] hitEnemies = Physics2D.OverlapAreaAll(
                attackPoint1.position,
                attackPoint2.position,
                enemyLayers
            );

            foreach (Collider2D enemy in hitEnemies)
            {
                if (enemy.name == "Hitbox")
                {
                    enemy.GetComponentInParent<EnemyController>().TakeDamage(attackDamage);
                }
                else if (enemy.tag == "Bullet")
                {
                    enemy.GetComponent<EnemyController>().Die();
                }
                else if (enemy.tag == "OrbBoss")
                {
                    enemy.GetComponent<OrbBossController>().TakeDamage(attackDamage);
                }
                else if (enemy.tag == "SkeleBoss")
                {
                    enemy.GetComponent<SkeleBoss>().TakeDamage(attackDamage);
                }
                else if (enemy.tag == "Dirt")
                {
                    Destroy(enemy.gameObject);
                }
                else if (enemy.tag == "WormBody")
                {
                    enemy.GetComponent<WormBossBody>().health += attackDamage;
                }
                else if (enemy.tag == "WormHead")
                {
                    enemy.GetComponent<WormBossHead>().health += attackDamage;
                }
            }
            attackCooldown = maxAttackCooldown;
        }
        attackCooldown -= Time.deltaTime;
    }

    public void StartKnockBack(Vector3 enemyPos)
    {
        isKnocking = true;
        knockCurrent = knockInitial;
        if (enemyPos.x > transform.position.x)
        {
            knockDireciton = Vector3.left;
        }
        else
        {
            knockDireciton = Vector3.right;
        }
    }

    private void KnockBack()
    {
        if (isKnocking)
        {
            if (knockDireciton.x < 0 && canGoLeft)
            {
                Vector3 newPos =
                    transform.position + knockDireciton * knockCurrent * Time.deltaTime;
                if (FutureWallCheck(newPos, false))
                {
                    transform.position = newPos;
                }
            }
            else if (knockDireciton.x > 0 && canGoRight)
            {
                Vector3 newPos =
                    transform.position + knockDireciton * knockCurrent * Time.deltaTime;
                if (FutureWallCheck(newPos, true))
                {
                    transform.position = newPos;
                }
            }

            knockCurrent -= Time.deltaTime * 50;
            if (knockCurrent < 0)
            {
                isKnocking = false;
            }
        }
    }

    private void HandlePause()
    {
        if (gamePaused)
        {
            // unpause
            timePaused = false;
            immobile = false;
            pauseOverlay.SetActive(false);
            normalMusic.UnPause();
            if (shopMusic.clip != null)
                shopMusic.UnPause();
        }
        else
        {
            // pause game
            timePaused = true;
            immobile = true;
            pauseOverlay.SetActive(true);
            normalMusic.Pause();
            if (shopMusic.clip != null)
                shopMusic.Pause();
            // other things that need to be done:
            // stop enemies from attacking
        }
        gamePaused = !gamePaused;
    }

    // Pause the timer when in shop
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Pause Timer")
        {
            timePaused = true;
        }
        if (collision.gameObject.name == "Load Data Trigger")
        {
            SetDefaultFields();
            GetCurrentDataFields();
        }
        if (collision.gameObject.name == "Start Game Trigger")
        {
            // only do this the first time
            if (!normalMusic.isPlaying)
            {
                // Set timer to starting time
                timePaused = false;
                // make timer visible
                GameObject.Find("GUI").transform.GetChild(0).gameObject.SetActive(true);

                // hopefully not starting the songs at the same time reduces lag
                normalMusic.Play();
                shopMusic.PlayDelayed(1f);
            }
        }

        // TODO: Get rid of this
        if (collision.gameObject.name == "Start Level Trigger")
        {
            isTutorial = false;
            // Set timer to starting time
            timePaused = false;
            // make timer visible
            GameObject.Find("GUI").transform.GetChild(0).gameObject.SetActive(true);

            // make sure this cant be activated again
            collision.gameObject.SetActive(false);
        }

        if (!invulnerable)
        {
            if (collision.tag == "WormBody")
            {
                AlterTime(-3);
            }
            if (collision.tag == "WormHead")
            {
                AlterTime(-6);
            }
        }
    }

    // Unpause timer after exiting shops
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Pause Timer")
            timePaused = false;
    }

    // function for altering and displaying health
    public void AlterTime(float timeGained)
    {
        if (!isTutorial)
        {
            remainingTime += timeGained;
            if (timeGained > 0)
            {
                // Instantiate little text thing that says '+15' or whatever
                healthPopup.GetComponentInChildren<TextMeshProUGUI>().text =
                    "+" + (timeGained + lifeStealConstant).ToString();
                remainingTime += lifeStealConstant;
                killedSFX.Play();
            }
            else
            {
                // Instantiate little text thing that says '+15' or whatever
                healthPopup.GetComponentInChildren<TextMeshProUGUI>().text = timeGained.ToString();
                anim.SetTrigger("hit");
                damagedSFX.Play();
                StartCoroutine(IFrames(0.5f));
            }
            Instantiate(healthPopup, transform.position, healthPopup.transform.rotation);
        }
        else
        {
            if (timeGained < 0)
            {
                healthPopup.GetComponentInChildren<TextMeshProUGUI>().text = "OUCH";
                Instantiate(healthPopup, transform.position, healthPopup.transform.rotation);
            }
        }
    }

    // i-frames
    private IEnumerator IFrames(float timeInvulnerable)
    {
        invulnerable = true;
        yield return new WaitForSeconds(timeInvulnerable);
        invulnerable = false;
    }

    // Platform falling timing sequence
    private IEnumerator PlatformFallDelay()
    {
        // Wait for 0.05 seconds.
        yield return new WaitForSeconds(0.3f);

        platformFalling = false;
    }

    // Dash timing sequence
    private IEnumerator DashDelay()
    {
        // Dash
        yield return new WaitForSeconds(dashDuration);

        // Dashing has finised
        isDashing = false;
        dashOnCooldown = true;

        // Give player a little bit of time before we turn hitbox back on
        yield return new WaitForSeconds(0.1f);
        invulnerable = false;

        // Dash cooldown
        yield return new WaitForSeconds(dashCooldown);
        dashOnCooldown = false;
    }

    // Cutscene handler
    private IEnumerator CutScene()
    {
        timePaused = true;

        // Set fall speed to be slower than normal, player maintains control
        maxGrav = maxGrav / 1.5f;
        remainingJumps = 0;
        remainingDashes = 0;

        // wait to hit ground
        while (!isGrounded)
        {
            yield return null;
        }
        // set fall speed back to normal
        maxGrav = maxGrav * 1.5f;
        yield return null;
    }

    // Game Over cutscene
    private IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1f);

        // Screen closing thingy
        GameObject parent = gameOverParent.transform.Find("GameOverTransition").gameObject;
        parent.SetActive(true);

        Transform top = parent.transform.Find("TopSquare").transform;
        Transform bottom = parent.transform.Find("BottomSquare").transform;
        while (top.localPosition.y > 70)
        {
            top.Translate(Vector2.down * Time.deltaTime * 2);
            bottom.Translate(Vector2.up * Time.deltaTime * 2);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        switch (SceneManager.GetActiveScene().name)
        {
            case "Level2":
                SceneManager.LoadScene("Level2", LoadSceneMode.Single);
                break;
            case "BossFight2":
                SceneManager.LoadScene("Level2", LoadSceneMode.Single);
                break;
            case "Level3":
                SceneManager.LoadScene("Level3", LoadSceneMode.Single);
                break;
            case "BossFight3":
                SceneManager.LoadScene("Level3", LoadSceneMode.Single);
                break;
            default:
                SceneManager.LoadScene("Level1", LoadSceneMode.Single);
                break;
        }
    }

    private void GetCurrentDataFields()
    {
        remainingTime = data.RemainingTime;
        maxJumps = data.MaxJumps;
        maxDashes = data.MaxDashes;
        dashDuration = data.DashDuration;
        maxAttackCooldown = data.MaxAttackCooldown;
        attackDamage = data.AttackDamage;
        moveSpeed = data.MoveSpeed;
        lifeStealConstant = data.LifeStealConstant;
    }

    private void SetCurrentDataFields()
    {
        data.RemainingTime = remainingTime;
        data.MaxJumps = maxJumps;
        data.MaxDashes = maxDashes;
        data.DashDuration = dashDuration;
        data.MaxAttackCooldown = maxAttackCooldown;
        data.AttackDamage = attackDamage;
        data.MoveSpeed = moveSpeed;
        data.LifeStealConstant = lifeStealConstant;
    }

    private void SetDefaultFields()
    {
        data.RemainingTime = 60;
        data.MaxJumps = 2;
        data.MaxDashes = 1;
        data.DashDuration = 0.08f;
        data.MaxAttackCooldown = 0.4f;
        data.AttackDamage = -3;
        data.MoveSpeed = 8;
        data.LifeStealConstant = 0;
    }
}
