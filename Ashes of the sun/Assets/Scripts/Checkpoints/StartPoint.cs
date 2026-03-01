using UnityEngine;

public class StartPoint : MonoBehaviour
{
    private Animator anim => GetComponent<Animator>();

    private void OnTriggerExit2D(Collider2D collision)
    
    {
        PlayerController playerController = collision.GetComponent<PlayerController>();

        if (playerController != null)
            anim.SetTrigger("activate");
    }
}