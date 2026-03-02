using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Animator anim => GetComponent<Animator>();
    private bool active;



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (active) return;
        
        PlayerController playercontroller = collision.GetComponent<PlayerController>();

        if (playercontroller !=null)
            ActivateCheckpoint();
    }

    private void ActivateCheckpoint()
    {
        active = true;
        anim.SetBool("activate", active);
        GameManager.instance.UpdateRespawnPosition(transform);
    }
}
