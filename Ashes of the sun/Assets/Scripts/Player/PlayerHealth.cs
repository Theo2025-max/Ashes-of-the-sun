using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    #region Health Settings

    [Header("Health Settings")]
    [SerializeField, Range(1, 10)] private int startingHealth = 5;

    private int currentHealth;
    private int maxHealth;
    private bool isDead;

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
    public GameManager game_manager;

    #endregion

    private void Awake()
    {
        maxHealth = startingHealth;
        currentHealth = startingHealth;

        playerController = GetComponent<PlayerController>();

        if (playerController == null)
            Debug.LogError("[PlayerHealth] PlayerController component is missing.", this);

        UpdateMotherFlamesUI();
    }

    private void Start()
    {
        if (game_manager == null) game_manager = GameManager.instance;
        if (game_manager == null) Debug.LogError("[PlayerHealth] GameManager reference is missing.", this);
    }

    private void OnEnable()
    {
        if (playerController != null) playerController.OnFlameShot += OnPlayerShoot;
    }

    private void OnDisable()
    {
        if (playerController != null) playerController.OnFlameShot -= OnPlayerShoot;
    }

    private void OnPlayerShoot() => TakeDamage(1);

    public void TakeDamage(int damageAmount)
    {
        if (isDead || currentHealth <= 0 || damageAmount <= 0) return;

        currentHealth = Mathf.Max(currentHealth - damageAmount, 0);

        UpdateMotherFlamesUI();

        if (currentHealth <= 0) Die();
    }

    public void Heal(int amount)
    {
        if (isDead || amount <= 0) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        UpdateMotherFlamesUI();
    }

    public void reset_health()
    {
        isDead = false;
        maxHealth = startingHealth;
        currentHealth = startingHealth;

        UpdateMotherFlamesUI();
    }

    public bool CanHeal() => !isDead && currentHealth < maxHealth;

    private void UpdateMotherFlamesUI()
    {
        if (motherFlames == null) return;

        for (int i = 0; i < motherFlames.Length; i++)
        {
            if (motherFlames[i] != null)
                motherFlames[i].gameObject.SetActive(i < currentHealth);
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;

        if (deathVFX != null) Instantiate(deathVFX, transform.position, Quaternion.identity);
        else Debug.LogWarning("[PlayerHealth] Death VFX reference is missing.", this);

        if (game_manager == null) game_manager = GameManager.instance;

        gameObject.SetActive(false);

        if (game_manager != null) game_manager.RespawnPlayer();
        else Debug.LogError("[PlayerHealth] Cannot respawn because GameManager is missing.", this);
    }
}