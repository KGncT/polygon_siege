using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TwoSidedLevelBar : MonoBehaviour
{
    [SerializeField] private Image leftBar;
    [SerializeField] private Image rightBar;
    [SerializeField] private TMP_Text levelText;    

    public void UpdateLevelBar(float normalized, int level)
    {
        leftBar.fillAmount = normalized;
        rightBar.fillAmount = normalized;
        levelText.text = $"Lv {level}";
    }
}
