using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInicio : MonoBehaviour
{

    public GameObject quitar;
    public GameObject howtoplay;
    public GameObject galeria;


    public void Game()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void HowToPlay()
    {
        quitar.SetActive(false);
        howtoplay.SetActive(true);
    }

    public void Galeria()
    {
        quitar.SetActive(false);
        galeria.SetActive(true);
    }

    public void Resume()
    {
        quitar.SetActive(true);
        howtoplay.SetActive(false);
        galeria.SetActive(false);
    }

    public void Exit()
    {
        Debug.Log("Saliendo...");
        Application.Quit();
    }
}
