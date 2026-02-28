using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    [Header("Cinemachine Settings")]
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private float panDuration = 0.5f;

    private bool isMoving = false;

    public IEnumerator LeadCameraToRespawn(Vector3 respawnPosition)
    {
        if (cinemachineCamera == null || isMoving)
            yield break;

        isMoving = true;

        // I want to detach follow
        cinemachineCamera.Follow = null;
        cinemachineCamera.LookAt = null;

        Vector3 startPos = cinemachineCamera.transform.position;
        Vector3 endPos = respawnPosition + new Vector3(0, 0, -10f);

        float elapsed = 0f;

        while (elapsed < panDuration)
        {
            cinemachineCamera.transform.position =
                Vector3.Lerp(startPos, endPos, elapsed / panDuration);

            elapsed += Time.deltaTime;
            yield return null;
        }

        cinemachineCamera.transform.position = endPos;
        isMoving = false;
    }

    public void FollowPlayerWithoutSnap(Transform playerTransform)
    {
        if (cinemachineCamera == null || playerTransform == null)
            return;

        // I want to assign Follow first
        cinemachineCamera.Follow = playerTransform;
        cinemachineCamera.LookAt = playerTransform;

        //CRITICAL: I need you to reset Cinemachine internal state 
        cinemachineCamera.PreviousStateIsValid = false;
    }
}