using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public List<GameObject> enemyPrefabs = new List<GameObject>();
    public List<Transform> spawnPoints = new List<Transform>();
    public float startDelay = 2f;
    public float spawnInterval = 1f;
    public int quantity = 10;

    [Header("Wave Settings")]
    public float waveInterval = 5f;
    public float LevelMultiplier = 1f;
    public int maxWaves = 10; // how many waves before stopping

    [Header("Special Prefabs")]
    public GameObject bossPrefab;
    public GameObject treasurePrefab;
    public Transform specialSpawnPoint; // where to spawn boss/treasure
    public GameObject nextStage;

    private int enemiesSpawned = 0;
    private int waveCount = 0;
    private bool spawningActive = true;

    // Events
    public event Action<int> OnWaveStarted;
    public event Action<int> OnWaveEnded;
    public event Action OnAllWavesCompleted;

    private void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    private IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(startDelay);

        while (spawningActive)
        {
            waveCount++;
            enemiesSpawned = 0;
            OnWaveStarted?.Invoke(waveCount);

            while (enemiesSpawned < quantity)
            {
                SpawnEnemy();
                enemiesSpawned++;
                yield return new WaitForSeconds(spawnInterval);
            }

            OnWaveEnded?.Invoke(waveCount);

            // Check if we've reached the final wave
            if (waveCount >= maxWaves)
            {
                spawningActive = false;
                HandleFinalWaveComplete();
                yield break; // stop the coroutine
            }

            yield return new WaitForSeconds(waveInterval);
        }
    }

    private void SpawnEnemy()
    {
        if (spawnPoints.Count == 0 || enemyPrefabs.Count == 0)
        {
            Debug.LogWarning("Missing spawn points or enemy prefabs on " + gameObject.name);
            return;
        }

        Transform randomSpawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)];
        GameObject randomEnemy = enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Count)];

        GameObject enemyInstance = Instantiate(randomEnemy, randomSpawnPoint.position, randomSpawnPoint.rotation);
        ApplyWaveLevel(enemyInstance);
    }

    private void ApplyWaveLevel(GameObject enemy)
    {
        EnemyController controller = enemy.GetComponent<EnemyController>();
        if (controller != null)
        {
            float extraHealth = waveCount * LevelMultiplier;
            controller.currentHealth += extraHealth;
        }
    }

    private void HandleFinalWaveComplete()
    {
        Debug.Log("All waves complete! Spawning boss/treasure...");
        OnAllWavesCompleted?.Invoke();

        // Spawn special items
        if (specialSpawnPoint == null && spawnPoints.Count > 0)
            specialSpawnPoint = spawnPoints[0];

        if (bossPrefab != null)
        {
            Instantiate(bossPrefab, specialSpawnPoint.position, specialSpawnPoint.rotation);
        }

        if (treasurePrefab != null)
        {
            Vector3 offset = new Vector3(2f, 0, 0); // optional offset
            Instantiate(treasurePrefab, specialSpawnPoint.position + offset, specialSpawnPoint.rotation);
        }

        if (nextStage != null)
        {
            nextStage.gameObject.SetActive(!isActiveAndEnabled);
        }
    }
}
