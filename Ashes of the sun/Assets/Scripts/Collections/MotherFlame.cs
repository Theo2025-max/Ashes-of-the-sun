using UnityEngine;

public class MotherFlame : MonoBehaviour
{
    [SerializeField] private GameObject pickupVfx;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
        if (playerHealth == null) return;

        if (!playerHealth.CanHeal()) return;

        playerHealth.Heal(1);

        if (pickupVfx != null)
            Instantiate(pickupVfx, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}