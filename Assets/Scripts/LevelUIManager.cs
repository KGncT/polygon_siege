using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LevelUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private Image healthbar_yellow;
    [SerializeField] private Image healthbar_green;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Image xpBarFill;   // opsiyonel: Image Type = Filled
    [SerializeField] private TMP_Text levelText; // opsiyonel
    [SerializeField] private Player player;
    [SerializeField] private LevelManager levelManager;
    private float currentHealth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateScore(0);
    }

    public void UpdateLives(int lives)
    {
        livesText.text = $"x {lives}";
    }

    public void IncreaseHealthbar(float healthNormalized)
    {
        currentHealth = healthNormalized;

        StartCoroutine(HealthbarIncreaseAction(healthNormalized));
    }

    public void DecreaseHealthbar(float healthNormalized)
    {
        currentHealth = healthNormalized;

        StartCoroutine(HealthbarDecreaseAction(healthNormalized));
    }

    public void UpdateXP(float normalized, int level)
    {
        if (xpBarFill != null) xpBarFill.fillAmount = normalized;
        if (levelText != null) levelText.text = $"Lv {level}";
    }

    public void UpdateScore(int score)
    {
        scoreText.text = $"Score: {score.ToString("D6")}";
    }


    private IEnumerator HealthbarDecreaseAction(float healthNormalized)
    {
        healthbar_yellow.fillAmount = healthNormalized;
        while(healthbar_green.fillAmount > healthbar_yellow.fillAmount)
        {
            healthbar_green.fillAmount = Mathf.Lerp(healthbar_green.fillAmount,healthbar_yellow.fillAmount, 0.3f);
            yield return null;
        }
    }
    
    private IEnumerator HealthbarIncreaseAction(float healthNormalized)
    {
        healthbar_yellow.fillAmount = healthNormalized;
        while(healthbar_green.fillAmount < healthbar_yellow.fillAmount)
        {
            healthbar_green.fillAmount = Mathf.Lerp(healthbar_green.fillAmount,healthbar_yellow.fillAmount, 0.3f);
            yield return null;
        }
    }
    

}
