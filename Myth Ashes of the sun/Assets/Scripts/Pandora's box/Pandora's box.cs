using FirstGearGames.SmoothCameraShaker;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Pandorasbox : MonoBehaviour
{
    [SerializeField] float max_health = 100;
    public float current_health;
    public Image healthbar_foreground;
    public Image healthbar_background;
    public ShakeData damage_shake;
    public TextMeshProUGUI SCORE_TEXT;
    private float time_survived;
    public GameObject death_screen;
    public AudioSource damage_sound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        current_health = max_health;
    }

    // Update is called once per frame
    void Update()
    {
        healthbar_foreground.fillAmount = current_health/100;
        healthbar_background.fillAmount = Mathf.Lerp(healthbar_background.fillAmount, healthbar_foreground.fillAmount, 0.01f);
        current_health = Mathf.Clamp(current_health, 0, max_health);
        time_survived += Time.deltaTime;
        int score = (int)time_survived;
        SCORE_TEXT.text = score.ToString();
        if (current_health == 0)
        {
            death_screen.SetActive(true);
            print("dead");
        }
    }


    public void take_damage(float amount)
    {
        damage_sound.Play();
        current_health -= amount;
        CameraShakerHandler.Shake(damage_shake);
        if(current_health == 0)
        {
            death_screen.SetActive(true);
            print("dead");
        }
    }

    public void quit()
    {
        SceneManager.LoadScene(0);
    }

    public void restart()
    {
        SceneManager.LoadScene("Level 1");
    }
}
