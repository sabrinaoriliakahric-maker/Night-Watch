using UnityEngine;

public class FloatingSeed : MonoBehaviour
{
    public float amplitude = 0.3f; // quanto oscilla
    public float speed = 2f;       // velocità dell’oscillazione

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position; // posizione iniziale
    }

    private void Update()
    {
        // oscillazione verticale sinusoidale
        float newY = startPos.y + (Mathf.Sin(Time.time * speed) * amplitude);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
