using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInicio : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Game()
    {
        SceneManager.LoadScene("SampleScene");
    }

    // Update is called once per frame
    public void Exit()
    {
        Debug.Log("Saliendo...");
        Application.Quit();
    }
}
