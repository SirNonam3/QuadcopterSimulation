using UnityEngine;
using sc.terrain.vegetationspawner; // Required for the Vegetation Spawner asset

public class WorldGenerator : MonoBehaviour
{
    // --- REFERENCES ---
    [Header("References")]
    public Terrain terrain;  // This was duplicated before! Now it's just once.
    public VegetationSpawner vegetationSpawner; // Drag ProceduralTerrain here

    // --- TERRAIN SETTINGS ---
    [Header("Shape Settings")]
    [Tooltip("How wide the hills are. Lower (3-5) = Wider.")]
    public float scale = 4.0f;       
    
    [Tooltip("Actual max height in meters. Lower this to 30-50 for low hills.")]
    public float maxTerrainHeight = 50.0f; 
    
    [Tooltip("Higher number (3-5) = More flat plains.")]
    public float flatness = 4.0f;    

    [Header("Safe Zone (Drone Landing)")]
    [Tooltip("Radius in meters around (0,0) that is perfectly flat.")]
    public float spawnRadius = 150.0f; 
    
    [Header("Generation Seed")]
    public float offsetX = 100f;     
    public float offsetY = 100f;

    // --- FUNCTIONS ---

    public void RandomizeAndGenerate()
    {
        // 1. Randomize the Terrain Shape
        offsetX = Random.Range(0f, 9999f);
        offsetY = Random.Range(0f, 9999f);
        GenerateTerrain(); 

        // 2. Trigger Vegetation Respawn (Using the Asset Store Tool)
        if (vegetationSpawner != null)
        {
            // A. The terrain shape changed, so we MUST rebuild the collision cache 
            // otherwise trees might spawn floating in the air or underground.
            vegetationSpawner.RebuildCollisionCache();

            // B. Force the trees to respawn on the new ground
            vegetationSpawner.Respawn(true, true); // (Grass=true, Trees=true)
        }
    }

    void GenerateTerrain()
    {
        if (terrain == null) return;
        
        // 1. Force the Terrain Size to be flatter
        TerrainData data = terrain.terrainData;
        Vector3 size = data.size;
        size.y = maxTerrainHeight; // Apply the new max height
        data.size = size;

        int width = data.heightmapResolution;
        int height = data.heightmapResolution;
        float[,] heights = new float[width, height];
        
        // Center in pixel coordinates (assuming terrain is centered at 0,0 world space)
        Vector2 center = new Vector2(width / 2f, height / 2f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Noise Calculation
                float xCoord = (float)x / width * scale + offsetX;
                float yCoord = (float)y / height * scale + offsetY;
                float noise = Mathf.PerlinNoise(xCoord, yCoord);

                // Flatten valleys (Power function)
                float yVal = Mathf.Pow(noise, flatness);

                // Safe Zone Logic (Dig a hole at 0,0)
                float distPixels = Vector2.Distance(new Vector2(x, y), center);
                // Convert pixels to meters approx
                float distMeters = distPixels * (data.size.x / width);

                if (distMeters < spawnRadius)
                {
                    yVal = 0; // Flat floor
                }
                else if (distMeters < spawnRadius + 50f)
                {
                    // Smooth blend up
                    float blend = (distMeters - spawnRadius) / 50f;
                    yVal *= blend;
                }

                heights[x, y] = yVal;
            }
        }
        data.SetHeights(0, 0, heights);
    }
}