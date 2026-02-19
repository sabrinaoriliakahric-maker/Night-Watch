using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float interactRange = 2f;

    private Rigidbody rb;
    public Transform cameraTransform; // collega qui la Main Camera

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void Update()
    {
        Move();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryInteract();
        }
    }

    private void Move()
    {
        float h = Input.GetAxis("Horizontal"); // A/D
        float v = Input.GetAxis("Vertical");   // W/S

        // direzione relativa alla camera
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // ignora la componente verticale della camera
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = (forward * v) + (right * h);

        if (moveDir.magnitude >= 0.1f)
        {
            rb.MovePosition(transform.position + (moveDir * moveSpeed * Time.deltaTime));
            transform.forward = moveDir; // player guarda dove si muove
        }
    }

    private void TryInteract()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactRange);
        foreach (Collider hit in hits)
        {
            IInteractable interactable = hit.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
                break;
            }
        }
    }
}
