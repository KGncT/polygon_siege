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
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GroundConstructor groundConstructor;

    // [YENİ] Level bitişi
    [Header("Level Bitişi")]
    [Tooltip("Dalgalar süreye göre otomatik ilerlesin. AdvanceWave()'i başka yerden çağırıyorsan kapat.")]
    [SerializeField] private bool autoAdvanceWaves = true;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private Transform bossSpawnPoint;

    public bool IsLevelCleared { get; private set; }
    public bool IsBossPhase { get; private set; }
    public int CurrentWaveIndex => currentWaveIndex;
    public int WaveCount => waves != null ? waves.Count : 0;

    // UI bağlamak istersen abone ol
    public event System.Action<int, int> OnWaveStarted;   // (index, toplam)
    public event System.Action OnBossSpawned;
    public event System.Action OnLevelCleared;

    private float waveTimer;
    private Enemy boss;

    [System.Serializable]
    public class WaveConfig
    {
        public string waveName = "Wave";
        public List<EnemySpawnWeight> enemies;

        // [YENİ]
        [Tooltip("Normal dalga süresi (sn). Süre dolunca sonraki dalgaya geçilir.")]
        public float duration = 45f;
        [Tooltip("Boss dalgası: boss ölünce level biter. 'enemies' listesi boss'un yanında çıkacak ek düşmanlardır.")]
        public bool isBossWave;
        public Enemy bossPrefab;
    }

    void Start()
    {
        groundConstructor.ConstructGround();

        uiManager = GetComponent<LevelUIManager>();
        gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);   // [YENİ]
        Time.timeScale = 1;

        uiManager.UpdateLives(characterLives);
        uiManager.UpdateScore(0);

        BeginWave(currentWaveIndex);                               // [YENİ]
    }

    // [YENİ] Süre / boss takibi
    void Update()
    {
        if (!autoAdvanceWaves || isGameOver || IsLevelCleared) return;
        if (waves == null || waves.Count == 0) return;

        if (IsBossPhase)
        {
            if (boss == null || !boss.gameObject.activeInHierarchy)
                CompleteLevel();
            return;
        }

        waveTimer += Time.deltaTime;
        if (waveTimer >= waves[currentWaveIndex].duration)
            NextWaveOrFinish();
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
        if (IsLevelCleared) return null;   // [YENİ] level bitince spawn dursun
        Debug.Log("in GetCurrentWaveEnemies");
        if (waves == null || waves.Count == 0) return null;
        Debug.Log("condition passed");
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

    // [YENİ] ---------------------------------------------------------

    private void BeginWave(int index)
    {
        if (waves == null || waves.Count == 0) return;

        currentWaveIndex = Mathf.Clamp(index, 0, waves.Count - 1);
        waveTimer = 0f;
        OnWaveStarted?.Invoke(currentWaveIndex, waves.Count);

        WaveConfig w = waves[currentWaveIndex];
        if (w.isBossWave) SpawnBoss(w);
    }

    private void NextWaveOrFinish()
    {
        if (currentWaveIndex + 1 < waves.Count)
            BeginWave(currentWaveIndex + 1);
        else
            CompleteLevel();   // son dalga boss değilse süre dolunca level biter
    }

    private void SpawnBoss(WaveConfig w)
    {
        if (w.bossPrefab == null)
        {
            Debug.LogWarning("Boss dalgasında bossPrefab atanmamış!");
            CompleteLevel();
            return;
        }

        Vector3 pos = bossSpawnPoint != null ? bossSpawnPoint.position : transform.position;
        boss = Instantiate(w.bossPrefab, pos, Quaternion.identity);
        // Enemy veri/pool init'i gerekiyorsa buraya ekle (ör. boss.Initialize(...))
        IsBossPhase = true;
        OnBossSpawned?.Invoke();
    }

    private void CompleteLevel()
    {
        if (IsLevelCleared || isGameOver) return;
        IsLevelCleared = true;
        IsBossPhase = false;

        ClearRemainingEnemies();
        OnLevelCleared?.Invoke();

        Time.timeScale = 0;
        if (victoryPanel != null) victoryPanel.SetActive(true);
    }

    // Kalan düşmanları temizle. Kendi ölüm/pool metoduna geçirmek istersen burayı değiştir.
    private void ClearRemainingEnemies()
    {
        Enemy[] all = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (Enemy e in all)
            e.gameObject.SetActive(false);
    }

    public void OnGameOver()
    {
        Time.timeScale = 0;
        gameOverPanel.SetActive(true);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        pausePanel.SetActive(false);
    }
}
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using System.Collections.Generic;


// // LevelManager'da olması beklenen (yoksa ekleyebilirsin):
// public interface ILevelEnemyProvider
// {
//     List<EnemySpawnWeight> GetCurrentWaveEnemies();
// }

// [System.Serializable]
// public class EnemySpawnWeight
// {
//     public EnemyDataSO enemyData;
//     public Enemy enemyPrefab;
//     public float weight = 1f;
// }

// public class LevelManager : MonoBehaviour, ILevelEnemyProvider
// {
//     [Header("Player Stats")]
//     public int characterLives = 3;
//     public float characterEnergy = 100f;
//     public float maxEnergy = 100f;

//     [Header("Wave / Enemy Ayarları")]
//     [SerializeField] private List<WaveConfig> waves;
//     [SerializeField] private int currentWaveIndex = 0;
//     private bool isGameOver;
//     public bool IsGameOver => isGameOver;
//     private LevelUIManager uiManager;
//     [SerializeField] private GameObject gameOverPanel;
//     [SerializeField] private GameObject pausePanel;
//     [SerializeField] private GroundConstructor groundConstructor;

//     [System.Serializable]
//     public class WaveConfig
//     {
//         public string waveName = "Wave";
//         public List<EnemySpawnWeight> enemies;
//     }

//     void Start()
//     {
//         groundConstructor.ConstructGround();

//         uiManager = GetComponent<LevelUIManager>();
//         gameOverPanel.SetActive(false);
//         Time.timeScale = 1;

//         uiManager.UpdateLives(characterLives);
//         uiManager.UpdateScore(0);
//     }

//     public void IncreaseLives(int amount = 1)
//     {
//         characterLives += amount;
//     }

//     public void DecreaseLives(int amount = 1)
//     {
//         characterLives = Mathf.Max(0, characterLives - amount);
//         uiManager.UpdateLives(characterLives);

//         if(characterLives <= 0)
//         {
//             isGameOver = true;
//             OnGameOver();            
//         }
//     }

//     public void IncreaseEnergy(float amount)
//     {
//         characterEnergy = Mathf.Min(maxEnergy, characterEnergy + amount);
//         uiManager.IncreaseHealthbar(characterEnergy / maxEnergy);
//     }

//     // Upgrade kartı: max can artar, artış kadar da mevcut can eklenir
//     public void IncreaseMaxEnergy(float amount)
//     {
//         maxEnergy += amount;
//         characterEnergy += amount;
//         uiManager.IncreaseHealthbar(characterEnergy / maxEnergy);
//     }

//     public void DecreaseEnergy(float amount)
//     {
//         characterEnergy = Mathf.Max(0f, characterEnergy - amount);
//         uiManager.DecreaseHealthbar(characterEnergy / maxEnergy);
//     }

//     // ILevelEnemyProvider implementasyonu
//     public List<EnemySpawnWeight> GetCurrentWaveEnemies()
//     {
//         Debug.Log("in GetCurrentWaveEnemies");
//         if (waves == null || waves.Count == 0) return null;
//         Debug.Log("condition passed");
//         int index = Mathf.Clamp(currentWaveIndex, 0, waves.Count - 1);
//         return waves[index].enemies;
//     }

//     public void AdvanceWave()
//     {
//         if (waves == null) return;
//         currentWaveIndex = Mathf.Min(currentWaveIndex + 1, waves.Count - 1);
//     }

//     public void SetWave(int index)
//     {
//         if (waves == null) return;
//         currentWaveIndex = Mathf.Clamp(index, 0, waves.Count - 1);
//     }

//     public void OnGameOver()
//     {
//         Time.timeScale = 0;
//         gameOverPanel.SetActive(true);
//     }

//     public void RestartLevel()
//     {
//         SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
//     }

//     public void GoToMainMenu()
//     {
//         SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
//     }

//     public void PauseGame()
//     {
//         Time.timeScale = 0;
//         pausePanel.SetActive(true);
//     }

//     public void ResumeGame()
//     {
//         Time.timeScale = 1;
//         pausePanel.SetActive(false);
//     }
// }
