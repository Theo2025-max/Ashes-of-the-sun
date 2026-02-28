using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField, Range(1, 10)] private int startingHealth = 5;

    private int currentHealth;
    private int maxHealth;

    [Header("VFX")]
    [SerializeField] private GameObject deathVFX;

    private Image[] motherFlames;
    private PlayerController playerController;

    private void Awake()
    {
        maxHealth = startingHealth;
        currentHealth = startingHealth;

        playerController = GetComponent<PlayerController>();
    }

    //CALLED BY GAME MANAGER
    public void BindHUD(PlayerHUD hud)
    {
        if (hud == null)
        {
            Debug.LogError("PlayerHUD is NULL");
            return;
        }

        motherFlames = hud.motherFlames;
        UpdateMotherFlamesUI();
    }

    private void OnEnable()
    {
        if (playerController != null)
            playerController.OnFlameShot += OnPlayerShoot;
    }

    private void OnDisable()
    {
        if (playerController != null)
            playerController.OnFlameShot -= OnPlayerShoot;
    }

    private void OnPlayerShoot()
    {
        TakeDamage(1);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        UpdateMotherFlamesUI();

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        UpdateMotherFlamesUI();
    }

    public bool CanHeal()
    {
        return currentHealth < maxHealth;
    }

    private void UpdateMotherFlamesUI()
    {
        if (motherFlames == null) return;

        for (int i = 0; i < motherFlames.Length; i++)
        {
            if (motherFlames[i] == null) continue;
            motherFlames[i].gameObject.SetActive(i < currentHealth);
        }
    }

    private void Die()
    {
        if (deathVFX != null)
            Instantiate(deathVFX, transform.position, Quaternion.identity);

        gameObject.SetActive(false);

        GameManager.instance.RespawnPlayer();
    }
}