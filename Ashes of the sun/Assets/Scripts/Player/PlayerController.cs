using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static event System.Action OnFlameShot; // Event fired whenever player shoots

    private Rigidbody2D rb;
    private Animator anim;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float doubleJumpForce = 8f;
    private bool canDoubleJump;

    [Header("Coyote & Buffer Jump")]
    [SerializeField] private float coyoteJumpWindow = 0.2f;
    [SerializeField] private float bufferJumpWindow = 0.25f;
    private float coyoteJumpActivated = -1f;
    private float bufferJumpActivated = -1f;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private LayerMask whatIsGround;
    private bool isGrounded;

    private float xInput;
    private bool facingRight = true;

    [Header("Flame Casting Settings")]
    public GameObject flameProjectile;
    public Transform flamePosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        HandleAnimations();
        HandleInput();
        HandleGroundCheck();
        AttemptBufferJump();
        ShootFlame();
    }

    private void HandleAnimations()
    {
        anim.SetFloat("xVelocity", rb.linearVelocity.x);
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            RequestBufferJump();
            JumpButton();
        }
    }

    #region Movement
    private void HandleMovement()
    {
        rb.linearVelocity = new Vector2(xInput * moveSpeed, rb.linearVelocity.y);
        HandleFlip();
    }

    private void HandleFlip()
    {
        if ((xInput < 0 && facingRight) || (xInput > 0 && !facingRight))
        {
            facingRight = !facingRight;
            transform.Rotate(0f, 180f, 0f);
        }
    }
    #endregion

    #region Jump Logic
    private void JumpButton()
    {
        bool coyoteAvailable = Time.time < coyoteJumpActivated + coyoteJumpWindow;

        if (isGrounded || coyoteAvailable)
        {
            Jump();
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
        canDoubleJump = true;
    }

    private void DoubleJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, doubleJumpForce);
        canDoubleJump = false;
    }

    private void RequestBufferJump()
    {
        if (!isGrounded)
            bufferJumpActivated = Time.time;
    }

    private void AttemptBufferJump()
    {
        if (Time.time < bufferJumpActivated + bufferJumpWindow && isGrounded)
        {
            bufferJumpActivated = -1f;
            Jump();
        }
    }

    private void ActivateCoyoteJump() => coyoteJumpActivated = Time.time;
    private void CancelCoyoteJump() => coyoteJumpActivated = -1f;
    #endregion

    private void HandleGroundCheck()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);

        if (isGrounded)
        {
            canDoubleJump = true;
            ActivateCoyoteJump();
        }
    }

    private void ShootFlame()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Spawn flame
            GameObject flame = Instantiate(flameProjectile, flamePosition.position, Quaternion.identity);
            float direction = facingRight ? 1f : -1f;
            flame.GetComponent<FlameResource>().SetDirection(direction);

            // Reduce player health
            OnFlameShot?.Invoke();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
    }
}