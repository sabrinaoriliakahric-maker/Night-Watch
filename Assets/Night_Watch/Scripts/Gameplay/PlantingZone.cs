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
        // Prima disattiva eventuali VFX precedenti per evitare duplicati
        DeactivateZoneVFX();
        
        if (zoneIndicatorVFXPrefab != null && activeZoneVFX == null)
        {
            activeZoneVFX = Instantiate(zoneIndicatorVFXPrefab, transform.position, Quaternion.identity, transform);
            Debug.Log("[PlantingZone] Zone indicator VFX attivato (fiamme)");
        }
    }

    /// <summary>
    /// Disattiva il VFX indicatore della zona
    /// </summary>
    public void DeactivateZoneVFX()
    {
        Debug.Log($"[PlantingZone] DeactivateZoneVFX chiamato per {name}, activeZoneVFX è null? {activeZoneVFX == null}");
        
        if (activeZoneVFX != null)
        {
            Destroy(activeZoneVFX);
            activeZoneVFX = null;
            Debug.Log("[PlantingZone] Zone indicator VFX distrutto");
        }
        else
        {
            // Debug: verifica se ci sono figli con ParticleSystem che non sono stati tracciati
            foreach (Transform child in transform)
            {
                ParticleSystem ps = child.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    Debug.Log($"[PlantingZone] Trovato ParticleSystem non tracciato come figlio: {child.name}, lo distruggo");
                    Destroy(child.gameObject);
                }
            }
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
            // Usa transform.up e transform.rotation per allineare il fiore con la zona
            Vector3 flowerPosition = transform.position + transform.up * flowerHeightOffset;
            Quaternion flowerRotation = transform.rotation; // Allinea la rotazione del fiore con la zona
            Debug.Log($"[PlantingZone] Spawn fiore al centro: {flowerPosition} (offset Y: {flowerHeightOffset})");
            
            // Verifica FlowerManager
            if (FlowerManager.Instance != null)
            {
                Debug.Log("[PlantingZone] FlowerManager.Instance non è null, chiamo SpawnFlower");
                FlowerManager.Instance.SpawnFlower(flowerPosition, flowerRotation);
            }
            else
            {
                Debug.LogError("[PlantingZone] FlowerManager.Instance è NULL!");
            }
            
            // Spawna effetto particellare se assegnato
            if (plantVFXPrefab != null)
            {
                Instantiate(plantVFXPrefab, transform.position, Quaternion.identity);
                Debug.Log("[PlantingZone] plantVFX spawnato");
            }
            else
            {
                Debug.LogWarning("[PlantingZone] plantVFXPrefab NON assegnato!");
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
