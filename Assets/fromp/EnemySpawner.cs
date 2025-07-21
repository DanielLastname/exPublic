using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public List<GameObject> enemyPrefabs = new List<GameObject>(); // Changed to list
    public List<Transform> spawnPoints = new List<Transform>();
    public float startDelay = 2f;
    public float spawnInterval = 1f;
    public int quantity = 10;

    private int enemiesSpawned = 0;

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(startDelay);

        while (enemiesSpawned < quantity)
        {
            SpawnEnemy();
            enemiesSpawned++;
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        if (spawnPoints.Count == 0 || enemyPrefabs.Count == 0)
        {
            Debug.LogWarning("Missing spawn points or enemy prefabs on " + gameObject.name);
            return;
        }

        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
        GameObject randomEnemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

        Instantiate(randomEnemy, randomSpawnPoint.position, randomSpawnPoint.rotation);
    }
}
