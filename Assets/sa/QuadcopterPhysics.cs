using UnityEngine;
using System.Collections.Generic;

public class QuadcopterPhysics : MonoBehaviour
{
    // --- 1. VARIABLES ---
    [Header("Model Parameters")]
    public float mass = 1.0f;
    public float armLength = 0.25f; 
    public float gravity = 9.81f;
    public float dragCoefficient = 0.5f; 

    [Header("Controls (Mode 2)")]
    public float thrustPower = 30.0f;  
    public float rotationSpeed = 2.0f; // Angular velocity sensitivity
    public float maxTiltAngle = 25.0f; 

    [Header("Collision")]
    public LayerMask obstacleLayer;

    // Public State (For UI)
    public Vector3 vel; 
    
    // Private Physics State
    private Vector3 pos; 
    private Vector3 acc; 
    
    // Euler Angles (Radians)
    private float phi;   // Roll
    private float theta; // Pitch
    private float psi;   // Yaw

    // Body Rates (p, q, r from Maple file)
    private float p; 
    private float q; 
    private float r; 

    // UI History Log
    public List<string> historyLog = new List<string>();
    private float logTimer = 0f;
    
    [HideInInspector]
    public bool isSimulationRunning = false;

    // --- 2. SETUP ---
    void Start()
    {
        pos = transform.position;
        vel = Vector3.zero;
        acc = Vector3.zero;
        
        // Initialize Yaw from current rotation
        Vector3 currentEuler = transform.eulerAngles;
        psi = currentEuler.y * Mathf.Deg2Rad;
        theta = 0;
        phi = 0;
        
        if (obstacleLayer == 0) obstacleLayer = 1;
    }

    public void SetInitialState(Vector3 startPos, Vector3 startVel)
    {
        pos = startPos;
        vel = startVel;
        transform.position = pos;
    }

    // --- 3. PHYSICS LOOP ---
    void FixedUpdate()
    {
        if (!isSimulationRunning) return;

        float dt = Time.fixedDeltaTime;

        // A. INPUTS (Body Rates p, q, r)
        float inputThrust = 0f;
        if (Input.GetKey(KeyCode.W)) inputThrust = 1.0f;
        if (Input.GetKey(KeyCode.S)) inputThrust = -1.0f;
        
        float inputYaw = 0f;
        if (Input.GetKey(KeyCode.D)) inputYaw = 1.0f;
        if (Input.GetKey(KeyCode.A)) inputYaw = -1.0f;

        float inputPitch = 0f;
        if (Input.GetKey(KeyCode.Keypad8)) inputPitch = 1.0f; 
        if (Input.GetKey(KeyCode.Keypad5)) inputPitch = -1.0f;

        float inputRoll = 0f;
        if (Input.GetKey(KeyCode.Keypad4)) inputRoll = -1.0f; 
        if (Input.GetKey(KeyCode.Keypad6)) inputRoll = 1.0f;  

        // Map inputs to Angular Velocity
        p = -inputRoll * rotationSpeed;
        q = inputPitch * rotationSpeed;
        r = inputYaw * rotationSpeed;

        // Stability: Return to level if no input
        if (inputRoll == 0) p -= phi * 5.0f; 
        if (inputPitch == 0) q -= theta * 5.0f;

        // B. KINEMATICS (The Instructor's Math + Stability Assist)
        float sinPhi = Mathf.Sin(phi);
        float cosPhi = Mathf.Cos(phi);
        float tanTheta = Mathf.Tan(theta);
        float secTheta = 1.0f / Mathf.Cos(theta); 

        // 1. Calculate Real Physics (from Maple File Tm1)
        float dPhi = p + (q * sinPhi * tanTheta) + (r * cosPhi * tanTheta);
        float dTheta = (q * cosPhi) - (r * sinPhi);
        float dPsi_Real = (q * sinPhi * secTheta) + (r * cosPhi * secTheta);

        // 2. Apply Stability Assist
        // If the user isn't actively turning (r is near 0), ignore the induced turn.
        // If the user IS turning, use the full physics formula.
        float dPsi = 0;
        if (Mathf.Abs(r) > 0.01f)
        {
            dPsi = dPsi_Real;
        }

        // Integrate
        phi += dPhi * dt;
        theta += dTheta * dt;
        psi += dPsi * dt;

        // Clamp
        float maxRad = maxTiltAngle * Mathf.Deg2Rad;
        phi = Mathf.Clamp(phi, -maxRad, maxRad);
        theta = Mathf.Clamp(theta, -maxRad, maxRad);

        // C. FORCES
        float tiltFactor = Mathf.Cos(phi) * Mathf.Cos(theta);
        if (tiltFactor < 0.2f) tiltFactor = 0.2f;
        float totalThrust = (mass * gravity) / tiltFactor;

        if (inputThrust != 0) totalThrust += inputThrust * thrustPower;
        else totalThrust -= vel.y * 5.0f; 

        // Rotation Matrices (Body -> World)
        float bodyFx = -Mathf.Sin(phi) * totalThrust; 
        float bodyFz = Mathf.Sin(theta) * totalThrust;  
        float bodyFy = Mathf.Cos(phi) * Mathf.Cos(theta) * totalThrust; 

        float Fx = bodyFx * Mathf.Cos(psi) + bodyFz * Mathf.Sin(psi);
        float Fz = bodyFz * Mathf.Cos(psi) - bodyFx * Mathf.Sin(psi);
        float Fy = bodyFy - (mass * gravity); 

        // Drag
        Fx -= dragCoefficient * vel.x;
        Fz -= dragCoefficient * vel.z;
        Fy -= dragCoefficient * vel.y;

        // D. INTEGRATION
        acc = new Vector3(Fx/mass, Fy/mass, Fz/mass);
        Vector3 nextVel = vel + acc * dt;
        Vector3 nextPos = pos + nextVel * dt;

        // E. COLLISION
        Vector3 moveDir = (nextPos - pos).normalized;
        float dist = Vector3.Distance(pos, nextPos);
        if (dist > 0.001f)
        {
            if (Physics.SphereCast(pos, 0.2f, moveDir, out RaycastHit hit, dist, obstacleLayer))
            {
                if (hit.collider.transform.root != transform.root)
                {
                    nextPos = pos;
                    nextVel = Vector3.zero;
                }
            }
        }

        pos = nextPos;
        vel = nextVel;

        if (pos.y < 0) { pos.y = 0; vel.y = 0; phi = 0; theta = 0; }

        // F. APPLY TO UNITY
        transform.position = pos;
        transform.rotation = Quaternion.Euler(theta * Mathf.Rad2Deg, psi * Mathf.Rad2Deg, phi * Mathf.Rad2Deg);

        // G. LOGGING
        logTimer += dt;
        if (logTimer > 0.1f)
        {
            if(historyLog.Count >= 10) historyLog.RemoveAt(0);
            historyLog.Add($"{Time.time:F1}s | Alt:{pos.y:F1} | Spd:{vel.magnitude:F1}");
            logTimer = 0;
        }
    }
}