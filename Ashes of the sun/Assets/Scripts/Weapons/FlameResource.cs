using UnityEngine;

public class FlameResource : MonoBehaviour
{
    public float flameSpeed = 10f;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetDirection(float direction)
    {
        rb.linearVelocity = new Vector2(direction * flameSpeed, 0f);
    }
}
