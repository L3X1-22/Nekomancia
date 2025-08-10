using UnityEngine;

public class CloseFight : MonoBehaviour
{
    public GameObject blurVolume;
    public GameObject canvasCombate;
    public void TerminarCombate()
    {
        Time.timeScale = 1f;
        canvasCombate.SetActive(false);
        blurVolume.SetActive(false);
    }
}
