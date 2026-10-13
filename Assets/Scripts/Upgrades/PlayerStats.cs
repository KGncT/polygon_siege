using UnityEngine;

// Oyuncunun run boyunca kazandığı çarpanlar.
// ScriptableObject'ler (WeaponDataSO vb.) runtime'da DEĞİŞTİRİLMEZ; bu sınıf onların üstüne çarpan uygular.
// Player objesine eklenir.
public class PlayerStats : MonoBehaviour
{
    public float DamageMultiplier { get; private set; } = 1f;
    public float FireRateMultiplier { get; private set; } = 1f;
    public float MoveSpeedMultiplier { get; private set; } = 1f;
    public float ProjectileSpeedMultiplier { get; private set; } = 1f;
    public float RangeMultiplier { get; private set; } = 1f;
    public int ExtraProjectiles { get; private set; } = 0;
    public int ExtraPierce { get; private set; } = 0;

    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
        if (player == null) player = FindAnyObjectByType<Player>();
    }

    public void Apply(UpgradeCardSO card)
    {
        float pct = card.value / 100f;

        switch (card.type)
        {
            case UpgradeType.Damage:          DamageMultiplier += pct; break;
            case UpgradeType.FireRate:        FireRateMultiplier += pct; break;
            case UpgradeType.MoveSpeed:       MoveSpeedMultiplier += pct; break;
            case UpgradeType.ProjectileSpeed: ProjectileSpeedMultiplier += pct; break;
            case UpgradeType.Range:           RangeMultiplier += pct; break;
            case UpgradeType.ProjectileCount: ExtraProjectiles += Mathf.RoundToInt(card.value); break;
            case UpgradeType.Pierce:          ExtraPierce += Mathf.RoundToInt(card.value); break;
            case UpgradeType.MaxHealth:       player?.IncreaseMaxHealth(Mathf.RoundToInt(card.value)); break;
            case UpgradeType.Heal:            player?.Heal(Mathf.RoundToInt(card.value)); break;
        }
    }
}
