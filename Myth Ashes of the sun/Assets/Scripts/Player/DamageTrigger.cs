using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>(); 

        if (playerController != null)
        {
            playerController.knockback(transform.position.x);
        }

        if (playerHealth != null) 
        {
            playerHealth.TakeDamage(1);
        }
    }
}