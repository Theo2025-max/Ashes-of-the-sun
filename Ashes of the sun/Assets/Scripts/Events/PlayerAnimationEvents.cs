using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();

        if (playerController == null)
            Debug.LogError("[PlayerAnimationEvents] No PlayerController was found in the parent hierarchy.", this);
    }

    public void FinishRespawn()
    {
        if (playerController != null) playerController.RespawnFinished(true);
    }
}