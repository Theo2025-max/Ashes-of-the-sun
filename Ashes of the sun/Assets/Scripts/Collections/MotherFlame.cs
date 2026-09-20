using UnityEngine;

public class MotherFlame : MonoBehaviour
{
    [SerializeField] private GameObject pickupVfx;

    private bool collected;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collected) return;

        PlayerHealth playerHealth =
            collision.GetComponentInParent<PlayerHealth>();

        if (playerHealth == null || !playerHealth.CanHeal()) return;

        collected = true;

        playerHealth.Heal(1);

        if (pickupVfx != null)
            Instantiate(pickupVfx, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}