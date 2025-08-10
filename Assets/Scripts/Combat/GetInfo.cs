using System.IO;
using UnityEngine;
using SQLite;
using System;
using System.Collections.Generic;

public class Getinfo : MonoBehaviour
{
    public SQLiteConnection db;

    // Clases para la base de datos (mantener públicas)
    public class enemigos
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public int carta_id { get; set; }
        public string frase { get; set; }
    }

    public class cartas
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public int set_mov_normales { get; set; }
        public int set_mov_invertidos { get; set; }
    }

    public class set_movimientos
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public int mov1 { get; set; }
        public int mov2 { get; set; }
        public int mov3 { get; set; }
    }

    public class movimientos
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }

        [NotNull]
        public string nombre { get; set; }

        public int fuerte_contra { get; set; }

        public int debil_contra { get; set; }
    }


    public class progreso
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public int enemigo_id { get; set; }
        public string fecha_derrota { get; set; }
    }

    void Awake()
    {
        // Ruta al archivo de la DB dentro del dispositivo
        string persistentPath = Path.Combine(Application.persistentDataPath, "cartas.db");

        if (!File.Exists(persistentPath))
        {
            // En desarrollo: copiar desde StreamingAssets
            string streamingPath = Path.Combine(Application.streamingAssetsPath, "cartas.db");
            File.Copy(streamingPath, persistentPath);
        }

        db = new SQLiteConnection(persistentPath);
    }

    public string[] CargarDatosEnemigoConCarta(int enemyID)
    {
        // Buscar enemigo
        var enemigo = db.Table<enemigos>().Where(e => e.id == enemyID).FirstOrDefault();

        if (enemigo == null)
        {
            Debug.LogError($"No se encontró enemigo con ID: {enemyID}");
            return new string[] { "", "" };
        }

        // Buscar carta del enemigo
        var carta = db.Table<cartas>().Where(c => c.id == enemigo.carta_id).FirstOrDefault();

        if (carta == null)
        {
            Debug.LogError($"No se encontró carta con ID: {enemigo.carta_id} para el enemigo {enemyID}");
            return new string[] { enemigo.frase, enemigo.carta_id.ToString() };
        }

        // Devolver array con frase e ID de la carta
        return new string[] { enemigo.frase, enemigo.carta_id.ToString() };
    }

    // Nuevo método para obtener cartas del jugador
    public List<int> ObtenerMazoJugador()
    {
        List<int> mazo = new List<int>();

        var progreso = db.Table<progreso>().ToList();
        foreach (var p in progreso)
        {
            var enemigo = db.Table<enemigos>().Where(e => e.id == p.enemigo_id).FirstOrDefault();
            if (enemigo != null && !mazo.Contains(enemigo.carta_id))
            {
                mazo.Add(enemigo.carta_id);
            }
        }

        return mazo;
    }
}