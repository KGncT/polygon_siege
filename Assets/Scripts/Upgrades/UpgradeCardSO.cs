using UnityEngine;

public enum UpgradeType
{
    Damage,
    FireRate,
    MoveSpeed,
    ProjectileSpeed,
    Range,
    MaxHealth,
    Heal,
    ProjectileCount,
    Pierce
}

public enum UpgradeRarity
{
    Common,
    Rare,
    Epic
}

[CreateAssetMenu(fileName = "UpgradeCard", menuName = "PolygonSiege/Upgrade Card")]
public class UpgradeCardSO : ScriptableObject
{
    [Header("Görünüm")]
    public string title = "Upgrade";
    [Tooltip("{0} yerine 'value' yazılır. Örn: \"Damage +{0}%\"")]
    public string description = "Damage +{0}%";
    public Sprite icon;
    public UpgradeRarity rarity = UpgradeRarity.Common;

    [Header("Etki")]
    public UpgradeType type;
    [Tooltip("Damage/FireRate/MoveSpeed/ProjectileSpeed/Range: yüzde (15 = +%15)\nMaxHealth/Heal: sabit miktar\nProjectileCount/Pierce: adet")]
    public float value = 10f;

    [Header("Seçim")]
    [Tooltip("Havuzdan çekilme ağırlığı (yüksek = daha sık çıkar)")]
    public float dropWeight = 1f;
    [Tooltip("Bu kart bir oyunda en fazla kaç kez alınabilir")]
    public int maxStacks = 5;

    public string GetDescription()
    {
        return string.Format(description, value);
    }
}
