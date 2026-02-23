using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FlameResource : MonoBehaviour
{
    [SerializeField] private float flameSpeed = 10f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifeTime = 3f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        spriteRenderer = GetComponent<SpriteRenderer>();

        Destroy(gameObject, lifeTime);
    }

    public void SetDirection(float direction)
    {
        // Movement
        rb.linearVelocity = new Vector2(direction * flameSpeed, 0f);

        // Visual flip (same concept as player facing)
        spriteRenderer.flipX = direction < 0f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponentInParent<Enemy>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}