using UnityEngine;

public class PlantingZone : MonoBehaviour, IInteractable
{
    private bool planted = false;
    private Renderer rend;

    [Header("Effetto particellare quando si pianta un fiore")]
    public GameObject plantVFXPrefab;
    
    [Header("Offset altezza fiore sopra la zona")]
    public float flowerHeightOffset = 0.5f;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    private void Start()
    {
        // All'inizio è disattivato (verrà attivato da PlantingZoneManager di notte)
        gameObject.SetActive(false);
    }

    public void Interact()
    {
        // Se già piantato, non fare nulla
        if (planted) return;

        // Verifica che sia notte
        if (GameManager.Instance == null ||
            GameManager.Instance.dayNightCycle == null ||
            GameManager.Instance.dayNightCycle.CurrentPhase != NightWatchPhase.Night)
        {
            Debug.Log("Puoi piantare solo di notte!");
            return;
        }

        // Usa un seme raccolto dal player
        if (SeedManager.Instance != null && SeedManager.Instance.UseSeedForPlanting())
        {
            planted = true;
            
            // Spawna il fiore AL CENTRO della planting zone con offset in altezza
            Vector3 flowerPosition = transform.position + Vector3.up * flowerHeightOffset;
            Debug.Log($"[PlantingZone] Spawn fiore al centro: {flowerPosition} (offset Y: {flowerHeightOffset})");
            FlowerManager.Instance?.SpawnFlower(flowerPosition);
            
            // Spawna effetto particellare se assegnato
            if (plantVFXPrefab != null)
            {
                Instantiate(plantVFXPrefab, transform.position, Quaternion.identity);
            }
            
            Debug.Log("Fiore piantato!");
            
            // NOTA: Il fiore resta visibile nella scena come oggetto separato.
            // Non disattiviamo la zona così il giocatore può vedere il fiore spawnato.
        }
        else
        {
            Debug.Log("Non hai semi da piantare!");
        }
    }

    /// <summary>
    /// Resetta la zona per il prossimo ciclo
    /// </summary>
    public void ResetZone()
    {
        planted = false;
    }
}
