using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private int currentScore;
    public int CurrentScore => currentScore;
    private LevelUIManager levelUIManager;

    [Header("Zoom Thresholds")]
    [SerializeField] private int[] zoomScoreThresholds = { 10, 20, 30 };
    // [SerializeField] private int[] zoomScoreThresholds = { 100, 300, 600 };
    private int nextThresholdIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        levelUIManager = FindAnyObjectByType<LevelUIManager>();

    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        levelUIManager.UpdateScore(currentScore);
        CheckZoomThreshold();

        // Öldürülen düşmanın puanı kadar XP kazandır
        if (UpgradeManager.Instance != null)
            UpgradeManager.Instance.AddXP(amount);
    }

    private void CheckZoomThreshold()
    {
        if (nextThresholdIndex >= zoomScoreThresholds.Length) return;

        if (currentScore >= zoomScoreThresholds[nextThresholdIndex])
        {
            CameraFollow.Instance.ZoomOutStep();
            nextThresholdIndex++;
        }
    }

    public void ResetScore()
    {
        currentScore = 0;
        nextThresholdIndex = 0;
        levelUIManager.UpdateScore(currentScore);
    }

}