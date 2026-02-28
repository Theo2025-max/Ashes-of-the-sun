using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Animator anim;
    private bool canBeActivated;
    private bool active;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController controller = GetComponent<PlayerController>();
    }

    private void ActivateCheckpoint()
    {

    }
}
