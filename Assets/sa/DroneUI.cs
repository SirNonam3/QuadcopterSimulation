using UnityEngine;
using TMPro;

public class DroneUI : MonoBehaviour
{
    public QuadcopterPhysics dronePhysics; // Link to the physics script
    public TextMeshProUGUI hudText;        // Link to the NEW HUD Text object

    void Update()
    {
        if (dronePhysics == null || hudText == null) return;

        // --- 1. SYSTEM PARAMETERS (The Instructor's Requirement) ---
        // We read these straight from the physics script, where GameManager saved them.
        string display = "<align=left><b>SYSTEM CONFIG</b>\n";
        display += $"Mass: {dronePhysics.mass} kg\n";
        display += $"Arm Length: {dronePhysics.armLength} m\n"; 
        display += $"Time Step: {Time.fixedDeltaTime:F3} s\n"; 
        display += "--------------------------\n";

        // --- 2. LIVE TELEMETRY ---
        display += "<b>LIVE DATA</b>\n";
        // Note: Make sure 'vel' is public in QuadcopterPhysics for this to work!
        display += $"Alt: {dronePhysics.transform.position.y:F1} m\n";
        // Calculate speed manually if 'vel' is private, or make 'vel' public
        display += $"Speed: {GetSpeed():F1} m/s\n";
        display += "--------------------------\n";

        // --- 3. FLIGHT LOG ---
        display += "<b>HISTORY (Last 10)</b>\n";
        display += "<size=80%>Time | Alt | Spd</size>\n";
        
        foreach (string logEntry in dronePhysics.historyLog)
        {
            display += logEntry + "\n";
        }

        hudText.text = display;
    }

    float GetSpeed()
    {
        // Requires 'public Vector3 vel' in QuadcopterPhysics.cs
        return dronePhysics.vel.magnitude; 
    }
}