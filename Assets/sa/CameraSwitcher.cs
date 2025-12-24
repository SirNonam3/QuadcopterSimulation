using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Transform targetCamera; // Drag your Main Camera here

    // --- NEW: Slot for the HUD ---
    public GameObject fpvHud;      // Drag your "HUD_FPV" object here

    [Header("Third Person View (TPV)")]
    public Vector3 tpvPosition = new Vector3(0, 3, -7); // Your preferred setting
    public Vector3 tpvRotation = new Vector3(20, 0, 0);

    [Header("First Person View (FPV)")]
    public Vector3 fpvPosition = new Vector3(0, 0.2f, 0.7f); // Your preferred setting
    public Vector3 fpvRotation = new Vector3(0, 0, 0);

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

        // --- Toggle the UI ---
        if (fpvHud != null)
        {
            fpvHud.SetActive(isFPV); // On if FPV, Off if TPV
        }

        // --- Move the Camera ---
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