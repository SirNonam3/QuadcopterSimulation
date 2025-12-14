using UnityEngine;
using TMPro;

public class FixHUD : MonoBehaviour
{
    void Start()
    {
        // 1. Force the object to be the last item (draw on top)
        transform.SetAsLastSibling();

        // 2. Reset Position to the Top-Left corner
        RectTransform rect = GetComponent<RectTransform>();
        if (rect != null)
        {
            // Reset scale just in case
            rect.localScale = Vector3.one; 
            
            // Anchor to Top-Left
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            
            // Set Position (X=20, Y=-20, Z=0)
            rect.anchoredPosition3D = new Vector3(20, -20, 0);
            
            // Set Size (Big enough to see)
            rect.sizeDelta = new Vector2(500, 800);
        }

        // 3. Force Color to Red (High Contrast)
        TextMeshProUGUI tmp = GetComponent<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.color = Color.red; // Make it bright red
            tmp.text = "HUD IS WORKING"; // Debug text
        }
        
        Debug.Log("FixHUD script ran on: " + gameObject.name);
    }
}