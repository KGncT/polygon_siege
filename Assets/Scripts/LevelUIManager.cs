using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LevelUIManager : MonoBehaviour
{
    [SerializeField] private HealthbarActions healthbarActions;
    [SerializeField] private HeartsActions heartsActions;
    [SerializeField] private TwoSidedLevelBar xpBar;   
    [SerializeField] private TMP_Text scoreText;

    public void UpdateLives(int lives)
    {
        heartsActions.UpdateHearts(lives);
    }

    public void IncreaseHealthbar(float healthNormalized)
    {
        healthbarActions.IncreaseHealthbar(healthNormalized);
    }

    public void DecreaseHealthbar(float healthNormalized)
    {
        healthbarActions.DecreaseHealthbar(healthNormalized);
    }

    public void UpdateXP(float normalized, int level)
    {            
        xpBar.UpdateLevelBar(normalized, level);
    }

    public void UpdateScore(int score)
    {
        scoreText.text = $"Score: {score.ToString("D6")}";
    }

    public void ResetUIElements(int lives, float healthNormalized)
    {
        UpdateLives(lives);
        IncreaseHealthbar(healthNormalized);

    }
    

}
