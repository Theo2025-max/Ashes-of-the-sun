using FirstGearGames.SmoothCameraShaker;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Pandorasbox : MonoBehaviour
{
    [SerializeField] private float max_health = 100f;

    private float current_health;

    public Image healthbar_foreground;
    public Image healthbar_background;
    public ShakeData damage_shake;
    public TextMeshProUGUI SCORE_TEXT;

    private float time_survived;
    private int lastDisplayedScore = -1;

    private void Start()
    {
        max_health = Mathf.Max(1f, max_health);
        current_health = max_health;

        UpdateHealthUI(true);
    }

    private void Update()
    {
        current_health = Mathf.Clamp(current_health, 0f, max_health);

        UpdateHealthUI(false);

        time_survived += Time.deltaTime;

        int score = (int)time_survived;

        if (score == lastDisplayedScore) return;

        lastDisplayedScore = score;

        if (SCORE_TEXT != null) SCORE_TEXT.text = score.ToString();
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0f) return;

        current_health = Mathf.Clamp(current_health - amount, 0f, max_health);

        UpdateHealthUI(false);

        if (damage_shake != null) CameraShakerHandler.Shake(damage_shake);
    }

    private void UpdateHealthUI(bool immediate)
    {
        float targetFill = max_health > 0f ? current_health / max_health : 0f;

        if (healthbar_foreground != null) healthbar_foreground.fillAmount = targetFill;

        if (healthbar_background == null) return;

        if (immediate)
        {
            healthbar_background.fillAmount = targetFill;
            return;
        }

        // Matches the old 0.01-per-frame smoothing at roughly 60 FPS
        // without making the visual behaviour frame-rate dependent.
        float lerpFactor = 1f - Mathf.Pow(.99f, Time.deltaTime * 60f);

        healthbar_background.fillAmount = Mathf.Lerp(healthbar_background.fillAmount, targetFill, lerpFactor);
    }
}