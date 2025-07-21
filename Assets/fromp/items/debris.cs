using UnityEngine;

public class Debris : MonoBehaviour
{
    // Time after which the debris will destroy itself
    public float lifeTime = 45f;

    private void Start()
    {
        // Start the timer for destruction
        Destroy(gameObject, lifeTime);
    }
}