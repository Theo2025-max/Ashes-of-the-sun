using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MotherFlamePickup : MonoBehaviour
{
    //[Header("Health Restore")]
    //[SerializeField] private int restoreAmount = 1;

    private void Awake()
    {
        // Ensure trigger collider
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();
        if (playerHealth == null) return;

        //playerHealth.RestoreHealthSmooth(restoreAmount);
        Destroy(gameObject);
    }
}