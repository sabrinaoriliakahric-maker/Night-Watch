using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float interactRange = 2f;

    private Rigidbody rb;

    [Header("Camera")]
    public Transform cameraTransform; // collega la Main Camera

    private IInteractable currentInteractable;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        HandleMovement();
        HandleInteraction();
    }

    // ==========================
    // MOVIMENTO
    // ==========================
    private void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (cameraTransform == null) return;

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = forward * v + right * h;

        Vector3 velocity = moveDir * moveSpeed;
        velocity.y = rb.linearVelocity.y;

        rb.linearVelocity = velocity;
    }

    // ==========================
    // INTERAZIONE (SPACE)
    // ==========================
    private void HandleInteraction()
    {
        // Trova l'oggetto interagibile davanti al player
        FindInteractable();

        // Se preme SPACE e c'è qualcosa di interagibile
        if (Input.GetKeyDown(KeyCode.Space) && currentInteractable != null)
        {
            // Controlla che il gioco sia in uno stato valido
            if (GameManager.Instance != null &&
                GameManager.Instance.dayNightCycle != null)
            {
                NightWatchPhase phase = GameManager.Instance.dayNightCycle.CurrentPhase;

                // Se è giorno, permetti interazione con semi
                if (phase == NightWatchPhase.Day)
                {
                    // Chiama l'interazione sull'oggetto (funziona per semi e planting zone)
                    currentInteractable.Interact();
                }
                // Se è notte, permetti interazione solo con PlantingZone
                else if (phase == NightWatchPhase.Night)
                {
                    // Chiama l'interazione (PlantingZone.Interact() gestisce la logica)
                    currentInteractable.Interact();
                }
            }
        }
    }

    // ==========================
    // RILEVA OGGETTI
    // ==========================
    private void FindInteractable()
    {
        currentInteractable = null;

        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
        
        // Debug: visualizza il raycast nell'editor
        Debug.DrawRay(transform.position + Vector3.up, transform.forward * interactRange, Color.red, 0.1f);
        
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            // Debug: mostra cosa ha colpito
            Debug.Log($"Player vede: {hit.collider.gameObject.name} (tag: {hit.collider.tag})");
            
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                currentInteractable = interactable;
                Debug.Log($"Oggetto interagibile trovato: {hit.collider.gameObject.name}");
            }
            else
            {
                Debug.Log($"L'oggetto {hit.collider.gameObject.name} non ha IInteractable!");
            }
        }
    }

    // ==========================
    // DEBUG VISIVO
    // ==========================
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position + Vector3.up, transform.forward * interactRange);
    }
}