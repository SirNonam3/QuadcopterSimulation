using UnityEngine;

public class CloudScroller : MonoBehaviour
{
    public float windSpeedX = 0.5f;
    public float windSpeedZ = 0.2f;
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        // Moves the texture offset to simulate wind
        float offsetX = Time.time * windSpeedX * 0.01f;
        float offsetZ = Time.time * windSpeedZ * 0.01f;
        rend.material.mainTextureOffset = new Vector2(offsetX, offsetZ);
    }
}