using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    #endregion

    #region References
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private float respawnDelay = 1f;

    [SerializeField] private CameraManager cameraManager;

    [HideInInspector] public PlayerController playerController;
    #endregion

    #region Respawn Management
    public void UpdateRespawnPosition(Transform newRespawnPoint) => respawnPoint = newRespawnPoint;

    public void RespawnPlayer()
    {
        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        if (cameraManager != null)
            yield return StartCoroutine(cameraManager.LeadCameraToRespawn(respawnPoint.position));

        yield return new WaitForSeconds(respawnDelay);

        GameObject newPlayer = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity);
        playerController = newPlayer.GetComponent<PlayerController>();

        if (cameraManager != null)
            cameraManager.FollowPlayerWithoutSnap(newPlayer.transform);

        PlayerEvents.PlayerSpawned(newPlayer.transform);
    }
    #endregion
}