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

    [Header("Wave / Enemy Ayarları")]
    [SerializeField] private List<WaveConfig> waves;
    [SerializeField] private int currentWaveIndex = 0;

    [System.Serializable]
    public class WaveConfig
    {
        public string waveName = "Wave";
        public List<EnemySpawnWeight> enemies;
    }

    public void IncreaseLives(int amount = 1)
    {
        characterLives += amount;
    }

    public void DecreaseLives(int amount = 1)
    {
        characterLives = Mathf.Max(0, characterLives - amount);
    }

    public void IncreaseEnergy(float amount)
    {
        characterEnergy += amount;
    }

    public void DecreaseEnergy(float amount)
    {
        characterEnergy = Mathf.Max(0f, characterEnergy - amount);
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
}
