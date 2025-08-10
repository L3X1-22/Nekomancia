using UnityEngine;

public class Force16By9 : MonoBehaviour
{
    void Start()
    {
        // Forzar relación de aspecto
        float targetRatio = 16f / 9f;
        float currentRatio = (float)Screen.width / Screen.height;
        
        if (currentRatio != targetRatio)
        {
            Screen.SetResolution(
                Mathf.RoundToInt(Screen.height * targetRatio), 
                Screen.height, 
                Screen.fullScreen
            );
        }
    }
}