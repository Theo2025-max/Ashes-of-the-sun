using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Existing scene references preserved for Inspector compatibility.
    public GameObject player;
    public PlayerHealth health;
    public PlayerController controller;

    #region Singleton

    public static GameManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    #endregion

    #region References

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private float respawnDelay = 1f;
    [SerializeField] private CameraManager cameraManager;

    [HideInInspector] public PlayerController playerController;

    private bool isRespawning;

    #endregion

    private void Start()
    {
        CachePlayerReferences();

        if (player == null) Debug.LogError("[GameManager] Player reference is missing.", this);
        if (respawnPoint == null) Debug.LogError("[GameManager] Respawn Point reference is missing.", this);
        if (health == null) Debug.LogError("[GameManager] PlayerHealth reference is missing.", this);
        if (controller == null) Debug.LogError("[GameManager] PlayerController reference is missing.", this);
    }

    #region Respawn Management

    public void UpdateRespawnPosition(Transform newRespawnPoint)
    {
        if (newRespawnPoint == null)
        {
            Debug.LogError("[GameManager] Cannot update the respawn position because the supplied Transform is null.", this);
            return;
        }

        respawnPoint = newRespawnPoint;
    }

    public void RespawnPlayer()
    {
        if (isRespawning) return;

        CachePlayerReferences();

        if (player == null || respawnPoint == null || health == null || controller == null)
        {
            Debug.LogError("[GameManager] Respawn requires Player, Respawn Point, PlayerHealth, and PlayerController references.", this);
            return;
        }

        isRespawning = true;
        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        controller.RespawnFinished(false);
        controller.force_reset_knockback();
        player.SetActive(false);

        if (cameraManager != null)
            yield return StartCoroutine(cameraManager.LeadCameraToRespawn(respawnPoint.position));

        yield return new WaitForSeconds(Mathf.Max(0f, respawnDelay));

        player.transform.position = respawnPoint.position;
        health.reset_health();

        player.SetActive(true);

        CachePlayerReferences();
        controller.force_reset_knockback();

        if (cameraManager != null) cameraManager.FollowPlayerWithoutSnap(player.transform);

        PlayerEvents.PlayerSpawned(player.transform);
        isRespawning = false;
    }

    private void CachePlayerReferences()
    {
        if (player == null && controller != null) player = controller.gameObject;
        if (player == null) return;

        if (controller == null) controller = player.GetComponent<PlayerController>();
        if (health == null) health = player.GetComponent<PlayerHealth>();

        playerController = controller;
    }

    #endregion
}