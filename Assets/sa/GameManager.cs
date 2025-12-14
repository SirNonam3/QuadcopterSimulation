using UnityEngine;
using TMPro; 

public class GameManager : MonoBehaviour
{
    // --- VARIABLES (Must be here, at the top) ---
    public QuadcopterPhysics dronePhysics;
    public GameObject menuPanel;

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

    // --- FUNCTIONS (Must be below the variables) ---

    public void StartSimulation()
    {
        // 1. Set Characteristics
        // We check if the input is not empty, then parse it
        if (Parse(inputMass, out float m)) dronePhysics.mass = m;
        
        // Note: You must add 'armLength' to QuadcopterPhysics.cs for this line to work!
        if (Parse(inputLength, out float l)) dronePhysics.armLength = l;

        // 2. Set Initial Position
        float px = 0, py = 0, pz = 0;
        Parse(inputPosX, out px);
        Parse(inputPosY, out py);
        Parse(inputPosZ, out pz);

        // 3. Set Initial Velocity
        float vx = 0, vy = 0, vz = 0;
        Parse(inputVelX, out vx);
        Parse(inputVelY, out vy);
        Parse(inputVelZ, out vz);

        // Apply to Drone (Requires SetInitialState method in Physics script)
        dronePhysics.SetInitialState(new Vector3(px, py, pz), new Vector3(vx, vy, vz));

        // 4. Set Time Steps
        // "Time step for modeling" -> Unity's FixedDeltaTime
        if (Parse(inputModelStep, out float dt))
        {
            // Safety check: Don't let time step be 0 or negative
            if (dt > 0.001f) Time.fixedDeltaTime = dt; 
        }

        // 5. Start the Engine
        dronePhysics.isSimulationRunning = true;
        menuPanel.SetActive(false);
    }

    // Helper function to read text safely
    bool Parse(TMP_InputField input, out float result)
    {
        result = 0;
        if (input != null && input.text.Length > 0)
        {
            return float.TryParse(input.text, out result);
        }
        return false;
    }
}