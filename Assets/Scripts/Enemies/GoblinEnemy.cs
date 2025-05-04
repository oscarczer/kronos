using System.Collections;
using UnityEngine;

public class GoblinEnemy : FollowingEnemy
{
    private Rigidbody2D enemyRb;
    public LayerMask groundLayer;
    public LayerMask platformLayer;
    public float gravAcc;
    public float maxGrav;

    // Player Movement States
    private bool onPlatform = false;
    private bool isGrounded = false;
    private bool platformFalling = false;
    private bool canGoLeft = false;
    private bool canGoRight = false;

    // Jump Specific Variables
    public float jumpSpeed;
    private bool isJumping = false;
    private float upSpeed = 0;

    void Start()
    {
        Player = (GameObject.FindGameObjectWithTag("Player"));
        Cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        enemyRb = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        CurrentHealth = maxHealth;
    }

    void Update()
    {
        CheckGround();
        CheckWalls();

        if (
            chase
            && !Player.GetComponent<PlayerController>().IsDead
            && !IsDead
            && AttackCooldown <= 0
        )
        {
            DownPlatform();
            Move();
            Jump();

            FacePlayer();

            StopChasingOffScreen();
        }
        AttackCooldown -= Time.deltaTime;

        if (IsKnocking)
        {
            KnockbackHandler();
        }
    }

    // General movement handler
    private void Move()
    {
        if (
            transform.position.x <= Player.transform.position.x + .5f
            && transform.position.x >= Player.transform.position.x - .5f
        )
        {
            Anim.SetBool("run", false);
            return;
        }

        bool advanceRight = false;

        if (transform.position.x < Player.transform.position.x)
        {
            advanceRight = true;
        }

        Vector3 newPos = transform.position + Vector3.left * moveSpeed * Time.deltaTime;

        if (advanceRight && canGoRight)
        {
            newPos = transform.position + Vector3.right * moveSpeed * Time.deltaTime;
        }

        if (FutureWallCheck(newPos, advanceRight))
        {
            FacePlayer();
            Anim.SetBool("run", true);
            transform.position = newPos;
        }
    }

    // Jump action handler
    private void Jump()
    {
        if (Player.transform.position.y > transform.position.y + 0.2f && (isGrounded || onPlatform))
        {
            upSpeed = jumpSpeed;
            Vector3 newPos = transform.position + Vector3.up * upSpeed * Time.deltaTime;
            if (FutureCheckCeiling(newPos))
            {
                transform.position = newPos;
            }
            isGrounded = false;
            onPlatform = false;
            isJumping = true;
        }
    }

    // Down platform action handler
    private void DownPlatform()
    {
        if (onPlatform && Player.transform.position.x < transform.position.x - .5f)
        {
            isGrounded = false;
            onPlatform = false;
            platformFalling = true;
            StartCoroutine(PlatformFallDelay());
        }
    }

    // Ground collision check
    private void CheckGround()
    {
        if (!isGrounded && !onPlatform)
        {
            if (upSpeed <= 0)
            {
                isJumping = false;
            }

            if (upSpeed > maxGrav)
            {
                upSpeed -= Time.deltaTime * gravAcc;
            }

            Vector3 newPos = transform.position + Vector3.up * upSpeed * Time.deltaTime;

            if (
                upSpeed < 0 && FutureCheckGround(newPos)
                || upSpeed > 0 && FutureCheckCeiling(newPos)
            )
            {
                transform.position = newPos;
            }
        }

        if (platformFalling)
        {
            return;
        }

        if (!isJumping)
        {
            Vector2 rayStartLeft = new(transform.position.x - 0.2f, transform.position.y - 0.8f);
            Vector2 rayStartRight = new(transform.position.x + 0.2f, transform.position.y - 0.8f);

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

            if (isGrounded)
            {
                LandPosition(groundLeft, groundRight);
            }
            else if (onPlatform)
            {
                LandPosition(platformLeft, platformRight);
            }
            else { }
        }
        // Checking ceiling:
        else
        {
            Vector2 rayStartLeft = new(transform.position.x - 0.2f, transform.position.y + 0.2f);
            Vector2 rayStartRight = new(transform.position.x + 0.2f, transform.position.y + 0.2f);

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

    // Wall collision check
    private void CheckWalls()
    {
        Vector2 rayStartBottomLeft = new(transform.position.x - 0.2f, transform.position.y - 0.9f);
        Vector2 rayStartBottomRight = new(transform.position.x + 0.2f, transform.position.y - 0.9f);
        Vector2 rayStartTopLeft = new(transform.position.x - 0.2f, transform.position.y + 0.2f);
        Vector2 rayStartTopRight = new(transform.position.x + 0.2f, transform.position.y + 0.2f);

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
            Vector2 rayStartTopRight = new(
                transform.position.x + 0.2f,
                transform.position.y + 0.2f
            );
            Vector2 rayStartBottomRight = new(
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
            Vector2 rayStartBottomLeft = new(
                transform.position.x - 0.2f,
                transform.position.y - 0.9f
            );
            Vector2 rayStartTopLeft = new(transform.position.x - 0.2f, transform.position.y + 0.2f);

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

        Vector2 rayStartLeft = new(transform.position.x - 0.2f, transform.position.y - 0.8f);
        Vector2 rayStartRight = new(transform.position.x + 0.2f, transform.position.y - 0.8f);

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
            groundLeft.collider != null
            || groundRight.collider != null
            || platformLeft.collider != null
            || platformRight.collider != null
        );
    }

    private bool FutureCheckCeiling(Vector3 newPosition)
    {
        Vector2 rayStartLeft = new(transform.position.x - 0.2f, transform.position.y + 0.2f);
        Vector2 rayStartRight = new(transform.position.x + 0.2f, transform.position.y + 0.2f);

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

    // Helper function for calculating landing position
    private void LandPosition(RaycastHit2D hitLeft, RaycastHit2D hitRight)
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
    }

    // Platform falling timing sequence
    private IEnumerator PlatformFallDelay()
    {
        yield return new WaitForSeconds(0.3f);
        platformFalling = false;
    }
}
