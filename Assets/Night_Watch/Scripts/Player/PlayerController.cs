using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float interactRange = 2f;

    private Rigidbody rb;

    [Header("Camera")]
    public Transform cameraTransform;

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
        // Determina la fase corrente
        NightWatchPhase phase = NightWatchPhase.Day;
        if (GameManager.Instance != null && GameManager.Instance.dayNightCycle != null)
        {
            phase = GameManager.Instance.dayNightCycle.CurrentPhase;
        }

        // DEBUG: Mostra la fase corrente
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log($"[PlayerController] SPACE premuto - Fase: {phase}");
            Debug.Log($"[PlayerController] Semi raccolti: {SeedManager.Instance?.CollectedSeeds}");
        }

        // Trova l'oggetto interagibile appropriato in base alla fase
        if (phase == NightWatchPhase.Day)
        {
            // Di giorno: cerca solo Seed
            FindInteractable<Seed>();
        }
        else if (phase == NightWatchPhase.Night)
        {
            // Di notte: cerca solo PlantingZone
            FindInteractable<PlantingZone>();
        }

        // DEBUG: Mostra se ha trovato qualcosa
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log($"[PlayerController] Trovato interactable: {currentInteractable != null}");
        }

        // Se preme SPACE e c'è qualcosa di interagibile
        if (Input.GetKeyDown(KeyCode.Space) && currentInteractable != null)
        {
            // Controlla che il gioco sia in uno stato valido
            if (GameManager.Instance != null &&
                GameManager.Instance.dayNightCycle != null)
            {
                // Esegue l'interazione
                currentInteractable.Interact();
            }
        }
    }

    // ==========================
    // RILEVA OGGETTI VICINI (GENERICO)
    // ==========================
    private void FindInteractable<T>() where T : MonoBehaviour
    {
        currentInteractable = null;

        // Usa OverlapSphere per trovare tutti i collider vicini
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactRange);

        // DEBUG: Mostra quanti collider trova
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log($"[PlayerController] Trovati {colliders.Length} collider nel raggio di {interactRange}");
        }

        float closestDistance = float.MaxValue;
        IInteractable closestInteractable = null;

        foreach (Collider col in colliders)
        {
            // Salta il collider del player stesso
            if (col.gameObject == gameObject) continue;

            // DEBUG: Mostra ogni collider trovato
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log($"[PlayerController] Collider trovato: {col.gameObject.name}, ha PlantingZone: {col.GetComponent<PlantingZone>() != null}");
            }

            // Verifica se il componente è del tipo specificato (Seed o PlantingZone)
            T targetComponent = col.GetComponent<T>();
            if (targetComponent != null)
            {
                IInteractable interactable = col.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    // Calcola la distanza
                    float dist = Vector3.Distance(transform.position, col.transform.position);
                    
                    // Prendi il più vicino
                    if (dist < closestDistance)
                    {
                        closestDistance = dist;
                        closestInteractable = interactable;
                    }
                }
            }
        }

        if (closestInteractable != null)
        {
            currentInteractable = closestInteractable;
            Debug.Log($"[PlayerController] Interactable assegnato: {closestInteractable.GetType().Name}");
        }
    }

    // ==========================
    // DEBUG VISIVO
    // ==========================
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
