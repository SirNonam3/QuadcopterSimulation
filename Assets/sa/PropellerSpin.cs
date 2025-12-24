using UnityEngine;

public class PropellerSpin : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 2000f; // Speed of rotation
    public Vector3 spinAxis = new Vector3(0, 1, 0); // Rotate around Y (Green axis)

    // Optional: Connect this to stop spinning when paused
    public GameManager gameManager;

    void Update()
    {
        // 1. Check if game is paused (only if you assigned the GameManager)
        if (gameManager != null && gameManager.dronePhysics.isSimulationRunning == false)
        {
            return; // Don't spin if paused
        }

        // 2. Rotate the propeller
        transform.Rotate(spinAxis * speed * Time.deltaTime);
    }
}