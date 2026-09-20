using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class FlameResource : MonoBehaviour
{
    [SerializeField] private float flameSpeed = 10f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifeTime = 3f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private bool hasHit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        rb.gravityScale = 0f;

        Destroy(gameObject, Mathf.Max(0f, lifeTime));
    }

    public void SetDirection(float direction)
    {
        rb.linearVelocity = new Vector2(direction * flameSpeed, 0f);

        if (spriteRenderer != null) spriteRenderer.flipX = direction < 0f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;

        Enemy enemy = collision.GetComponentInParent<Enemy>();

        if (enemy == null) return;

        hasHit = true;

        enemy.TakeDamage(damage);

        Destroy(gameObject);
    }
}