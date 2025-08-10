using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInicio : MonoBehaviour
{

    public GameObject quitar;
    public GameObject howtoplay;
    public GameObject boton;


    public void Game()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void HowToPlay()
    {
        quitar.SetActive(false);
        howtoplay.SetActive(true);
        boton.SetActive(true);
    }

    public void Resume()
    {
        quitar.SetActive(true);
        howtoplay.SetActive(false);
        boton.SetActive(false);
    }

    public void Exit()
    {
        Debug.Log("Saliendo...");
        Application.Quit();
    }
}
