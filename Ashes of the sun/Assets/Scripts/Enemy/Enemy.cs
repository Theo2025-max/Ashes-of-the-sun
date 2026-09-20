using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected SpriteRenderer sr;
    protected Animator anim;
    protected Rigidbody2D rb;
    protected Collider2D[] colliders;

    [Header("Movement")]
    [SerializeField] public float moveSpeed = 2f;
    protected bool canMove = true;
    protected bool facingRight = true;
    protected int facingDir = 1;

    [Header("Collision")]
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected float groundCheckDistance = 1f;
    [SerializeField] protected float wallCheckDistance = .7f;
    [SerializeField] protected Transform groundCheck;

    protected bool isGrounded;
    protected bool isWallDetected;

    [Header("Health")]
    [SerializeField] protected int maxHealth = 20;
    protected int currentHealth = 2;

    [Header("Damage")]
    [SerializeField] private int damageAmount = 1;
    public int DamageAmount => damageAmount;

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

        if (sr == null) Debug.LogError("[Enemy] SpriteRenderer component is missing.", this);
        if (anim == null) Debug.LogError("[Enemy] Animator component is missing.", this);
        if (rb == null) Debug.LogError("[Enemy] Rigidbody2D component is missing.", this);
    }

    protected virtual void Update()
    {
        if (isDead)
        {
            HandleDeathRotation();
            return;
        }

        HandleCollision();
        HandleAnimator();
    }

    protected virtual void HandleAnimator() { }

    protected virtual void HandleCollision()
    {
        if (groundCheck != null)
            isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);

        isWallDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
    }

    protected virtual void HandleFlip(float targetX)
    {
        if (targetX < transform.position.x && facingRight ||
            targetX > transform.position.x && !facingRight)
        {
            Flip();
        }
    }

    protected virtual void Flip()
    {
        facingDir *= -1;
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
    }

    public virtual void TakeDamage(int damage)
    {
        if (isDead || damage <= 0) return;

        currentHealth -= damage;

        if (anim != null) anim.SetTrigger("hit");
        if (currentHealth <= 0) Die();
    }

    public virtual void Die()
    {
        if (isDead) return;

        isDead = true;
        canMove = false;

        EnableColliders(false);

        if (rb != null)
        {
            if (rb.bodyType == RigidbodyType2D.Kinematic)
                rb.bodyType = RigidbodyType2D.Dynamic;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, deathImpactSpeed);
        }

        deathRotationDirection = Random.value < .5f ? -1 : 1;

        Destroy(gameObject, 10f);
    }

    protected void EnableColliders(bool enable)
    {
        if (colliders == null) return;

        foreach (Collider2D enemyCollider in colliders)
        {
            if (enemyCollider != null) enemyCollider.enabled = enable;
        }
    }

    private void HandleDeathRotation()
    {
        transform.Rotate(0, 0, deathRotationSpeed * deathRotationDirection * Time.deltaTime);
    }

    protected virtual void OnDrawGizmos()
    {
        if (groundCheck != null)
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);

        Gizmos.DrawLine(transform.position,transform.position + Vector3.right * facingDir * wallCheckDistance);
    }
}