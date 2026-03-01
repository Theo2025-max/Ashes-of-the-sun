using FirstGearGames.SmoothCameraShaker;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Pandorasbox : MonoBehaviour
{
    [SerializeField] float max_health = 100;
    private float current_health;
    public Image healthbar_foreground;
    public Image healthbar_background;
    public ShakeData damage_shake;
    public TextMeshProUGUI SCORE_TEXT;
    private float time_survived;

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
    }


    public void take_damage(float amount)
    {
        current_health -= amount;
        CameraShakerHandler.Shake(damage_shake);
    }
}
