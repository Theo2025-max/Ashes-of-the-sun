using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Components

    private Rigidbody2D rb;
    private Animator anim;
    private CapsuleCollider2D cd;

    private Vector2 moveInput;

    #endregion

    #region Movement Settings

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;

    private bool canDoubleJump;

    #endregion

    #region Buffer & Coyote Jump

    [Header("Buffer & Coyote Jump")]
    [SerializeField] private float bufferJumpWindow = .25f;
    private float bufferJumpActivated = -1;

    [SerializeField] private float coyoteJumpWindow = .5f;
    private float coyoteJumpActivated = -1;

    #endregion

    #region Wall Interaction

    [Header("Wall Interaction")]
    [SerializeField] private float wallJumpDuration = .6f;
    [SerializeField] private Vector2 wallJumpForce;

    private bool isWallJumping;

    [Header("knockback")]
    [SerializeField] private float knockbackDuration = 1f;
    [SerializeField] private Vector2 knockbackPower;
    private bool isKnocked;
    private bool canBeKnocked =true;



    #endregion

    #region Collision

    [Header("Collision")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private LayerMask whatIsGround;

    private bool isGrounded;
    private bool isAirborne;
    private bool isWallDetected;

    #endregion

    #region Facing

    private bool facingRight = true;
    private int facingDir = 1;

    #endregion

    #region Flame Casting

    [Header("Flame Casting Settings")]
    public GameObject flameProjectile;
    public Transform flamePosition;

    public event Action OnFlameShot;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CapsuleCollider2D>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        // Handle input
        moveInput.x = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            JumpButton();
            RequestBufferJump();
        }

      
        // Movement & physics
        UpdateAirborneStatus();

        if(isKnocked) return;

        ShootFlame();
        HandleWallSlide();
        HandleMovement();
        HandleFlip();
        HandleCollision();
        HandleAnimations();
    }


    public void knockback()
    {
        if (!canBeKnocked) return;

        StartCoroutine(knockbackRoutine());
        anim.SetTrigger("knockback");
        rb.linearVelocity = new Vector2(knockbackPower.x * -facingDir, knockbackPower.y);
    }
    #endregion

    #region Movement Logic

    private void HandleMovement()
    {
        if (isWallDetected || isWallJumping)
            return;

        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }

    private void JumpButton()
    {
        bool coyoteJumpAvailable = Time.time < coyoteJumpActivated + coyoteJumpWindow;

        if (isGrounded || coyoteJumpAvailable)
        {
            Jump();
        }
        else if (isWallDetected && !isGrounded)
        {
            WallJump();
        }
        else if (canDoubleJump)
        {
            DoubleJump();
        }

        CancelCoyoteJump();
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    private void DoubleJump()
    {
        StopCoroutine(WallJumpRoutine());
        isWallJumping = false;
        canDoubleJump = false;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, doubleJumpForce);
    }

    private void WallJump()
    {
        canDoubleJump = true;

        rb.linearVelocity = new Vector2(wallJumpForce.x * -facingDir, wallJumpForce.y);

        Flip();

        StopAllCoroutines();
        StartCoroutine(WallJumpRoutine());
    }

    private IEnumerator WallJumpRoutine()
    {
        isWallJumping = true;
        yield return new WaitForSeconds(wallJumpDuration);
        isWallJumping = false;
    }

    private void HandleWallSlide()
    {
        bool canWallSlide = isWallDetected && rb.linearVelocity.y < 0;
        float yModifier = moveInput.y < 0 ? 1f : .05f;

        if (!canWallSlide)
            return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * yModifier);
    }

    #endregion

    #region Airborne Logic

    private void UpdateAirborneStatus()
    {
        if (isGrounded && isAirborne)
            HandleLanding();

        if (!isGrounded && !isAirborne)
            BecomeAirborne();
    }

    private void BecomeAirborne()
    {
        isAirborne = true;

        if (rb.linearVelocity.y < 0)
            ActivateCoyoteJump();
    }

    private void HandleLanding()
    {
        isAirborne = false;
        canDoubleJump = true;

        AttemptBufferJump();
    }

    #endregion

    #region Buffer & Coyote

    private void RequestBufferJump()
    {
        if (isAirborne)
            bufferJumpActivated = Time.time;
    }

    private void AttemptBufferJump()
    {
        if (Time.time < bufferJumpActivated + bufferJumpWindow)
        {
            bufferJumpActivated = Time.time - 1;
            Jump();
        }
    }

    private void ActivateCoyoteJump() => coyoteJumpActivated = Time.time;
    private void CancelCoyoteJump() => coyoteJumpActivated = Time.time - 1;

    #endregion

    #region Collision & Animation


    private IEnumerator knockbackRoutine()
    {
        canBeKnocked = false;
        isKnocked = true;

        yield return new WaitForSeconds(knockbackDuration);

        canBeKnocked = true;
        isKnocked = false;
    }

    public void Die() => Destroy(gameObject);

    private void HandleCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        isWallDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
    }

    private void HandleAnimations()
    {
        if (anim == null)
            return;

        anim.SetFloat("xVelocity", rb.linearVelocity.x);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isWallDetected", isWallDetected);
    }

    #endregion

    #region Facing

    private void HandleFlip()
    {
        if (moveInput.x < 0 && facingRight || moveInput.x > 0 && !facingRight)
            Flip();
    }

    private void Flip()
    {
        facingDir *= -1;
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
    }

    #endregion

    #region Flame Logic

    private void ShootFlame()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject flame = Instantiate(flameProjectile, flamePosition.position, Quaternion.identity);

            float direction = facingRight ? 1f : -1f;

            FlameResource flameResource = flame.GetComponent<FlameResource>();
            if (flameResource != null)
                flameResource.SetDirection(direction);

            OnFlameShot?.Invoke();
        }
    }

    #endregion

    #region Gizmos

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green; 

        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));

        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (wallCheckDistance * facingDir), transform.position.y));
    }

    #endregion
}