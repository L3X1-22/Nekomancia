using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactRange = 0.7f; // Distancia máxima para interactuar
    public LayerMask interactableLayer; // Capa para objetos interactuables
    public GameObject interactUI; // UI que dice "Presiona E para interactuar"

    private IInteractable currentTarget;

    void Update()
    {
        DetectInteractable();

        if (currentTarget != null && Input.GetKeyDown(KeyCode.E))
        {
            currentTarget.Interact();
        }
    }

    void DetectInteractable()
    {
        // Raycast desde la posición del jugador hacia adelante
        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactRange, interactableLayer);

        if (hit != null && hit.TryGetComponent<IInteractable>(out var interactable))
        {
            currentTarget = interactable;
            if (interactUI != null) interactUI.SetActive(true);
        }
        else
        {
            currentTarget = null;
            if (interactUI != null) interactUI.SetActive(false);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
