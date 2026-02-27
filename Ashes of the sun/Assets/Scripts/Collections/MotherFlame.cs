using UnityEngine;

public class MotherFlame : MonoBehaviour
{
    [SerializeField] private GameObject pickupVfx;

    private GameManager gameManager;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        gameManager = GameManager.instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

        if (playerHealth == null)
            return;

        // Check if player actually needs healing
        if (!playerHealth.CanHeal())
            return;

        // Heal player
        playerHealth.Heal(1);

        // Spawn VFX
        if (pickupVfx != null)
            Instantiate(pickupVfx, transform.position, Quaternion.identity);

        // Destroy flame only if heal happened
        Destroy(gameObject);
    }
}