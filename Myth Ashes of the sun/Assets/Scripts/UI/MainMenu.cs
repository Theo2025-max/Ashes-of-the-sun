using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   public void PlayGame()
    {
        SceneManager.LoadScene("Level 1");
    }


    public void QuitGame()
    {
        Debug.Log("You silly Goose , this doesnt work on Editor");
        Application.Quit();
    }
}
