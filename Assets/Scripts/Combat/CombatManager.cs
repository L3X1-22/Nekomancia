using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
using UnityEngine;
using UnityEngine.UI;


public class CombatManager : MonoBehaviour
{
    private Getinfo getInfo;
    private Combat combat;
    private CloseFight closeFight;
    public GameObject canvasCombate;
    public Image spriteProtagonista;
    public Image spriteEnemigo;
    string[] datosEnemigo;

    void Start()
    {
        // Obtener referencia automáticamente
        getInfo = GetComponent<Getinfo>();
        closeFight = GetComponent<CloseFight>();
        combat = GetComponent<Combat>();
    }
    public void IniciarCombate(int enemy)
    {
        datosEnemigo = getInfo.CargarDatosEnemigoConCarta(enemy);
        Debug.Log("combate iniciado");
        canvasCombate.SetActive(true);

        combat.StartCombat(datosEnemigo);
        closeFight.TerminarCombate();
        Debug.Log("Combate terminado");
    }
}