using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleSpawner : MonoBehaviour
{
    // List for spawning all items
    public List<GameObject> spawnItems = new List<GameObject>();

    // List for spawning a random item
    public List<GameObject> randomSpawnItems = new List<GameObject>();

    // Push force when items are spawned
    public float pushForce = 5f;

    public float BummerChance = 66;

    // Method to handle item destruction and spawning
    public void Spawn()
    {
        // Spawn all items in the spawnItems list
        if (spawnItems.Count > 0)
        {
            foreach (GameObject item in spawnItems)
            {
                SpawnItem(item);
            }
        }

        // Spawn a random item from the randomSpawnItems list
        if (randomSpawnItems.Count > 0)
        {
            if (Random.RandomRange(0, 100) < BummerChance) return;
            int randomIndex = Random.Range(0, randomSpawnItems.Count);
            SpawnItem(randomSpawnItems[randomIndex]);
        }

    }

    // Spawn a given item and apply force to it
    void SpawnItem(GameObject item)
    {
        if (item != null)
        {
            // Instantiate the item at the current position of the collectible
            GameObject spawnedItem = Instantiate(item, transform.position, Quaternion.identity);

            // Get the Rigidbody of the spawned item
            Rigidbody rb = spawnedItem.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Apply a push force upwards and in a random direction
                Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f)).normalized;
                rb.AddForce(randomDirection * pushForce, ForceMode.Impulse);
            }
        }
    }

}
