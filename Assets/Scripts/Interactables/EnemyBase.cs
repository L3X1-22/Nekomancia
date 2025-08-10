using UnityEngine;

public class EnemyBase : MonoBehaviour, IInteractable
{
    public CombatManager combatManager; 
    public GameObject blurVolume;
    public GameObject interactUI;
    public int enemyID;
    public void Interact()
    {
        //pausar el mundo atras
        interactUI.SetActive(false);
        blurVolume.SetActive(true);
        Time.timeScale = 0f;
        // Activar combate con datos del enemigo
        combatManager.IniciarCombate(enemyID);
    }
}
