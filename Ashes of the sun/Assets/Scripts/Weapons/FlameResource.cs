using UnityEngine;

public class FlameResource : MonoBehaviour
{
    public int flameSpeed;
    Rigidbody2D rb;
    GameObject player;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player");

        if (player.transform.localEulerAngles.y == 180)
        {
            flameSpeed = flameSpeed * -1;
        }
    }

    private void Update()
    {
        rb.linearVelocity = new Vector2 (flameSpeed, 0);
    }
}
