using System.Collections.Generic;
using UnityEngine;

// XP / level takibi + level atlayınca oyunu durdurup kart seçtirir.
// Level sahnesine (LevelManager'ın olduğu objeye ya da ayrı bir objeye) eklenir.
public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [Header("Referanslar")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private LevelUIManager uiManager;
    [SerializeField] private LevelManager levelManager;

    [Header("Kart Havuzu")]
    [SerializeField] private List<UpgradeCardSO> cardPool;
    [SerializeField] private int cardsPerOffer = 3;

    [Header("XP Eğrisi")]
    [SerializeField] private int baseXpToLevel = 50;
    [SerializeField] private float xpGrowth = 1.35f;

    [Header("UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private UpgradeCardUI[] cardSlots;

    private int level = 1;
    private int currentXP;
    private int xpToNext;
    private int pendingLevelUps;
    private bool isChoosing;

    private readonly Dictionary<UpgradeCardSO, int> stacks = new Dictionary<UpgradeCardSO, int>();
    private readonly List<UpgradeCardSO> offered = new List<UpgradeCardSO>();

    public int Level => level;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        panel.SetActive(false);
        xpToNext = XpForLevel(level);
        RefreshUI();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    // ScoreManager.AddScore tarafından çağrılır (öldürülen düşmanın scoreValue'su kadar XP)
    public void AddXP(int amount)
    {
        if (amount <= 0) return;

        currentXP += amount;

        while (currentXP >= xpToNext)
        {
            currentXP -= xpToNext;
            level++;
            pendingLevelUps++;
            xpToNext = XpForLevel(level);
        }

        RefreshUI();

        if (!isChoosing && pendingLevelUps > 0)
            ShowOffer();
    }

    private int XpForLevel(int lvl)
    {
        return Mathf.RoundToInt(baseXpToLevel * Mathf.Pow(xpGrowth, lvl - 1));
    }

    private void RefreshUI()
    {
        if (uiManager != null)
            uiManager.UpdateXP(xpToNext > 0 ? (float)currentXP / xpToNext : 0f, level);
    }

    private void ShowOffer()
    {
        if (levelManager != null && levelManager.IsGameOver)
        {
            pendingLevelUps = 0;
            return;
        }

        PickCards(Mathf.Min(cardsPerOffer, cardSlots.Length));

        if (offered.Count == 0)
        {
            // Havuz tükendi, seçilecek kart yok
            pendingLevelUps = 0;
            return;
        }

        isChoosing = true;
        Time.timeScale = 0f;
        panel.SetActive(true);

        for (int i = 0; i < cardSlots.Length; i++)
        {
            if (i < offered.Count)
                cardSlots[i].Setup(offered[i], GetStacks(offered[i]), OnCardChosen);
            else
                cardSlots[i].Hide();
        }
    }

    private void OnCardChosen(UpgradeCardSO card)
    {
        if (!isChoosing) return; // çift tıklama koruması

        stacks[card] = GetStacks(card) + 1;
        playerStats.Apply(card);

        pendingLevelUps--;
        isChoosing = false;
        panel.SetActive(false);
        Time.timeScale = 1f;

        // Aynı anda birden fazla level atlandıysa sıradaki teklifi göster
        if (pendingLevelUps > 0)
            ShowOffer();
    }

    private int GetStacks(UpgradeCardSO card)
    {
        return stacks.TryGetValue(card, out int count) ? count : 0;
    }

    // Ağırlıklı, tekrarsız seçim; maxStacks dolan kartlar elenir
    private void PickCards(int count)
    {
        offered.Clear();

        List<UpgradeCardSO> candidates = new List<UpgradeCardSO>();
        foreach (var c in cardPool)
        {
            if (c != null && GetStacks(c) < c.maxStacks)
                candidates.Add(c);
        }

        while (offered.Count < count && candidates.Count > 0)
        {
            float total = 0f;
            foreach (var c in candidates) total += Mathf.Max(0f, c.dropWeight);

            int pickIndex = candidates.Count - 1;
            if (total > 0f)
            {
                float roll = Random.Range(0f, total);
                float cumulative = 0f;
                for (int i = 0; i < candidates.Count; i++)
                {
                    cumulative += Mathf.Max(0f, candidates[i].dropWeight);
                    if (roll <= cumulative) { pickIndex = i; break; }
                }
            }

            offered.Add(candidates[pickIndex]);
            candidates.RemoveAt(pickIndex);
        }
    }

    [ContextMenu("Debug: Level Atlat")]
    private void DebugLevelUp()
    {
        AddXP(xpToNext - currentXP);
    }
}
