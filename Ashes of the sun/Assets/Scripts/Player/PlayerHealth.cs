using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField, Range(1, 10)] private int startingHealth = 5;
    private int currentHealth;

    [Header("UI")]
    [SerializeField] private Image[] motherFlames;

    private void Awake()
    {
        currentHealth = startingHealth;
        UpdateMotherFlamesUI();
    }

    private void OnEnable()
    {
        // Subscribe to shooting event
        PlayerController.OnFlameShot += OnPlayerShoot;
    }

    private void OnDisable()
    {
        PlayerController.OnFlameShot -= OnPlayerShoot;
    }

    // Called when the player shoots
    private void OnPlayerShoot()
    {
        TakeDamage(1); // Shooting costs 1 health
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(currentHealth, 0); // prevent negative health
        UpdateMotherFlamesUI();

        if (currentHealth <= 0)
            Die();
    }

    private void UpdateMotherFlamesUI()
    {
        for (int i = 0; i < motherFlames.Length; i++)
            motherFlames[i].gameObject.SetActive(i < currentHealth);
    }

    private void Die()
    {
        Debug.Log("Player has died!");
        gameObject.SetActive(false);
    }
}