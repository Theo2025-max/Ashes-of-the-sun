using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private PlayerController playerController;


    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
    }


    public void FinishRespawn() => playerController.RespawnFinished(true);
}
