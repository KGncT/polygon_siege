using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private Image healthbarFill;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Player player;
    [SerializeField] private LevelManager levelManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateScore(0);
    }

    public void UpdateLives(int lives)
    {
        livesText.text = $"x {lives}";
    }

    public void UpdateHealth(float healthNormalized)
    {
        healthbarFill.fillAmount = healthNormalized;
    }

    public void UpdateScore(int score)
    {
        scoreText.text = $"Score: {score.ToString("D6")}";
    }
}
