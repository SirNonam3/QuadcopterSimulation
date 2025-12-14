using UnityEngine;
using TMPro; 
using System.Text.RegularExpressions; 
using UnityEngine.SceneManagement; // REQUIRED for "Factory Reset"

public class GameManager : MonoBehaviour
{
    public QuadcopterPhysics dronePhysics;
    public GameObject menuPanel;
    public TextMeshProUGUI buttonText;

    [Header("UI Input Fields")]
    public TMP_InputField inputMass;
    public TMP_InputField inputLength;
    
    [Header("Initial State Inputs")]
    public TMP_InputField inputPosX;
    public TMP_InputField inputPosY;
    public TMP_InputField inputPosZ;
    public TMP_InputField inputVelX;
    public TMP_InputField inputVelY;
    public TMP_InputField inputVelZ;

    [Header("Time Settings")]
    public TMP_InputField inputModelStep;   
    public TMP_InputField inputControlStep; 

    private bool hasStarted = false;

    void Start()
    {
        // Pre-fill boxes with default scene values immediately
        LoadCurrentValues();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        if (menuPanel.activeSelf)
        {
            ResumeSimulation();
        }
        else
        {
            dronePhysics.isSimulationRunning = false;
            menuPanel.SetActive(true);
            if (buttonText != null) buttonText.text = "RESUME";
            LoadCurrentValues();
        }
    }

    // --- RESET FUNCTION (Fixed to Reload Scene) ---
    public void ResetSimulation()
    {
        // This reloads the scene entirely. 
        // It puts the camera, drone, inputs, and physics exactly back 
        // to how they were when you first pressed "Play".
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ResumeSimulation()
    {
        // 1. Apply Physics Parameters (Mass, Length, Time)
        if (Parse(inputMass, out float m)) dronePhysics.mass = m;
        if (Parse(inputLength, out float l)) dronePhysics.armLength = l;
        if (Parse(inputModelStep, out float dt) && dt > 0.001f) Time.fixedDeltaTime = dt;

        // 2. Teleport (Only if user typed something new or it's the first start)
        ApplyTeleport();

        // 3. Start/Resume
        dronePhysics.isSimulationRunning = true;
        menuPanel.SetActive(false);
        hasStarted = true;
    }

    void ApplyTeleport()
    {
        Vector3 currentPos = dronePhysics.transform.position;
        float px = currentPos.x; 
        float py = currentPos.y; 
        float pz = currentPos.z; 

        if (Parse(inputPosX, out float x)) px = x;
        if (Parse(inputPosY, out float y)) py = y;
        if (Parse(inputPosZ, out float z)) pz = z;

        float vx = 0, vy = 0, vz = 0;
        if (Parse(inputVelX, out float ivx)) vx = ivx;
        if (Parse(inputVelY, out float ivy)) vy = ivy;
        if (Parse(inputVelZ, out float ivz)) vz = ivz;

        // Only teleport if we haven't started yet OR if user explicitly typed coordinates
        if (!hasStarted)
        {
             dronePhysics.SetInitialState(new Vector3(px, py, pz), new Vector3(vx, vy, vz));
        }
    }

    void LoadCurrentValues()
    {
        // Visual Labels + Values
        if(inputMass) inputMass.text = $"Mass (kg): {dronePhysics.mass}";
        if(inputLength) inputLength.text = $"Length (m): {dronePhysics.armLength}";
        if(inputModelStep) inputModelStep.text = $"Time Step: {Time.fixedDeltaTime}";

        Vector3 pos = dronePhysics.transform.position;
        if(inputPosX) inputPosX.text = $"Pos X: {pos.x:F2}";
        if(inputPosY) inputPosY.text = $"Pos Y: {pos.y:F2}";
        if(inputPosZ) inputPosZ.text = $"Pos Z: {pos.z:F2}";

        Vector3 vel = dronePhysics.vel;
        if(inputVelX) inputVelX.text = $"Vel X: {vel.x:F2}";
        if(inputVelY) inputVelY.text = $"Vel Y: {vel.y:F2}";
        if(inputVelZ) inputVelZ.text = $"Vel Z: {vel.z:F2}";
    }

    // Smart Parse: Ignores text like "Mass:" and finds the number
    bool Parse(TMP_InputField input, out float result)
    {
        result = 0;
        if (input != null && !string.IsNullOrEmpty(input.text))
        {
            Match match = Regex.Match(input.text, @"[-+]?[0-9]*\.?[0-9]+");
            if (match.Success) return float.TryParse(match.Value, out result);
        }
        return false;
    }
}