using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


// LevelManager'da olması beklenen (yoksa ekleyebilirsin):
public interface ILevelEnemyProvider
{
    List<EnemySpawnWeight> GetCurrentWaveEnemies();
}

[System.Serializable]
public class EnemySpawnWeight
{
    public EnemyDataSO enemyData;
    public Enemy enemyPrefab;
    public float weight = 1f;
}

public class LevelManager : MonoBehaviour, ILevelEnemyProvider
{
    [Header("Player Stats")]
    public int characterLives = 3;
    public float characterEnergy = 100f;
    public float maxEnergy = 100f;

    [Header("Wave / Enemy Ayarları")]
    [SerializeField] private List<WaveConfig> waves;
    [SerializeField] private int currentWaveIndex = 0;
    private bool isGameOver;
    public bool IsGameOver => isGameOver;
    private LevelUIManager uiManager;
    [SerializeField] private GameObject panel;

    [System.Serializable]
    public class WaveConfig
    {
        public string waveName = "Wave";
        public List<EnemySpawnWeight> enemies;
    }

    void Start()
    {
        uiManager = GetComponent<LevelUIManager>();
        panel.SetActive(false);
        Time.timeScale = 1;
    }

    public void IncreaseLives(int amount = 1)
    {
        characterLives += amount;
    }

    public void DecreaseLives(int amount = 1)
    {
        characterLives = Mathf.Max(0, characterLives - amount);
        uiManager.UpdateLives(characterLives);

        if(characterLives <= 0)
        {
            isGameOver = true;
            OnGameOver();            
        }
    }

    public void IncreaseEnergy(float amount)
    {
        characterEnergy = Mathf.Min(maxEnergy, characterEnergy + amount);
        uiManager.IncreaseHealthbar(characterEnergy / maxEnergy);
    }

    // Upgrade kartı: max can artar, artış kadar da mevcut can eklenir
    public void IncreaseMaxEnergy(float amount)
    {
        maxEnergy += amount;
        characterEnergy += amount;
        uiManager.IncreaseHealthbar(characterEnergy / maxEnergy);
    }

    public void DecreaseEnergy(float amount)
    {
        characterEnergy = Mathf.Max(0f, characterEnergy - amount);
        uiManager.DecreaseHealthbar(characterEnergy / maxEnergy);
    }

    // ILevelEnemyProvider implementasyonu
    public List<EnemySpawnWeight> GetCurrentWaveEnemies()
    {
        if (waves == null || waves.Count == 0) return null;

        int index = Mathf.Clamp(currentWaveIndex, 0, waves.Count - 1);
        return waves[index].enemies;
    }

    public void AdvanceWave()
    {
        if (waves == null) return;
        currentWaveIndex = Mathf.Min(currentWaveIndex + 1, waves.Count - 1);
    }

    public void SetWave(int index)
    {
        if (waves == null) return;
        currentWaveIndex = Mathf.Clamp(index, 0, waves.Count - 1);
    }

    public void OnGameOver()
    {
        Time.timeScale = 0;
        panel.SetActive(true);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
