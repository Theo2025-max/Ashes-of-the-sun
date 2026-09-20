using System;
using System.Collections;
using FirstGearGames.SmoothCameraShaker;
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

    private bool canBeControlled;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;

    private float defaultGravityScale;
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
    private Coroutine wallJumpRoutine;

    [Header("knockback")]
    [SerializeField] private float knockbackDuration = 1f;
    [SerializeField] private Vector2 knockbackPower;

    private bool isKnocked;
    private bool canBeKnocked = true;
    private Coroutine knockbackCoroutine;

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

    #region Flame Casting & VFX

    [Header("Flame Casting Settings")]
    public GameObject flameProjectile;
    public Transform flamePosition;

    public event Action OnFlameShot;

    [Header("VFX")]
    public GameObject deathVFX;

    public ShakeData shoot_shake;
    public ShakeData damage_shake;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CapsuleCollider2D>();
        anim = GetComponentInChildren<Animator>();

        if (rb == null) Debug.LogError("[PlayerController] Rigidbody2D component is missing.", this);
        if (cd == null) Debug.LogError("[PlayerController] CapsuleCollider2D component is missing.", this);
        if (anim == null) Debug.LogError("[PlayerController] Animator component is missing from the Player hierarchy.", this);
        if (flameProjectile == null) Debug.LogError("[PlayerController] Flame Projectile reference is missing.", this);
        if (flamePosition == null) Debug.LogError("[PlayerController] Flame Position reference is missing.", this);
    }

    private void Start()
    {
        if (rb == null || cd == null) return;

        defaultGravityScale = rb.gravityScale;
        RespawnFinished(false);
    }

    private void Update()
    {
        if (rb == null || cd == null || anim == null) return;

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        UpdateAirborneStatus();

        if (!canBeControlled || isKnocked)
        {
            HandleCollision();
            HandleAnimations();
            return;
        }

        if (Input.GetButtonDown("Jump"))
        {
            JumpButton();
            RequestBufferJump();
        }

        ShootFlame();
        if (!canBeControlled || !gameObject.activeInHierarchy) return;

        HandleWallSlide();
        HandleMovement();
        HandleFlip();
        HandleCollision();
        HandleAnimations();
    }

    public void RespawnFinished(bool finished)
    {
        if (rb == null || cd == null) return;

        if (!finished)
        {
            ResetTransientMovementState();
            rb.linearVelocity = Vector2.zero;
        }

        rb.gravityScale = finished ? defaultGravityScale : 0f;
        canBeControlled = finished;
        cd.enabled = finished;
    }

    public void knockback(float sourceDamageXPosition)
    {
        if (!canBeKnocked || rb == null || anim == null) return;

        canBeKnocked = false;
        isKnocked = true;

        if (damage_shake != null) CameraShakerHandler.Shake(damage_shake);

        float knockbackDir = transform.position.x < sourceDamageXPosition ? -1f : 1f;

        if (knockbackCoroutine != null) StopCoroutine(knockbackCoroutine);

        knockbackCoroutine = StartCoroutine(knockbackRoutine());

        anim.SetTrigger("knockback");
        rb.linearVelocity = new Vector2(knockbackPower.x * knockbackDir, knockbackPower.y);
    }

    public void force_reset_knockback()
    {
        if (knockbackCoroutine != null)
        {
            StopCoroutine(knockbackCoroutine);
            knockbackCoroutine = null;
        }

        canBeKnocked = true;
        isKnocked = false;
    }

    #endregion

    #region Movement Logic

    private void HandleMovement()
    {
        if (isWallDetected || isWallJumping) return;

        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }

    private void JumpButton()
    {
        bool coyoteJumpAvailable = Time.time < coyoteJumpActivated + coyoteJumpWindow;

        if (isGrounded || coyoteJumpAvailable) Jump();
        else if (isWallDetected && !isGrounded) WallJump();
        else if (canDoubleJump) DoubleJump();

        CancelCoyoteJump();
    }

    private void Jump() => rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

    private void DoubleJump()
    {
        if (wallJumpRoutine != null)
        {
            StopCoroutine(wallJumpRoutine);
            wallJumpRoutine = null;
        }

        isWallJumping = false;
        canDoubleJump = false;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, doubleJumpForce);
    }

    private void WallJump()
    {
        canDoubleJump = true;

        rb.linearVelocity = new Vector2(wallJumpForce.x * -facingDir, wallJumpForce.y);

        Flip();

        if (wallJumpRoutine != null) StopCoroutine(wallJumpRoutine);

        wallJumpRoutine = StartCoroutine(WallJumpRoutine());
    }

    private IEnumerator WallJumpRoutine()
    {
        isWallJumping = true;

        yield return new WaitForSeconds(Mathf.Max(0f, wallJumpDuration));

        isWallJumping = false;
        wallJumpRoutine = null;
    }

    private void HandleWallSlide()
    {
        bool canWallSlide = isWallDetected && rb.linearVelocity.y < 0;
        float yModifier = moveInput.y < 0 ? 1f : .05f;

        if (!canWallSlide) return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * yModifier);
    }

    #endregion

    #region Airborne Logic

    private void UpdateAirborneStatus()
    {
        if (isGrounded && isAirborne) HandleLanding();
        if (!isGrounded && !isAirborne) BecomeAirborne();
    }

    private void BecomeAirborne()
    {
        isAirborne = true;

        if (rb.linearVelocity.y < 0) ActivateCoyoteJump();
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
        if (isAirborne) bufferJumpActivated = Time.time;
    }

    private void AttemptBufferJump()
    {
        if (Time.time >= bufferJumpActivated + bufferJumpWindow) return;

        bufferJumpActivated = Time.time - 1;
        Jump();
    }

    private void ActivateCoyoteJump() => coyoteJumpActivated = Time.time;
    private void CancelCoyoteJump() => coyoteJumpActivated = Time.time - 1;

    #endregion

    #region Collision & Animation

    private IEnumerator knockbackRoutine()
    {
        yield return new WaitForSeconds(Mathf.Max(0f, knockbackDuration));

        canBeKnocked = true;
        isKnocked = false;
        knockbackCoroutine = null;
    }

    public void Die()
    {
        if (deathVFX != null) Instantiate(deathVFX, transform.position, Quaternion.identity);
        else Debug.LogWarning("[PlayerController] Death VFX reference is missing.", this);
    }

    private void HandleCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        isWallDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
    }

    private void HandleAnimations()
    {
        anim.SetFloat("xVelocity", rb.linearVelocity.x);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isWallDetected", isWallDetected);
    }

    private void ResetTransientMovementState()
    {
        if (wallJumpRoutine != null)
        {
            StopCoroutine(wallJumpRoutine);
            wallJumpRoutine = null;
        }

        force_reset_knockback();

        isWallJumping = false;
    }

    #endregion

    #region Facing

    private void HandleFlip()
    {
        if (moveInput.x < 0 && facingRight || moveInput.x > 0 && !facingRight) Flip();
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
        if (!Input.GetMouseButtonDown(0)) return;

        if (flameProjectile == null || flamePosition == null)
        {
            Debug.LogError("[PlayerController] Cannot shoot because Flame Projectile or Flame Position is missing.", this);
            return;
        }

        GameObject flame = Instantiate(flameProjectile, flamePosition.position, Quaternion.identity);

        if (shoot_shake != null) CameraShakerHandler.Shake(shoot_shake);

        float direction = facingRight ? 1f : -1f;

        FlameResource flameResource = flame.GetComponent<FlameResource>();

        if (flameResource != null) flameResource.SetDirection(direction);
        else Debug.LogWarning("[PlayerController] Spawned flame does not contain a FlameResource component.", flame);

        OnFlameShot?.Invoke();
    }

    #endregion

    #region Gizmos

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawLine(transform.position,new Vector2(transform.position.x, transform.position.y - groundCheckDistance));

        Gizmos.DrawLine(transform.position,new Vector2(transform.position.x + wallCheckDistance * facingDir, transform.position.y));
    }

    #endregion
}