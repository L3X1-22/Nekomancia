using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class Combat : MonoBehaviour
{
    // UI
    [Header("UI Textos")]
    public TMP_Text playerHealthText;
    public TMP_Text enemyHealthText;
    public TMP_Text enemyDialogueText;

    [Header("UI Imágenes")]
    public Image playerCardImage;
    public Image enemyCardImage;

    [Header("Botones")]
    public Button[] attackButtons = new Button[3];
    public Button invertButton;
    public Button changeCardButton;

    // Base de datos
    private Getinfo getInfo;

    // Datos de combate
    private int playerHealth;
    private int enemyHealth;
    private int playerCardId;
    private int enemyCardId;
    private int enemyID;
    private bool isPlayerCardInverted;

    //info para cerrar la pelea
/*     public GameObject blurVolume;
    public GameObject canvasCombate;
 */
    private List<int> playerDeck = new List<int>();
    private List<int> currentPlayerMoves = new List<int>();
    private string enemyPhrase;

    public class TablaInfo
    {
        public string name { get; set; }
    }

    void Start()
    {
        getInfo = GetComponent<Getinfo>();
        if (getInfo == null)
        {
            Debug.LogError("No se encontró componente Getinfo en este GameObject");
            return;
        }

        // Eventos de botones
        invertButton.onClick.AddListener(InvertCard);
        changeCardButton.onClick.AddListener(ChangeCard);

        for (int i = 0; i < attackButtons.Length; i++)
        {
            int index = i;
            attackButtons[i].onClick.AddListener(() => Attack(index));
        }
    }

    public void StartCombat(string[] datosCombate)
    {
        if (datosCombate.Length < 2)
        {
            Debug.LogError("Datos de combate inválidos");
            return;
        }

        enemyPhrase = datosCombate[0];
        if (!int.TryParse(datosCombate[1], out enemyID))
        {
            Debug.LogError("ID de enemigo inválido");
            return;
        }

        var enemigos = getInfo.db.Query<Getinfo.enemigos>("SELECT * FROM enemigos WHERE id = ?", enemyID);
        if (enemigos.Count > 0)
            enemyCardId = enemigos[0].carta_id;
        else
            Debug.LogError("No se encontró enemigo con ID " + enemyID);

        playerHealth = 100;
        enemyHealth = 100;

       /*  //para depurar uwu
        var tablas = getInfo.db.Query<Getinfo.movimientos>("SELECT * FROM movimientos");
        foreach (var t in tablas) Debug.Log(t.nombre); */

        LoadPlayerDeck();
        SelectRandomPlayerCard();
        SetupEnemyCard();
        UpdateUI();

        Debug.Log("¡Combate iniciado!");
    }

    private void LoadPlayerDeck()
    {
        playerDeck.Clear();
        playerDeck.Add(1); // Carta "El Loco" siempre presente

        var defeatedEnemies = getInfo.db.Query<Getinfo.progreso>("SELECT * FROM progreso");
        foreach (var enemy in defeatedEnemies)
        {
            var enemigos = getInfo.db.Query<Getinfo.enemigos>("SELECT * FROM enemigos WHERE id = ?", enemy.enemigo_id);
            if (enemigos.Count > 0)
                playerDeck.Add(enemigos[0].carta_id);
        }
    }

    private void SelectRandomPlayerCard()
    {
        if (playerDeck.Count == 0) return;

        playerCardId = playerDeck[Random.Range(0, playerDeck.Count)];
        isPlayerCardInverted = Random.Range(0, 2) == 1;
        LoadPlayerMoves();
    }

    private void LoadPlayerMoves()
    {
        currentPlayerMoves.Clear();

        var cartas = getInfo.db.Query<Getinfo.cartas>("SELECT * FROM cartas WHERE id = ?", playerCardId);
        if (cartas.Count == 0) return;

        var carta = cartas[0];
        int setId = isPlayerCardInverted ? carta.set_mov_invertidos : carta.set_mov_normales;
        
        var sets = getInfo.db.Query<Getinfo.set_movimientos>("SELECT * FROM set_movimientos WHERE id = ?", setId);
        if (sets.Count > 0)
        {
            var set = sets[0];
            currentPlayerMoves.AddRange(new[] { set.mov1, set.mov2, set.mov3 });
        }

        for (int i = 0; i < attackButtons.Length; i++)
        {
            if (i < currentPlayerMoves.Count)
            {
                var movimientos = getInfo.db.Query<Getinfo.movimientos>("SELECT * FROM movimientos WHERE id = ?", currentPlayerMoves[i]);
                if (movimientos.Count > 0)
                {
                    var move = movimientos[0];
                    var btnText = attackButtons[i].GetComponentInChildren<TMP_Text>();
                    if (btnText != null)
                        btnText.text = move.nombre;

                    attackButtons[i].gameObject.SetActive(true);
                }
            }
            else
            {
                attackButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void SetupEnemyCard()
    {
        if (enemyDialogueText != null)
            enemyDialogueText.text = enemyPhrase;

       /*  if (enemyCardImage != null)
        {
            Sprite cardSprite = Resources.Load<Sprite>($"Cartas/{enemyCardId}");
            if (cardSprite != null)
                enemyCardImage.sprite = cardSprite;
            else
                Debug.LogWarning($"No se encontró sprite para la carta {enemyCardId}");
        } */
    }

    public void InvertCard()
    {
        isPlayerCardInverted = !isPlayerCardInverted;
        LoadPlayerMoves();

        if (playerCardImage != null)
            playerCardImage.transform.rotation = Quaternion.Euler(0, 0, isPlayerCardInverted ? 180 : 0);
    }

    public void ChangeCard()
    {
        SelectRandomPlayerCard();

        if (playerCardImage != null)
        {
            Sprite cardSprite = Resources.Load<Sprite>($"Cartas/{playerCardId}");
            if (cardSprite != null)
                playerCardImage.sprite = cardSprite;
        }
    }

    public void Attack(int moveIndex)
    {
        if (moveIndex < 0 || moveIndex >= currentPlayerMoves.Count) return;

        int moveId = currentPlayerMoves[moveIndex];
        var movimientos = getInfo.db.Query<Getinfo.movimientos>("SELECT * FROM movimientos WHERE id = ?", moveId);
        if (movimientos.Count == 0) return;

        var movimiento = movimientos[0];
        int damage = CalculateDamage(movimiento);
        enemyHealth -= damage;

        if (enemyHealth <= 0)
        {
            enemyHealth = 0;
            EndCombat(true);
            return;
        }

        EnemyTurn();
        UpdateUI();
    }

    private int CalculateDamage(Getinfo.movimientos movimiento)
    {
        var cartas = getInfo.db.Query<Getinfo.cartas>("SELECT * FROM cartas WHERE id = ?", enemyCardId);
        if (cartas.Count == 0) return 20;

        var cartaEnemiga = cartas[0];
        if (movimiento.fuerte_contra == cartaEnemiga.id) return 40;
        if (movimiento.debil_contra == cartaEnemiga.id) return 10;
        return 20;
    }

    private void EnemyTurn()
    {
        var cartas = getInfo.db.Query<Getinfo.cartas>("SELECT * FROM cartas WHERE id = ?", enemyCardId);
        if (cartas.Count == 0) return;

        var cartaEnemiga = cartas[0];
        var sets = getInfo.db.Query<Getinfo.set_movimientos>("SELECT * FROM set_movimientos WHERE id = ?", cartaEnemiga.set_mov_normales);
        if (sets.Count == 0) return;

        var setEnemigo = sets[0];
        List<int> enemyMoves = new List<int> { setEnemigo.mov1, setEnemigo.mov2, setEnemigo.mov3 };
        int randomMoveId = enemyMoves[Random.Range(0, enemyMoves.Count)];
        
        var movimientos = getInfo.db.Query<Getinfo.movimientos>("SELECT * FROM movimientos WHERE id = ?", randomMoveId);

        int damage = 20;
        if (movimientos.Count > 0)
        {
            var movimientoEnemigo = movimientos[0];
            var cartasJugador = getInfo.db.Query<Getinfo.cartas>("SELECT * FROM cartas WHERE id = ?", playerCardId);
            if (cartasJugador.Count > 0)
            {
                if (movimientoEnemigo.fuerte_contra == playerCardId) damage = 40;
                else if (movimientoEnemigo.debil_contra == playerCardId) damage = 10;
            }
        }
        
        playerHealth -= damage;

        if (playerHealth <= 0)
        {
            playerHealth = 0;
            EndCombat(false);
        }
    }

    private void UpdateUI()
    {
        if (playerHealthText != null)
            playerHealthText.text = $"Salud Jugador: {playerHealth}";
        if (enemyHealthText != null)
            enemyHealthText.text = $"Salud Enemigo: {enemyHealth}";
    }

    private void EndCombat(bool playerWon)
    {

        if (playerWon)
        {
            Debug.Log("¡Victoria!");
            var nuevoProgreso = new Getinfo.progreso
            {
                enemigo_id = enemyID,
                fecha_derrota = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            getInfo.db.Insert(nuevoProgreso);
        }
        else
        {
            Debug.Log("Derrota...");
        }

        var closefight = GetComponent<CloseFight>();
        closefight.TerminarCombate();
    }
}