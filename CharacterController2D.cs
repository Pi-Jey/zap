using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CharacterController2D : MonoBehaviour
{
    private Rigidbody2D rb;
    [Range(0, 0.3f)] public float MovementSmoothing = 0.1f;
    public bool AirControl = false;
    const float CheckRadius = 0.2f;
    private bool FacingRight = true;
    private Vector3 velocity = Vector3.zero;
    private bool canMove = true;

    public Transform GroundCheck;
    public LayerMask WhatIsGround;
    private bool isGrounded;

    public float JumpForce = 900f;
    private bool AbleToDJump = false;
    private bool canDoubleJump = true;
    private readonly float FallSpeedLimit = 20f;

    public GameObject DashButton;
    private bool AbleToDash = false;
    public float DashForce = 25f;
    private bool canDash = true;
    private bool isDashing = false;

    public Transform WallCheck;
    private bool isOnWall = false;
    private bool isWallSliding = false;
    private bool isRecentWallSlidding = false;
    private float WallJumpStart = 0;
    private float WallJumpDist = 0;
    private bool OnWallJumpVelocityLimit = false;
    private bool canCheck = false;

    public int currentlife = 3;
    public Image[] LifeImage;
    private bool canHitted = true;

    private Animator animator;

    [Header("Events")]
    [Space]

    public UnityEvent OnFallEvent;

    public UnityEvent OnLandEvent;

    public UnityEvent OnDeadEvent;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        OnFallEvent ??= new UnityEvent();
        OnLandEvent ??= new UnityEvent();
    }
    public void GettingAbleToDJump()
    {
        AbleToDJump = true;
    }
    public void GettingAbleToDash()
    {
        AbleToDash = true;
        if (Application.platform == RuntimePlatform.Android)
        {
            DashButton.SetActive(true);
        }
    }
    private void Update()
    {
        for (int i = 0; i < LifeImage.Length; i++)
        {
            LifeImage[i].enabled = i < currentlife;
        }
    }
    private void FixedUpdate()
    {
        bool wasGrounded = isGrounded;
        isGrounded = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(GroundCheck.position, CheckRadius, WhatIsGround);
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != gameObject)
            {
                isGrounded = true;
            }
            if (!wasGrounded)
            {
                OnLandEvent.Invoke();
                canDoubleJump = true;
                if (rb.velocity.y < 0f)
                {
                    OnWallJumpVelocityLimit = false;
                }
            }
        }

        isOnWall = false;

        if (!isGrounded)
        {
            OnFallEvent.Invoke();
            Collider2D[] collidersWall = Physics2D.OverlapCircleAll(WallCheck.position, CheckRadius, WhatIsGround);
            foreach (Collider2D collider in collidersWall)
            {
                if (collider.gameObject != null)
                {
                    isDashing = false;
                    isOnWall = true;
                }
            }
        }

        if (OnWallJumpVelocityLimit)
        {
            if (rb.velocity.y < -0.5f)
            {
                OnWallJumpVelocityLimit = false;
            }
            WallJumpDist = (WallJumpStart - transform.position.x) * transform.localScale.x;
            if (WallJumpDist < -0.5f && WallJumpDist > -1f)
            {
                canMove = true;
            }
            if (WallJumpDist < -1f && WallJumpDist >= -2f)
            {
                canMove = true;
                rb.velocity = new Vector2(10f * transform.localScale.x, rb.velocity.y);
            }
            else if (WallJumpDist < -2f || WallJumpDist > 0)
            {
                OnWallJumpVelocityLimit = false;
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }
    }


    public void Move(float move, bool jump, bool dash)
    {
        if (canMove)
        {
            if (AbleToDash && dash && canDash && !isWallSliding)
            {
                StartCoroutine(DashCooldown());
            }
            if (isDashing)
            {
                rb.velocity = new Vector2(transform.localScale.x * DashForce, 0);
            }
            else if (isGrounded || AirControl)
            {
                if (rb.velocity.y < -FallSpeedLimit)
                {
                    rb.velocity = new Vector2(rb.velocity.x, -FallSpeedLimit);
                }
                Vector3 targetVelocity = new Vector2(move * 10f, rb.velocity.y);
                rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, MovementSmoothing);
                if (move > 0 && !FacingRight && !isWallSliding)
                {
                    Flip();
                }
                else if (move < 0 && FacingRight && !isWallSliding)
                {
                    Flip();
                }
            }
            if (isGrounded && jump)
            {
                animator.SetBool("IsJumping", true);
                animator.SetBool("JumpUp", true);
                isGrounded = false;
                rb.AddForce(new Vector2(0f, JumpForce));
                canDoubleJump = true;

            }
            else if (AbleToDJump && !isGrounded && jump && canDoubleJump && !isWallSliding)
            {
                canDoubleJump = false;
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(new Vector2(0f, JumpForce / 1.2f));
                animator.SetBool("IsDoubleJumping", true);
            }

            else if (isOnWall && !isGrounded)
            {
                if (!isRecentWallSlidding && rb.velocity.y < 0 || isDashing)
                {
                    isWallSliding = true;
                    WallCheck.localPosition = new Vector3(-WallCheck.localPosition.x, WallCheck.localPosition.y, 0);
                    Flip();
                    StartCoroutine(WaitToCheck());
                    canDoubleJump = true;
                    animator.SetBool("IsWallSliding", true);
                }
                isDashing = false;

                if (isWallSliding)
                {
                    if (move * transform.localScale.x > 0.1f)
                    {
                        StartCoroutine(WaitToEndSliding());
                    }
                    else
                    {
                        isRecentWallSlidding = true;
                        rb.velocity = new Vector2(-transform.localScale.x * 2, -5);
                    }
                }

                if (jump && isWallSliding)
                {
                    animator.SetBool("IsJumping", true);
                    animator.SetBool("JumpUp", true);
                    rb.velocity = new Vector2(0f, 0f);
                    rb.AddForce(new Vector2(transform.localScale.x * JumpForce * 1.2f, JumpForce));
                    WallJumpStart = transform.position.x;
                    OnWallJumpVelocityLimit = true;
                    canDoubleJump = true;
                    isWallSliding = false;
                    animator.SetBool("IsWallSliding", false);
                    isRecentWallSlidding = false;
                    WallCheck.localPosition = new Vector3(Mathf.Abs(WallCheck.localPosition.x), WallCheck.localPosition.y, 0);
                    canMove = false;
                }
                else if (AbleToDash && dash && canDash)
                {
                    isWallSliding = false;
                    animator.SetBool("IsWallSliding", false);
                    isRecentWallSlidding = false;
                    WallCheck.localPosition = new Vector3(Mathf.Abs(WallCheck.localPosition.x), WallCheck.localPosition.y, 0);
                    canDoubleJump = true;
                    StartCoroutine(DashCooldown());
                }
            }
            else if (isWallSliding && !isOnWall && canCheck)
            {
                isWallSliding = false;
                animator.SetBool("IsWallSliding", false);
                isRecentWallSlidding = false;
                WallCheck.localPosition = new Vector3(Mathf.Abs(WallCheck.localPosition.x), WallCheck.localPosition.y, 0);
                canDoubleJump = true;
            }
        }
    }

    private void Flip()
    {
        FacingRight = !FacingRight;

        Vector3 _scale = transform.localScale;
        _scale.x *= -1;
        transform.localScale = _scale;
    }

    public void GetDamage(int damage, Vector3 position)
    {
        if (canHitted)
        {
            animator.SetBool("Hit", true);
            currentlife -= damage;
            Vector2 direction = Vector3.Normalize(transform.position - position);
            rb.velocity = Vector2.zero;
            rb.AddForce(direction * 400);
            if (currentlife <= 0)
            {
                StartCoroutine(PlayerDead());
            }
            else
            {
                StartCoroutine(DamageCooldown());
            }
        }
    }

    IEnumerator DashCooldown()
    {
        animator.SetBool("IsDashing", true);
        isDashing = true;
        canDash = false;
        yield return new WaitForSeconds(0.1f);
        isDashing = false;
        yield return new WaitForSeconds(0.6f);
        canDash = true;
    }
    IEnumerator DamageCooldown()
    {
        canMove = false;
        canHitted = false;
        yield return new WaitForSeconds(0.2f);
        canMove = true;
        yield return new WaitForSeconds(0.8f);
        canHitted = true;
    }

    IEnumerator WaitToCheck()
    {
        canCheck = false;
        yield return new WaitForSeconds(0.1f);
        canCheck = true;
    }

    IEnumerator WaitToEndSliding()
    {
        yield return new WaitForSeconds(0.1f);
        canDoubleJump = true;
        isWallSliding = false;
        animator.SetBool("IsWallSliding", false);
        isRecentWallSlidding = false;
        WallCheck.localPosition = new Vector3(Mathf.Abs(WallCheck.localPosition.x), WallCheck.localPosition.y, 0);
    }

    IEnumerator PlayerDead()
    {
        animator.SetBool("IsDead", true);
        canMove = false;
        canHitted = false;
        GetComponent<Attack>().enabled = false;
        yield return new WaitForSeconds(0.5f);
        OnDeadEvent.Invoke();
    }
}
