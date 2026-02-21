using UnityEngine;

/// <summary>
/// Componente per il seme raccoglibile
/// </summary>
public class Seed : MonoBehaviour, IInteractable
{
    [Header("Effetto visivo raccolta (opzionale)")]
    public GameObject collectVFX;

    private void Start()
    {
        // Assicurati che il game object abbia il tag "Seed"
        if (string.IsNullOrEmpty(gameObject.tag) || gameObject.tag == "Untagged")
        {
            gameObject.tag = "Seed";
        }
    }

    /// <summary>
    /// Interazione: il seme viene raccolto
    /// </summary>
    public void Interact()
    {
        // Spawna effetto visivo prima di raccogliere
        if (collectVFX != null)
        {
            Instantiate(collectVFX, transform.position, Quaternion.identity);
        }
        
        // Chiama il metodo di SeedManager per raccogliere il seme
        if (SeedManager.Instance != null)
        {
            SeedManager.Instance.CollectSeed(gameObject);
        }
    }
}
