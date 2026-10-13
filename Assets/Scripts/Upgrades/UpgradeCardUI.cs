using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Tek bir kart slotu. Kart prefab'ının / panel içindeki slotun kökine eklenir.
public class UpgradeCardUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text stackText;   // opsiyonel: "2/5"
    [SerializeField] private Image iconImage;      // opsiyonel
    [SerializeField] private Image borderImage;    // opsiyonel: nadirliğe göre renklenir

    [Header("Nadirlik Renkleri")]
    [SerializeField] private Color commonColor = new Color(0.75f, 0.75f, 0.75f);
    [SerializeField] private Color rareColor = new Color(0.30f, 0.60f, 1.00f);
    [SerializeField] private Color epicColor = new Color(0.70f, 0.30f, 1.00f);

    [SerializeField] private float popInDuration = 0.18f;

    public void Setup(UpgradeCardSO card, int currentStacks, Action<UpgradeCardSO> onChosen)
    {
        titleText.text = card.title;
        descriptionText.text = card.GetDescription();

        if (stackText != null)
            stackText.text = card.maxStacks > 1 && card.maxStacks < 99 ? $"{currentStacks}/{card.maxStacks}" : "";

        if (iconImage != null)
        {
            iconImage.sprite = card.icon;
            iconImage.enabled = card.icon != null;
        }

        if (borderImage != null)
        {
            switch (card.rarity)
            {
                case UpgradeRarity.Rare: borderImage.color = rareColor; break;
                case UpgradeRarity.Epic: borderImage.color = epicColor; break;
                default:                 borderImage.color = commonColor; break;
            }
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onChosen?.Invoke(card));

        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(PopIn());
    }

    public void Hide()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    // Time.timeScale = 0 iken çalışması için unscaled süre kullanılır
    private IEnumerator PopIn()
    {
        Transform t = transform;
        t.localScale = Vector3.zero;
        float elapsed = 0f;

        while (elapsed < popInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(elapsed / popInDuration);
            t.localScale = Vector3.one * Mathf.SmoothStep(0f, 1f, k);
            yield return null;
        }
        t.localScale = Vector3.one;
    }
}
