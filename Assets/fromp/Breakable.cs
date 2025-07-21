using UnityEngine;
using System.Collections;

public class Breakable : MonoBehaviour, IDamagable
{
    public float health = 100f;
    private bool isWobbling = false; // Flag to track whether wobble should happen

    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log("Enemy took damage! Health: " + health);

        if (!isWobbling)  // Start wobbling if it's not already wobbling
        {
            StartCoroutine(Break());
        }
    }

    // Adjust these variables to control the wobble intensity and speed
    private float wobbleIntensity = 30f;  // Reduced intensity for visible effect
    private float wobbleSpeed = 5f;      // Adjust speed for smoother effect

    IEnumerator Break()
    {
        isWobbling = true; // Set flag to indicate wobble is active

        float timeElapsed = 0f;
        while (timeElapsed < 0.5f)  // Wobble for 1/2 seconds or adjust this value
        {
            // Calculate the wobble angles based on time and intensity
            float angleX = Mathf.Sin(Time.time * wobbleSpeed) * wobbleIntensity;
            float angleZ = Mathf.Cos(Time.time * wobbleSpeed) * wobbleIntensity;

            // Apply the wobble effect continuously
            transform.rotation = Quaternion.Euler(angleX, 0, angleZ);

            timeElapsed += Time.deltaTime;
            yield return null;  // Wait for the next frame
        }

        CollectibleSpawner CS = GetComponent<CollectibleSpawner>();
        
        if (CS != null)
        {
            CS.Spawn();
        }

        Destroy(gameObject);
    }
}
