using UnityEngine;

public class MotherFlame : MonoBehaviour
{
    #region VFX
    [SerializeField] private GameObject pickupVfx;
    #endregion

    #region References
    private GameManager gameManager;
    private Animator anim;
    public AudioSource pickup;
    #endregion

    #region Unity Callbacks
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
        if (playerHealth == null) return;

        if (!playerHealth.CanHeal()) return;

        playerHealth.Heal(1);
        pickup.Play();

        if (pickupVfx != null)
            Instantiate(pickupVfx, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
    #endregion
}