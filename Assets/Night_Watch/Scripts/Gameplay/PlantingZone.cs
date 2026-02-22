using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PlantingZone : MonoBehaviour, IInteractable
{
    private bool planted = false;
    private Renderer rend;

    [Header("Effetto particellare quando si pianta un fiore")]
    public GameObject plantVFXPrefab;
    
    [Header("Effetto fiamme per rendere visibile la zona (attivo di notte)")]
    public GameObject zoneIndicatorVFXPrefab;
    
    [Header("Offset altezza fiore sopra la zona")]
    public float flowerHeightOffset = 0.5f;

    // Riferimento al VFX delle fiamme istanziato
    private GameObject activeZoneVFX;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    private void Start()
    {
        // All'inizio è disattivato (verrà attivato da PlantingZoneManager di notte)
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Attiva il VFX indicatore della zona (le fiamme)
    /// </summary>
    public void ActivateZoneVFX()
    {
        if (zoneIndicatorVFXPrefab != null && activeZoneVFX == null)
        {
            activeZoneVFX = Instantiate(zoneIndicatorVFXPrefab, transform.position, Quaternion.identity, transform);
            Debug.Log("[PlantingZone] ✅ Zone indicator VFX attivato (fiamme)");
        }
    }

    /// <summary>
    /// Disattiva il VFX indicatore della zona
    /// </summary>
    public void DeactivateZoneVFX()
    {
        if (activeZoneVFX != null)
        {
            Destroy(activeZoneVFX);
            activeZoneVFX = null;
            Debug.Log("[PlantingZone] Zone indicator VFX disattivato");
        }
    }

    public void Interact()
    {
        Debug.Log($"[PlantingZone] Interact chiamato - planted: {planted}");
        
        // Se già piantato, non fare nulla
        if (planted) 
        {
            Debug.Log("[PlantingZone] Già piantato, ritorno");
            return;
        }

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
            Debug.Log($"[PlantingZone] ✅ Spawn fiore al centro: {flowerPosition} (offset Y: {flowerHeightOffset})");
            
            // Verifica FlowerManager
            if (FlowerManager.Instance != null)
            {
                Debug.Log("[PlantingZone] ✅ FlowerManager.Instance non è null, chiamo SpawnFlower");
                FlowerManager.Instance.SpawnFlower(flowerPosition);
            }
            else
            {
                Debug.LogError("[PlantingZone] ❌ FlowerManager.Instance è NULL!");
            }
            
            // Spawna effetto particellare se assegnato
            if (plantVFXPrefab != null)
            {
                Instantiate(plantVFXPrefab, transform.position, Quaternion.identity);
                Debug.Log("[PlantingZone] ✅ plantVFX spawnato");
            }
            else
            {
                Debug.LogWarning("[PlantingZone] ⚠️ plantVFXPrefab NON assegnato!");
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
