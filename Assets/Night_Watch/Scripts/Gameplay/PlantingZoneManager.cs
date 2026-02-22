using System.Collections.Generic;
using UnityEngine;

public class PlantingZoneManager : MonoBehaviour
{
    public static PlantingZoneManager Instance;

    [Header("Prefab effetto particellare (opzionale)")]
    public GameObject spawnVFXPrefab;

    [Header("Numero di zone da attivare")]
    public int numberOfZones = 5;
    
    [Header("Offset altezza delle zone sopra il terreno")]
    public float zoneHeight = 0.1f;
    
    [Header("Layer del terreno per il Raycast")]
    public LayerMask groundLayer = ~0;
    
    // Riferimento all'area di spawn
    private BoxCollider seedSpawnArea => SeedManager.Instance?.seedSpawnArea;

    // Lista delle zone attive
    private List<PlantingZone> activeZones = new List<PlantingZone>();
    
    // Tutte le planting zone disponibili nella scena
    private PlantingZone[] allZones;

    // Track della fase precedente per rilevare il cambio
    private NightWatchPhase previousPhase = NightWatchPhase.Day;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // Trova tutte le planting zone nella scena
        allZones = FindObjectsByType<PlantingZone>(FindObjectsSortMode.None);
        
        Debug.Log($"[PlantingZoneManager] Trovate {allZones?.Length ?? 0} planting zone nella scena");

        // Disattiva tutte all'inizio
        foreach (var zone in allZones)
        {
            zone.gameObject.SetActive(false);
        }
        
        if (GameManager.Instance != null && GameManager.Instance.dayNightCycle != null)
        {
            previousPhase = GameManager.Instance.dayNightCycle.CurrentPhase;
        }
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.dayNightCycle == null)
            return;

        NightWatchPhase currentPhase = GameManager.Instance.dayNightCycle.CurrentPhase;

        // Se la fase è cambiata da Day a Night
        if (previousPhase == NightWatchPhase.Day && currentPhase == NightWatchPhase.Night)
        {
            OnNightStart();
        }
        // Se la fase è cambiata da Night a Day
        else if (previousPhase == NightWatchPhase.Night && currentPhase == NightWatchPhase.Day)
        {
            OnDayStart();
        }

        previousPhase = currentPhase;
    }

    /// <summary>
    /// Called when night starts - activate planting zones
    /// </summary>
    private void OnNightStart()
    {
        Debug.Log("[PlantingZoneManager] Inizio NOTTE - attivo zone");
        ActivatePlantingZones();
    }

    /// <summary>
    /// Called when day starts - deactivate planting zones
    /// </summary>
    private void OnDayStart()
    {
        Debug.Log("[PlantingZoneManager] Inizio GIORNO - disattivo zone");
        DeactivateAllZones();
    }

    /// <summary>
    /// Attiva un numero casuale di planting zone nella stessa area dei semi
    /// </summary>
    public void ActivatePlantingZones()
    {
        if (allZones == null || allZones.Length == 0 || seedSpawnArea == null)
        {
            Debug.LogWarning("PlantingZoneManager: Nessuna planting zone trovata nella scena!");
            return;
        }

        Debug.Log($"[PlantingZoneManager] Attivazione zone - allZones: {allZones.Length}, numberOfZones: {numberOfZones}");

        // Disattiva tutte prima
        foreach (var zone in allZones)
        {
            zone.gameObject.SetActive(false);
        }
        activeZones.Clear();

        // Mescola l'ordine delle zone
        List<PlantingZone> shuffledZones = new List<PlantingZone>(allZones);
        for (int i = 0; i < shuffledZones.Count; i++)
        {
            PlantingZone temp = shuffledZones[i];
            int randomIndex = Random.Range(i, shuffledZones.Count);
            shuffledZones[i] = shuffledZones[randomIndex];
            shuffledZones[randomIndex] = temp;
        }

        // Attiva solo il numero richiesto di zone
        int zonesToActivate = Mathf.Min(numberOfZones, shuffledZones.Count);
        
        Debug.Log($"[PlantingZoneManager] Zone da attivare: {zonesToActivate}");
        
        for (int i = 0; i < zonesToActivate; i++)
        {
            // Posiziona la zona nell'area di spawn
            Vector3 newPos = GetRandomPositionInArea();
            shuffledZones[i].transform.position = newPos;
            shuffledZones[i].gameObject.SetActive(true);
            activeZones.Add(shuffledZones[i]);
            
            Debug.Log($"[PlantingZoneManager] Attivata zona {i}: {shuffledZones[i].name} alla posizione {newPos}");
            
            // Spawna effetto particellare se assegnato
            if (spawnVFXPrefab != null)
            {
                Instantiate(spawnVFXPrefab, newPos, Quaternion.identity);
            }
        }

        Debug.Log($"[PlantingZoneManager] ATTIVATE {zonesToActivate} planting zones!");
    }

    /// <summary>
    /// Disattiva tutte le zone
    /// </summary>
    public void DeactivateAllZones()
    {
        foreach (var zone in activeZones)
        {
            if (zone != null)
            {
                zone.ResetZone();
                zone.gameObject.SetActive(false);
            }
        }
        activeZones.Clear();
    }

    /// <summary>
    /// Restituisce una posizione casuale all'interno dell'area di spawn
    /// </summary>
    private Vector3 GetRandomPositionInArea()
    {
        if (seedSpawnArea == null)
        {
            Debug.LogError("[PlantingZoneManager] seedSpawnArea è NULL!");
            return Vector3.zero;
        }

        Vector3 randomOffset = new Vector3(
            Random.Range(-seedSpawnArea.size.x / 2, seedSpawnArea.size.x / 2),
            Random.Range(-seedSpawnArea.size.y / 2, seedSpawnArea.size.y / 2),
            Random.Range(-seedSpawnArea.size.z / 2, seedSpawnArea.size.z / 2)
        );

        Vector3 localPoint = seedSpawnArea.center + randomOffset;
        Vector3 worldPos = seedSpawnArea.transform.TransformPoint(localPoint);
        
        RaycastHit hit;
        Vector3 rayOrigin = new Vector3(worldPos.x, 100f, worldPos.z);
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 200f, groundLayer, QueryTriggerInteraction.Ignore))
        {
            worldPos.y = hit.point.y + zoneHeight;
        }
        else
        {
            worldPos.y = seedSpawnArea.transform.position.y + zoneHeight;
            Debug.LogWarning($"[PlantingZoneManager] Raycast non ha colpito il terreno, uso fallback Y: {worldPos.y}");
        }
        
        Debug.Log($"[PlantingZoneManager] Posizione generata: {worldPos}");
        
        return worldPos;
    }
}
