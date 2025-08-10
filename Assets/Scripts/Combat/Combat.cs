using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [Header("Configuración de Mensajes")]
    public float messageDisplayTime = 2f;

    // Base de datos
    private Getinfo getInfo;

    // Datos de combate
    private int playerHealth;
    private int enemyHealth;
    private int playerCardId;
    private int enemyCardId;
    private int enemyID;
    private bool isPlayerCardInverted;

    private List<int> playerDeck = new List<int>();
    private List<int> currentPlayerMoves = new List<int>();
    private string enemyPhrase;
    private bool isPlayerTurn = true;

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
        invertButton.onClick.AddListener(() => StartCoroutine(HandleInvertCard()));
        changeCardButton.onClick.AddListener(() => StartCoroutine(HandleChangeCard()));

        for (int i = 0; i < attackButtons.Length; i++)
        {
            int index = i;
            attackButtons[i].onClick.AddListener(() => StartCoroutine(HandleAttack(index)));
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

        LoadPlayerDeck();
        SelectRandomPlayerCard();
        SetupEnemyCard();
        UpdateUI();
        
        StartCoroutine(ShowInitialMessage());

        Debug.Log("¡Combate iniciado!");
    }

    private IEnumerator ShowInitialMessage()
    {
        DisableAllButtons();
        
        if (enemyDialogueText != null)
            enemyDialogueText.text = "¡El combate ha comenzado! Elige tu movimiento.";
        
        yield return new WaitForSecondsRealtime(messageDisplayTime);
        
        if (enemyDialogueText != null)
            enemyDialogueText.text = enemyPhrase;
            
        EnableAllButtons();
        isPlayerTurn = true;
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
        UpdatePlayerCardUI();
    }

    private void UpdatePlayerCardUI()
    {
        if (playerCardImage != null)
        {
            Sprite cardSprite = LoadCardSprite(playerCardId);
            if (cardSprite != null)
                playerCardImage.sprite = cardSprite;
            else
                Debug.LogWarning($"No se encontró sprite para la carta {playerCardId}");
                
            // Rotar la carta si está invertida (180 grados = de cabeza)
            playerCardImage.transform.rotation = Quaternion.Euler(0, 0, isPlayerCardInverted ? 180 : 0);
        }
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
        if (enemyCardImage != null)
        {
            Sprite cardSprite = LoadCardSprite(enemyCardId);
            if (cardSprite != null)
                enemyCardImage.sprite = cardSprite;
            else
                Debug.LogWarning($"No se encontró sprite para la carta {enemyCardId}");
        }
    }

    private IEnumerator HandleInvertCard()
    {
        if (!isPlayerTurn) yield break;
        
        DisableAllButtons();
        
        // Mostrar mensaje de inversión
        if (enemyDialogueText != null)
            enemyDialogueText.text = "El jugador ha invertido la carta.";
        
        yield return new WaitForSecondsRealtime(messageDisplayTime);
        
        // Ejecutar inversión
        isPlayerCardInverted = !isPlayerCardInverted;
        LoadPlayerMoves();
        UpdatePlayerCardUI();
        
        // Continuar con el turno del enemigo
        yield return StartCoroutine(EnemyTurnCoroutine());
    }

    private IEnumerator HandleChangeCard()
    {
        if (!isPlayerTurn) yield break;
        
        DisableAllButtons();
        
        // Mostrar mensaje de cambio
        if (enemyDialogueText != null)
            enemyDialogueText.text = "El jugador ha cambiado de carta.";
        
        yield return new WaitForSecondsRealtime(messageDisplayTime);
        
        // Ejecutar cambio
        SelectRandomPlayerCard();
        
        // Continuar con el turno del enemigo
        yield return StartCoroutine(EnemyTurnCoroutine());
    }

    private IEnumerator HandleAttack(int moveIndex)
    {
        if (!isPlayerTurn || moveIndex < 0 || moveIndex >= currentPlayerMoves.Count) yield break;

        DisableAllButtons();

        int moveId = currentPlayerMoves[moveIndex];
        var movimientos = getInfo.db.Query<Getinfo.movimientos>("SELECT * FROM movimientos WHERE id = ?", moveId);
        if (movimientos.Count == 0) yield break;

        var movimiento = movimientos[0];
        int damage = CalculateDamage(movimiento);
        
        // Mostrar mensaje del ataque del jugador
        string effectiveness = GetEffectivenessText(movimiento, enemyCardId);
        if (enemyDialogueText != null)
            enemyDialogueText.text = $"Has usado {movimiento.nombre}. {effectiveness}";
        
        yield return new WaitForSecondsRealtime(messageDisplayTime);
        
        enemyHealth -= damage;
        UpdateUI();

        if (enemyHealth <= 0)
        {
            enemyHealth = 0;
            UpdateUI();
            EndCombat(true);
            yield break;
        }

        yield return StartCoroutine(EnemyTurnCoroutine());
    }

    private IEnumerator EnemyTurnCoroutine()
    {
        isPlayerTurn = false;

        var cartas = getInfo.db.Query<Getinfo.cartas>("SELECT * FROM cartas WHERE id = ?", enemyCardId);
        if (cartas.Count == 0) yield break;

        var cartaEnemiga = cartas[0];
        var sets = getInfo.db.Query<Getinfo.set_movimientos>("SELECT * FROM set_movimientos WHERE id = ?", cartaEnemiga.set_mov_normales);
        if (sets.Count == 0) yield break;

        var setEnemigo = sets[0];
        List<int> enemyMoves = new List<int> { setEnemigo.mov1, setEnemigo.mov2, setEnemigo.mov3 };
        int randomMoveId = enemyMoves[Random.Range(0, enemyMoves.Count)];
        
        var movimientos = getInfo.db.Query<Getinfo.movimientos>("SELECT * FROM movimientos WHERE id = ?", randomMoveId);

        if (movimientos.Count > 0)
        {
            var movimientoEnemigo = movimientos[0];
            int damage = CalculateEnemyDamage(movimientoEnemigo);
            
            // Mostrar mensaje del ataque enemigo
            string effectiveness = GetEnemyEffectivenessText(movimientoEnemigo, playerCardId);
            if (enemyDialogueText != null)
                enemyDialogueText.text = $"El enemigo ha usado {movimientoEnemigo.nombre}. {effectiveness}";
            
            yield return new WaitForSecondsRealtime(messageDisplayTime);
            
            playerHealth -= damage;
            UpdateUI();

            if (playerHealth <= 0)
            {
                playerHealth = 0;
                UpdateUI();
                EndCombat(false);
                yield break;
            }
        }

        // Volver al turno del jugador
        if (enemyDialogueText != null)
            enemyDialogueText.text = enemyPhrase;
            
        EnableAllButtons();
        isPlayerTurn = true;
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

    private int CalculateEnemyDamage(Getinfo.movimientos movimientoEnemigo)
    {
        int damage = 20;
        var cartasJugador = getInfo.db.Query<Getinfo.cartas>("SELECT * FROM cartas WHERE id = ?", playerCardId);
        if (cartasJugador.Count > 0)
        {
            if (movimientoEnemigo.fuerte_contra == playerCardId) damage = 40;
            else if (movimientoEnemigo.debil_contra == playerCardId) damage = 10;
        }
        return damage;
    }

    private string GetEffectivenessText(Getinfo.movimientos movimiento, int targetCardId)
    {
        var cartas = getInfo.db.Query<Getinfo.cartas>("SELECT * FROM cartas WHERE id = ?", targetCardId);
        if (cartas.Count == 0) return "Es efectivo.";

        var cartaObjetivo = cartas[0];
        if (movimiento.fuerte_contra == cartaObjetivo.id) return "¡Es muy efectivo!";
        if (movimiento.debil_contra == cartaObjetivo.id) return "Es poco efectivo...";
        return "Es efectivo.";
    }

    private string GetEnemyEffectivenessText(Getinfo.movimientos movimientoEnemigo, int playerCardId)
    {
        if (movimientoEnemigo.fuerte_contra == playerCardId) return "¡Es muy efectivo!";
        if (movimientoEnemigo.debil_contra == playerCardId) return "Es poco efectivo...";
        return "Es efectivo.";
    }

    private void DisableAllButtons()
    {
        foreach (var button in attackButtons)
            if (button != null) button.interactable = false;
        
        if (invertButton != null)
            invertButton.interactable = false;
        if (changeCardButton != null)
            changeCardButton.interactable = false;
    }

    private void EnableAllButtons()
    {
        foreach (var button in attackButtons)
            if (button != null) button.interactable = true;
        
        if (invertButton != null)
            invertButton.interactable = true;
        if (changeCardButton != null)
            changeCardButton.interactable = true;
    }

    private Sprite LoadCardSprite(int cardId)
    {
        // Consultar el nombre de la carta desde la base de datos
        var cartas = getInfo.db.Query<Getinfo.cartas>("SELECT * FROM cartas WHERE id = ?", cardId);
        if (cartas.Count == 0)
        {
            Debug.LogError($"No se encontró carta con ID {cardId} en la base de datos");
            return null;
        }

        var carta = cartas[0];
        string cardName = carta.nombre.ToLower(); // Convertir a minúsculas
        
        // Construir la ruta: Sprites/Baraja/WGJ Nekomancia [nombre]
        string spritePath = $"Sprites/Baraja/WGJ Nekomancia {cardName}";
        
        Debug.Log($"Intentando cargar sprite: {spritePath}");
        
        Sprite cardSprite = Resources.Load<Sprite>(spritePath);
        
        // Si no encontró el sprite, intentar con variaciones comunes
        if (cardSprite == null)
        {
            Debug.LogWarning($"No se encontró sprite en: {spritePath}");
            
            // Intentar sin "la/el" al inicio
            string[] variations = {
                cardName.Replace("la ", "").Replace("el ", ""),
                cardName.Replace("la sacerdotisa", "sacerdotisa"),
                cardName.Replace("el colgado", "colgado"),
                cardName.Replace("el loco", "loco"),
                cardName.Replace("la muerte", "muerte"),
                cardName.Replace("el mago", "mago"),
                cardName.Replace("la emperatriz", "emperatriz"),
                cardName.Replace("el emperador", "emperador")
            };
            
            foreach (string variation in variations)
            {
                if (variation != cardName) // Solo probar si es diferente
                {
                    string variationPath = $"Sprites/Baraja/WGJ Nekomancia {variation}";
                    Debug.Log($"Probando variación: {variationPath}");
                    cardSprite = Resources.Load<Sprite>(variationPath);
                    if (cardSprite != null)
                    {
                        Debug.Log($"¡Sprite encontrado con variación: {variationPath}");
                        break;
                    }
                }
            }
        }
        
        if (cardSprite == null)
        {
            Debug.LogError($"No se pudo cargar sprite para carta ID {cardId} (nombre: {carta.nombre})");
            Debug.LogError("Verifica que el archivo existe en Resources/Sprites/Baraja/ y que el nombre coincida");
        }
            
        return cardSprite;
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
        DisableAllButtons();

        if (playerWon)
        {
            Debug.Log("¡Victoria!");
            if (enemyDialogueText != null)
                enemyDialogueText.text = "¡Has ganado el combate!";
                
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
            if (enemyDialogueText != null)
                enemyDialogueText.text = "Has sido derrotado...";
        }

        StartCoroutine(EndCombatDelayed());
    }

    private IEnumerator EndCombatDelayed()
    {
        yield return new WaitForSecondsRealtime(messageDisplayTime);
        
        var closefight = GetComponent<CloseFight>();
        if (closefight != null)
            closefight.TerminarCombate();
        else
            Debug.LogError("No se encontró componente CloseFight");
    }
}