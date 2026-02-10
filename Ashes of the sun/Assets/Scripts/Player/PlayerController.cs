using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;

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
    [SerializeField] private Transform fireCastPoint;
    [SerializeField] private GameObject flameProjectile;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        HandleInput();
        HandleGroundCheck();
        AttemptBufferJump();
        ShootFlame();
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
            
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
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
        isGrounded = Physics2D.Raycast(transform.position,Vector2.down,groundCheckDistance,whatIsGround);

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
            Vector2 shootDirection = facingRight ? Vector2.right : Vector2.left;

            GameObject flame = Instantiate(flameProjectile, fireCastPoint.position, Quaternion.identity);

            flame.GetComponent<FlameResource>().Initialize(shootDirection);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position,new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
    }
}
