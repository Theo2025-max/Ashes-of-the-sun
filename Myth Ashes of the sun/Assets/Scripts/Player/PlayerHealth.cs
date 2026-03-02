using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    #region Health Settings
    [Header("Health Settings")]
    [SerializeField, Range(1, 10)] private int startingHealth = 5;
    private int currentHealth;
    private int maxHealth;
    #endregion

    #region VFX
    [Header("VFX")]
    public GameObject deathVFX;
    #endregion

    #region UI
    [Header("UI")]
    [SerializeField] private Image[] motherFlames;
    #endregion

    #region References
    private PlayerController playerController;
    #endregion

    #region Unity Callbacks

    //ROY STUFF
    public GameManager game_manager;
    private void Awake()
    {
        maxHealth = startingHealth;
        currentHealth = startingHealth;
        UpdateMotherFlamesUI();

        playerController = GetComponent<PlayerController>();
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
    #endregion

    #region Player Actions
    private void OnPlayerShoot()
    {
        TakeDamage(1);
    }

    public void TakeDamage(int damageAmount)
    {
        if(currentHealth > 0)
        {
            currentHealth -= damageAmount;
            currentHealth = Mathf.Max(currentHealth, 0);

            UpdateMotherFlamesUI();

            if (currentHealth <= 0)
                Die();
        }

    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        UpdateMotherFlamesUI();
    }

    public void reset_health()
    {
        maxHealth = startingHealth;
        currentHealth = startingHealth;
        UpdateMotherFlamesUI();
    }

    public bool CanHeal()
    {
        return currentHealth < maxHealth;
    }
    #endregion

    #region UI Updates
    private void UpdateMotherFlamesUI()
    {
        for (int i = 0; i < motherFlames.Length; i++)
            motherFlames[i].gameObject.SetActive(i < currentHealth);
    }
    #endregion

    #region Death
    private void Die()
    {
        Instantiate(deathVFX, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
        game_manager.RespawnPlayer();
    }
    #endregion
}