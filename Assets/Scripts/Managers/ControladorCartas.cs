using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorCartas : MonoBehaviour
{
    private Getinfo getInfo;
    //private Combat combat;
    private List<int> deck = new List<int>();
    private int cantidad = 0;
    public GameObject demonio;
    public GameObject portal;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        getInfo = GetComponent<Getinfo>();
        if (getInfo != null)
        {
            Debug.Log("Getinfo encontrado correctamente.");
        }
        else
        {
            Debug.Log("Getinfo no encontrado correctamente.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        LoadDeck();
        cantidad = deck.Count;
        if (cantidad >= 3)
        {
            demonio.SetActive(true);
            portal.SetActive(true);
            //Debug.Log("HEMOS MATADO A 3");
        }
    }

    private void LoadDeck()
    {
        deck.Clear();
        deck.Add(1); // Carta "El Loco" siempre presente

        var defeatedEnemies = getInfo.db.Query<Getinfo.progreso>("SELECT * FROM progreso");
        foreach (var enemy in defeatedEnemies)
        {
            var enemigos = getInfo.db.Query<Getinfo.enemigos>("SELECT * FROM enemigos WHERE id = ?", enemy.enemigo_id);
            if (enemigos.Count > 0)
                deck.Add(enemigos[0].carta_id);
        }
    }
}
