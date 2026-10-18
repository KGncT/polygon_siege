using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthbarActions : MonoBehaviour
{
    [SerializeField] private Image healthbar_front;
    [SerializeField] private Image healthbar_back;

    private IEnumerator HealthbarDecreaseAction(float healthNormalized)
    {
        healthbar_front.fillAmount = healthNormalized;
        while(healthbar_back.fillAmount > healthbar_front.fillAmount)
        {
            healthbar_back.fillAmount = Mathf.Lerp(healthbar_back.fillAmount,healthbar_front.fillAmount, 0.3f);
            yield return null;
        }
    }
    
    private IEnumerator HealthbarIncreaseAction(float healthNormalized)
    {
        healthbar_front.fillAmount = healthNormalized;
        while(healthbar_back.fillAmount < healthbar_front.fillAmount)
        {
            healthbar_back.fillAmount = Mathf.Lerp(healthbar_back.fillAmount,healthbar_front.fillAmount, 0.3f);
            yield return null;
        }
    }

    public void IncreaseHealthbar(float healthNormalized)
    {
        StartCoroutine(HealthbarIncreaseAction(healthNormalized));
    }

    public void DecreaseHealthbar(float healthNormalized)
    {
        StartCoroutine(HealthbarDecreaseAction(healthNormalized));
    }
}
