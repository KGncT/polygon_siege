using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Referanslar")]
    [SerializeField] private Transform player;
    [SerializeField] private LevelManager levelManager;

    [Header("Spawn Noktaları")]
    [SerializeField] private Transform spawnPointsParent; // içinde child transform'lar var
    private Transform[] spawnPoints;

    [Header("Zamanlama")]
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private int prewarmCountPerType = 10;

    private Dictionary<EnemyDataSO, ObjectPool<Enemy>> pools = new Dictionary<EnemyDataSO, ObjectPool<Enemy>>();
    private Coroutine spawnRoutine;

    private void Awake()
    {
        CacheSpawnPoints();
    }

    private void CacheSpawnPoints()
    {
        if (spawnPointsParent == null)
        {
            Debug.LogError("[EnemySpawner] spawnPointsParent atanmadı!");
            return;
        }

        int count = spawnPointsParent.childCount;
        spawnPoints = new Transform[count];
        for (int i = 0; i < count; i++)
            spawnPoints[i] = spawnPointsParent.GetChild(i);
    }

    private void OnEnable()
    {
        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    private void OnDisable()
    {
        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        List<EnemySpawnWeight> currentWave = levelManager.GetCurrentWaveEnemies();
        if (currentWave == null || currentWave.Count == 0) return;

        EnemySpawnWeight entry = PickWeightedEntry(currentWave);
        if (entry == null) return;

        Transform point = GetRandomSpawnPoint();
        if (point == null) return;

        Vector3 spawnPos = point.position;
        Quaternion spawnRot = Quaternion.LookRotation((player.position - spawnPos).normalized);

        ObjectPool<Enemy> pool = GetOrCreatePool(entry);
        // Enemy enemy = pool.Get(spawnPos);
        Enemy enemy = pool.Get(spawnPos, spawnRot);
        enemy.Init(entry.enemyData, player, (e) => pool.Return(e));
    }

    private ObjectPool<Enemy> GetOrCreatePool(EnemySpawnWeight entry)
    {
        if (!pools.TryGetValue(entry.enemyData, out ObjectPool<Enemy> pool))
        {
            pool = new ObjectPool<Enemy>(entry.enemyPrefab, prewarmCountPerType);
            pools[entry.enemyData] = pool;
        }
        return pool;
    }

    private Transform GetRandomSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0) return null;
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }

    private EnemySpawnWeight PickWeightedEntry(List<EnemySpawnWeight> wave)
    {
        float totalWeight = 0f;
        foreach (var e in wave) totalWeight += e.weight;

        float rnd = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var e in wave)
        {
            cumulative += e.weight;
            if (rnd <= cumulative) return e;
        }
        return wave[wave.Count - 1];
    }

    // Editor'de spawn noktalarını görselleştirme
    private void OnDrawGizmosSelected()
    {
        if (spawnPointsParent == null) return;

        Gizmos.color = Color.red;
        foreach (Transform t in spawnPointsParent)
        {
            Gizmos.DrawWireSphere(t.position, 0.5f);
        }
    }


}