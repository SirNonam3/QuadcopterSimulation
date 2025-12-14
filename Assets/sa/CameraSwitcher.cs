using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Transform targetCamera; // Drag your Main Camera here

    [Header("Third Person View (TPV)")]
    public Vector3 tpvPosition = new Vector3(0, 2, -5); // Behind and Up
    public Vector3 tpvRotation = new Vector3(20, 0, 0); // Look down slightly

    [Header("First Person View (FPV)")]
    public Vector3 fpvPosition = new Vector3(0, 0.2f, 0.5f); // Front of drone
    public Vector3 fpvRotation = new Vector3(0, 0, 0);      // Look straight

    private bool isFPV = false; // Start in TPV

    void Start()
    {
        // Ensure we start in the correct mode
        UpdateCameraPosition();
    }

    void Update()
    {
        // Check for 'V' key press
        if (Input.GetKeyDown(KeyCode.V))
        {
            isFPV = !isFPV; // Toggle the mode
            UpdateCameraPosition();
        }
    }

    void UpdateCameraPosition()
    {
        if (targetCamera == null) return;

        if (isFPV)
        {
            // Set to FPV (Inside/Front of drone)
            targetCamera.localPosition = fpvPosition;
            targetCamera.localEulerAngles = fpvRotation;
        }
        else
        {
            // Set to TPV (Behind drone)
            targetCamera.localPosition = tpvPosition;
            targetCamera.localEulerAngles = tpvRotation;
        }
    }
}