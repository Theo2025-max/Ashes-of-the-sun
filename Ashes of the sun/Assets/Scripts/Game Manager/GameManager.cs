using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private float respawnDelay = 1f;

    [SerializeField] private CameraManager cameraManager;

    [HideInInspector] public PlayerController playerController;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void RespawnPlayer()
    {
        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        //I Want to move camera to respawn position
        if (cameraManager != null)
            yield return StartCoroutine(cameraManager.LeadCameraToRespawn(respawnPoint.position));

        //I want to delay before spawning
        yield return new WaitForSeconds(respawnDelay);

        //I want spawn player at respawn point
        GameObject newPlayer = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity);
        playerController = newPlayer.GetComponent<PlayerController>();

        //I want to assign the camera to follow WITHOUT snap or memory
        if (cameraManager != null)
            cameraManager.FollowPlayerWithoutSnap(newPlayer.transform);

        //I want to notify systems
        PlayerEvents.PlayerSpawned(newPlayer.transform);
    }
}