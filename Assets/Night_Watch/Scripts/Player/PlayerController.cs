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
        FindInteractable();

        if (Input.GetKeyDown(KeyCode.Space) && currentInteractable != null)
        {
            if (GameManager.Instance != null &&
                GameManager.Instance.dayNightCycle != null)
            {
                // Se è giorno, raccogli semi
                if (GameManager.Instance.dayNightCycle.CurrentPhase == NightWatchPhase.Day)
                {
                    // Ricerca diretta del seme tramite Raycast
                    Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
                    if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
                    {
                        // Verifica se l'oggetto è un seme (tramite tag o nome)
                        if (hit.collider.CompareTag("Seed"))
                        {
                            SeedManager.Instance.CollectSeed(hit.collider.gameObject);
                        }
                    }
                }
                // Se è notte, pianta fiori
                else if (GameManager.Instance.dayNightCycle.CurrentPhase == NightWatchPhase.Night)
                {
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
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
                currentInteractable = interactable;
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