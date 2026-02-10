using UnityEngine;

public class FlameResource : MonoBehaviour
{
    [Header("Shooting Settings")]
    [SerializeField] private float flameProjectileSpeed = 10f;
    [SerializeField] private GameObject impactEffect;

    private Rigidbody2D rb;

    private Vector2 moveDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector2 direction)
    {
        moveDirection = direction.normalized;
        rb.linearVelocity = moveDirection * flameProjectileSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // TEMPORARILY DISABLED:
        // Instantiate(impactEffect, transform.position, Quaternion.identity);
        // Destroy(gameObject);
    }

}
