using UnityEngine;
using UnityEngine.UI; 

public class ShowUIOnTrigger : MonoBehaviour
{
    [Tooltip("Assign the UI element you want to show when the player enters the trigger.")]
    [SerializeField] private GameObject uiElement;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController playerController = collision.GetComponent<PlayerController>();
        if (playerController != null)
        {
            uiElement.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerController playerController = collision.GetComponent<PlayerController>();
        if (playerController != null)
        {
            uiElement.SetActive(false);
        }
    }
}