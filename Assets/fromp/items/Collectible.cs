using System.Collections;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    // References to colliders
    public Collider rigidBodyCollider;
    public Collider triggerCollider;
    private Rigidbody rb;

    // Player reference (dynamically found)
    private GameObject player;

    // Movement speed for absorb
    public float initialAbsorbSpeed = 1f;
    private float currentAbsorbSpeed;

    // Time limit for absorption before deletion
    private float timeLimit = 60f; // 60 seconds to absorb the collectible

    private bool isBeingAbsorbed = false;
    private bool collectOnDestroy = true; // Default to true (can be set in the inspector)

    private void Start()
    {
        // Ensure the colliders are set correctly
        if (rigidBodyCollider == null)
            rigidBodyCollider = GetComponent<Collider>();

        if (triggerCollider == null)
            triggerCollider = GetComponent<Collider>();

        // Make sure the Rigidbody is properly accessed
        rb = GetComponent<Rigidbody>();

        // Set Rigidbody to kinematic initially if needed
        rb.isKinematic = false; // Make sure it's not kinematic by default unless required
        rb.useGravity = true;   // Enable gravity when it's not being absorbed

        // Start the coroutine to find the closest player
        StartCoroutine(FindPlayersCoroutine());

        // Start the timer for destruction if not absorbed
        StartCoroutine(CheckAbsorptionTimeout());
    }

    private IEnumerator FindPlayersCoroutine()
    {
        // Wait for the players to be found
        yield return new WaitUntil(() => player != null);

        // If player is close enough, start the absorb routine
        if (player != null && !isBeingAbsorbed)
        {
            StartCoroutine(Absorb(player.transform));
        }
    }

    private IEnumerator Absorb(Transform target)
    {
        isBeingAbsorbed = true;
        currentAbsorbSpeed = initialAbsorbSpeed;

        // Turn off gravity while absorbing
        rb.useGravity = false;

        // While not at the player, move the collectible toward the player
        while (Vector3.Distance(transform.position, target.position) > 0.25f) // Absorb is triggered at 75% of the previous distance
        {
            // Lerp towards the player at an increasing speed
            transform.position = Vector3.Lerp(transform.position, target.position, currentAbsorbSpeed * Time.deltaTime);

            // Increment the speed every frame
            currentAbsorbSpeed += 0.5f;

            yield return null;
        }

        // Once close enough, you can handle what happens when it's absorbed
        Debug.Log("Collectible absorbed!");
        Collect(); // Call the Collect method to destroy the object
    }

    private void Update()
    {
        if (!isBeingAbsorbed)
        {
            // Search for players every frame
            SearchForPlayers();
        }
    }

    private void SearchForPlayers()
    {
        // Find all PlayerController objects in the scene
        PlayerController[] players = FindObjectsOfType<PlayerController>();

        // Check if any player is close enough using unsquared distance
        foreach (var playerController in players)
        {
            // Use the unsquared distance check to avoid expensive sqrt calls
            float dx = transform.position.x - playerController.transform.position.x;
            float dz = transform.position.z - playerController.transform.position.z;
            float unsquaredDistance = dx * dx + dz * dz;

            if (unsquaredDistance < 18.75f) // 75% of the previous distance range (25) = 18.75f
            {
                player = playerController.gameObject;
                break; // No need to continue checking if we found a player close enough
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // If the rigidbody collides with an object from the Player layer, collect the item
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Collect();
        }
    }

    // The Collect method destroys the game object
    void Collect()
    {
        if (collectOnDestroy)
        {
            Debug.Log("Item Collected!");
            // Call the Collect method to destroy the object
            Destroy(gameObject);
        }
    }

    private IEnumerator CheckAbsorptionTimeout()
    {
        // Wait for the time limit to pass (60 seconds)
        yield return new WaitForSeconds(timeLimit);

        if (!isBeingAbsorbed)
        {
            // If not absorbed within the time limit, destroy the object
            Debug.LogWarning("Collectible not absorbed in time, destroying!");
            Collect(); // This will call the method to destroy the object
        }
    }

    // Method to stop destruction if absorption started
    private void StopDestructionOnAbsorb()
    {
        StopCoroutine(CheckAbsorptionTimeout());
    }
}
