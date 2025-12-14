using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    [Header("Sky Settings")]
    public Material[] skyboxes;        
    public Light directionalLight;     

    [Header("Ground Settings")]
    public GameObject[] worlds;        

    private int currentIndex = 0;

    void Start()
    {
        Debug.Log("Environment Manager Started.");
        if (skyboxes.Length == 0 || worlds.Length == 0)
        {
            Debug.LogError("ERROR: You forgot to assign Skyboxes or Worlds in the Inspector!");
        }
        
        UpdateEnvironment(0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log("Tab Pressed! Switching...");
            currentIndex++;
            // We don't reset to 0 here anymore. We just keep counting up.
            // The logic below handles the looping using the % (modulo) operator.
            UpdateEnvironment(currentIndex);
        }
    }

    void UpdateEnvironment(int index)
    {
        // --- 1. CHANGE SKY ---
        if (skyboxes.Length > 0)
        {
            // The % operator loops the index safely. 
            // If index is 5 and length is 5, result is 0.
            int skyIndex = index % skyboxes.Length; 
            RenderSettings.skybox = skyboxes[skyIndex];
            DynamicGI.UpdateEnvironment(); 
        }

        // --- 2. CHANGE GROUND ---
        if (worlds.Length > 0)
        {
            int worldIndex = index % worlds.Length; // Loops worlds safely (0, 1, 0, 1...)

            for (int i = 0; i < worlds.Length; i++)
            {
                // Only turn on the one that matches our looped index
                bool shouldBeActive = (i == worldIndex);
                if (worlds[i] != null)
                {
                    worlds[i].SetActive(shouldBeActive);
                }
            }
        }

        // --- 3. ADJUST SUN ---
        if (directionalLight != null && skyboxes.Length > 0)
        {
            int skyIndex = index % skyboxes.Length;
            string skyName = skyboxes[skyIndex].name.ToLower();
            
            if (skyName.Contains("night") || skyName.Contains("dark"))
            {
                directionalLight.intensity = 0.2f; 
                directionalLight.color = new Color(0.4f, 0.4f, 0.7f);
            }
            else
            {
                directionalLight.intensity = 1.0f; 
                directionalLight.color = Color.white;
            }
        }
    }
}