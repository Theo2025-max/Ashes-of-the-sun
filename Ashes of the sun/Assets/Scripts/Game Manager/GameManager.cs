using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform respawnPoint;

    [Header("Respawn")]
    [SerializeField] private float respawnDelay = 1f;

    [Header("UI")]
    [SerializeField] private PlayerHUD playerHUD;

    [Header("Camera")]
    [SerializeField] private CameraManager cameraManager;

    [HideInInspector] public PlayerController playerController;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void RespawnPlayer()
    {
        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        if (cameraManager != null)
            yield return StartCoroutine(
                cameraManager.LeadCameraToRespawn(respawnPoint.position)
            );

        yield return new WaitForSeconds(respawnDelay);

        GameObject newPlayer =
            Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity);

        playerController = newPlayer.GetComponent<PlayerController>();

        PlayerHealth health = newPlayer.GetComponent<PlayerHealth>();
        if (health != null)
            health.BindHUD(playerHUD);

        if (cameraManager != null)
            cameraManager.FollowPlayerWithoutSnap(newPlayer.transform);
    }
}