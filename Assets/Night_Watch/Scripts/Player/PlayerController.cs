using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    [Header("Range interazione Semi (giorno)")]
    public float interactRange = 1f;
    
    [Header("Range interazione PlantingZone (notte)")]
    public float plantingZoneInteractRange = 3f;

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

        // Ruota il player verso la direzione di movimento (Facing Direction)
        if (moveDir.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
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
            Debug.Log($"[PlayerController] Player pos: {transform.position}");
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

        // Usa il range appropriato: più grande per PlantingZone di notte, più piccolo per Seed di giorno
        float currentRange = (typeof(T) == typeof(PlantingZone)) ? plantingZoneInteractRange : interactRange;
        
        // Usa OverlapSphere per trovare tutti i collider vicini
        Collider[] colliders = Physics.OverlapSphere(transform.position, currentRange);

        // DEBUG: Mostra quanti collider trova
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log($"[PlayerController] Trovati {colliders.Length} collider nel raggio di {currentRange}");
        }

        float closestDistance = float.MaxValue;
        IInteractable closestInteractable = null;

        foreach (Collider col in colliders)
        {
            // Salta il collider del player stesso
            if (col.gameObject == gameObject) continue;

            // Verifica se il componente è del tipo specificato (Seed o PlantingZone)
            T targetComponent = col.GetComponent<T>();
            if (targetComponent != null)
            {
                IInteractable interactable = col.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    // Calcola la distanza 2D (ignorando Y) per le PlantingZone
                    // Questo permette di rilevare le zone anche se sono a diverse altezze
                    float dist;
                    if (typeof(T) == typeof(PlantingZone))
                    {
                        // Distanza 2D: solo X e Z
                        float dx = transform.position.x - col.transform.position.x;
                        float dz = transform.position.z - col.transform.position.z;
                        dist = Mathf.Sqrt(dx * dx + dz * dz);
                    }
                    else
                    {
                        // Distanza 3D per i Seed
                        dist = Vector3.Distance(transform.position, col.transform.position);
                    }
                    
                    // DEBUG: mostra distanza
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        bool hasPlantingZone = col.GetComponent<PlantingZone>() != null;
                        Debug.Log($"[PlayerController] Collider: {col.gameObject.name}, PlantingZone: {hasPlantingZone}, Pos: {col.transform.position}, Distanza: {dist:F2}");
                    }
                    
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
        // Determina la fase corrente per mostrare il range corretto
        float currentRange = interactRange;
        
        if (GameManager.Instance != null && 
            GameManager.Instance.dayNightCycle != null &&
            GameManager.Instance.dayNightCycle.CurrentPhase == NightWatchPhase.Night)
        {
            currentRange = plantingZoneInteractRange;
            Gizmos.color = Color.magenta; // Magenta per notte (planting zone)
        }
        else
        {
            Gizmos.color = Color.yellow; // Giorno per seed
        }
        
        Gizmos.DrawWireSphere(transform.position, currentRange);
    }
}
