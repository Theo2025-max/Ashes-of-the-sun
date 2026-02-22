using UnityEngine;

public class Enemy : MonoBehaviour
{
    // COMPONENTS
    protected SpriteRenderer sr;
    protected Animator anim;
    protected Rigidbody2D rb;
    protected Collider2D[] colliders;

    // MOVEMENT
    [Header("Movement")]
    [SerializeField] protected float moveSpeed = 2f;
    protected bool canMove = true;
    protected bool facingRight = true;
    protected int facingDir = 1;

    // COLLISION
    [Header("Collision")]
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected float groundCheckDistance = 1f;
    [SerializeField] protected float wallCheckDistance = 0.7f;
    [SerializeField] protected Transform groundCheck;
    protected bool isGrounded;
    protected bool isWallDetected;

    // HEALTH
    [Header("Health")]
    [SerializeField] protected int maxHealth = 20;
    protected int currentHealth =2;

    // DEATH
    [Header("Death")]
    [SerializeField] protected float deathImpactSpeed = 5f;
    [SerializeField] protected float deathRotationSpeed = 150f;
    protected bool isDead;
    protected int deathRotationDirection = 1;

    protected virtual void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        colliders = GetComponentsInChildren<Collider2D>();

        currentHealth = maxHealth;
    }

    protected virtual void Update()
    {
        if (isDead)
        HandleDeathRotation();

        HandleCollision();
        HandleAnimator();
    }

    protected virtual void HandleAnimator()
    {
       // anim.SetFloat("xVelocity", rb.linearVelocity.x);
    }

    protected virtual void HandleCollision()
    {
        if (groundCheck)
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);

        isWallDetected = Physics2D.Raycast( transform.position, Vector2.right * facingDir,wallCheckDistance, whatIsGround);
    }

    protected virtual void HandleFlip(float targetX)
    {
        float myX = transform.position.x;
        if ((targetX < myX) ^ facingRight)
            Flip();
    }

    protected virtual void Flip()
    {
        facingDir *= -1;
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
    }

    // ======================
    // DAMAGE & HEALTH
    // ======================
    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        anim.SetTrigger("hit");

        if (currentHealth <= 0)
            Die();
    }

    // ======================
    // DEATH LOGIC
    // ======================
    public virtual void Die()
    {
        if (isDead) return;

        if (rb.bodyType == RigidbodyType2D.Kinematic)
            rb.bodyType = RigidbodyType2D.Dynamic;

        EnableColliders(false);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, deathImpactSpeed);

        deathRotationDirection = Random.value < 0.5f ? -1 : 1;
        isDead = true;

        Destroy(gameObject, 10f);
    }

    protected void EnableColliders(bool enable)
    {
        foreach (var c in colliders)
            c.enabled = enable;
    }

    private void HandleDeathRotation()
    {
        transform.Rotate(0,0, deathRotationSpeed * deathRotationDirection * Time.deltaTime);
    }

    protected virtual void OnDrawGizmos()
    {
        if (groundCheck)
            Gizmos.DrawLine(groundCheck.position,groundCheck.position + Vector3.down * groundCheckDistance);

        Gizmos.DrawLine(transform.position,transform.position + Vector3.right * facingDir * wallCheckDistance);
    }
}