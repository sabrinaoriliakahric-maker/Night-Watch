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
    
    // Lista per tracciare i VFX di spawn istanziati (da distruggere poi)
    private List<GameObject> spawnedVFXList = new List<GameObject>();
    
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
        
        // FIX: Aggiorna previousPhase PRIMA dei controlli per evitare doppie chiamate
        // Salva la fase da elaborare
        NightWatchPhase phaseToProcess = previousPhase;
        previousPhase = currentPhase;

        // Se la fase è cambiata da Day a Night
        if (phaseToProcess == NightWatchPhase.Day && currentPhase == NightWatchPhase.Night)
        {
            OnNightStart();
        }
        // Se la fase è cambiata da Night a Day
        else if (phaseToProcess == NightWatchPhase.Night && currentPhase == NightWatchPhase.Day)
        {
            OnDayStart();
        }
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
        Debug.Log($"[PlantingZoneManager] Inizio GIORNO - disattivo zone e rimuovo fiori (activeZones count: {activeZones.Count})");
        
        // Rimuovi tutti i fiori spawnati
        if (FlowerManager.Instance != null)
        {
            FlowerManager.Instance.ClearAllFlowers();
        }
        
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

        // Disattiva tutte prima e rimuovi i VFX per evitare duplicati
        foreach (var zone in allZones)
        {
            zone.DeactivateZoneVFX(); // Rimuove i VFX vecchi
            zone.gameObject.SetActive(false);
        }
        activeZones.Clear();
        
        // Distruggi anche i VFX di spawn vecchi dalla lista
        foreach (GameObject vfx in spawnedVFXList)
        {
            if (vfx != null)
            {
                Destroy(vfx);
            }
        }
        spawnedVFXList.Clear();

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
        
        // Lista per tracciare le posizioni delle zone attivate (per distanziarle)
        List<Vector3> zonePositions = new List<Vector3>();
        
        for (int i = 0; i < zonesToActivate; i++)
        {
            PlantingZone zone = shuffledZones[i];
            
            // Posiziona la zona nell'area di spawn, distanziata dalle altre zone
            Vector3 newPos = GetRandomPositionInArea(zonePositions);
            zone.transform.position = newPos;
            
            // Reset della rotazione per evitare fiori ruotati
            zone.transform.rotation = Quaternion.identity;
            
            // Aggiungi la posizione alla lista per le successive zone
            zonePositions.Add(newPos);
            zone.gameObject.SetActive(true);
            
            // Forza il sync dei transform per i physics PRIMA di abilitare il collider
            Physics.SyncTransforms();
            
            // Abilita esplicitamente il BoxCollider dopo SetActive
            BoxCollider col = zone.GetComponent<BoxCollider>();
            if (col != null)
            {
                // Imposta dimensioni minime se troppo piccole
                if (col.size.x < 0.1f || col.size.y < 0.1f || col.size.z < 0.1f)
                {
                    col.size = new Vector3(2f, 2f, 2f);
                    Debug.Log($"[PlantingZoneManager] BoxCollider troppo piccolo, ridimensionato a (2,2,2)");
                }
                
                col.enabled = true;
                Debug.Log($"[PlantingZoneManager] BoxCollider abilitato per {zone.name}, enabled={col.enabled}, isTrigger={col.isTrigger}, size={col.size}, layer={LayerMask.LayerToName(zone.gameObject.layer)}");
            }
            else
            {
                Debug.LogWarning($"[PlantingZoneManager] Nessun BoxCollider trovato su {zone.name}!");
            }
            
            // Debug: verifica che il collider sia effettivamente rilevabile
            Collider[] testColliders = Physics.OverlapSphere(zone.transform.position, 1f);
            Debug.Log($"[PlantingZoneManager] Test overlap alla pos {zone.transform.position}: trovati {testColliders.Length} collider");
            foreach (var tc in testColliders)
            {
                Debug.Log($"[PlantingZoneManager]   - {tc.name} (PlantingZone: {tc.GetComponent<PlantingZone>() != null})");
            }
            
            // Attiva il VFX delle fiamme per rendere visibile la zona
            zone.ActivateZoneVFX();
            
            activeZones.Add(zone);
            
            Debug.Log($"[PlantingZoneManager] Attivata zona {i}: {zone.name} alla posizione {newPos}");
            
            // Spawna effetto particellare se assegnato e aggiungilo alla lista per потом distruggerlo
            if (spawnVFXPrefab != null)
            {
                GameObject vfx = Instantiate(spawnVFXPrefab, newPos, Quaternion.identity);
                spawnedVFXList.Add(vfx);
                Debug.Log($"[PlantingZoneManager] VFX di spawn aggiunto alla lista, totale: {spawnedVFXList.Count}");
            }
        }

        Debug.Log($"[PlantingZoneManager] ATTIVATE {zonesToActivate} planting zones!");
    }

    /// <summary>
    /// Disattiva tutte le zone
    /// </summary>
    public void DeactivateAllZones()
    {
        Debug.Log($"[PlantingZoneManager] DeactivateAllZones chiamato - zone da disattivare: {activeZones.Count}");
        
        foreach (var zone in activeZones)
        {
            if (zone != null)
            {
                Debug.Log($"[PlantingZoneManager] Disattivo zona: {zone.name}");
                
                // Rimuovi il VFX delle fiamme prima di disattivare
                zone.DeactivateZoneVFX();
                zone.ResetZone();
                zone.gameObject.SetActive(false);
                
                Debug.Log($"[PlantingZoneManager] Zona {zone.name} disattivata");
            }
        }
        activeZones.Clear();
        
        // Distruggi tutti i VFX di spawn tracciati
        Debug.Log($"[PlantingZoneManager] Distruzione VFX di spawn - count: {spawnedVFXList.Count}");
        foreach (GameObject vfx in spawnedVFXList)
        {
            if (vfx != null)
            {
                Destroy(vfx);
            }
        }
        spawnedVFXList.Clear();
        
        Debug.Log("[PlantingZoneManager] Tutte le zone disattivate");
    }

    [Header("Distanza minima tra le zone (metri)")]
    public float minZoneDistance = 2f;

    [Header("Tentativi massimi per trovare posizione valida")]
    public int maxPositionAttempts = 30;

    /// <summary>
    /// Restituisce una posizione casuale all'interno dell'area di spawn
    /// </summary>
    private Vector3 GetRandomPositionInArea()
    {
        return GetRandomPositionInArea(null);
    }

    /// <summary>
    /// Restituisce una posizione casuale all'interno dell'area di spawn, distanziata dalle posizioni esistenti
    /// </summary>
    /// <param name="existingPositions">Lista di posizioni da evitare (può essere null)</param>
    /// <returns>Posizione valida o Vector3.zero se non trovata</returns>
    private Vector3 GetRandomPositionInArea(List<Vector3> existingPositions)
    {
        if (seedSpawnArea == null)
        {
            Debug.LogError("[PlantingZoneManager] seedSpawnArea è NULL!");
            return Vector3.zero;
        }

        Vector3 worldPos = Vector3.zero;
        bool validPositionFound = false;
        int attempts = 0;

        // Prova a trovare una posizione valida per un numero massimo di tentativi
        while (!validPositionFound && attempts < maxPositionAttempts)
        {
            attempts++;

            Vector3 randomOffset = new Vector3(
                Random.Range(-seedSpawnArea.size.x / 2, seedSpawnArea.size.x / 2),
                Random.Range(-seedSpawnArea.size.y / 2, seedSpawnArea.size.y / 2),
                Random.Range(-seedSpawnArea.size.z / 2, seedSpawnArea.size.z / 2)
            );

            Vector3 localPoint = seedSpawnArea.center + randomOffset;
            worldPos = seedSpawnArea.transform.TransformPoint(localPoint);
            
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

            // Verifica distanza dalle posizioni esistenti
            if (existingPositions != null && existingPositions.Count > 0)
            {
                bool tooClose = false;
                foreach (Vector3 existingPos in existingPositions)
                {
                    float distance = Vector3.Distance(worldPos, existingPos);
                    if (distance < minZoneDistance)
                    {
                        tooClose = true;
                        Debug.Log($"[PlantingZoneManager] Tentativo {attempts}: posizione {worldPos} troppo vicina a {existingPos} (distanza: {distance:F2}m < {minZoneDistance}m)");
                        break;
                    }
                }

                if (!tooClose)
                {
                    validPositionFound = true;
                    Debug.Log($"[PlantingZoneManager] Posizione valida trovata dopo {attempts} tentativi: {worldPos}");
                }
            }
            else
            {
                // Prima zona, nessun controllo di distanza necessario
                validPositionFound = true;
                Debug.Log($"[PlantingZoneManager] Prima zona, posizione generata: {worldPos}");
            }
        }

        if (!validPositionFound)
        {
            Debug.LogWarning($"[PlantingZoneManager] Impossibile trovare posizione valida dopo {maxPositionAttempts} tentativi! Uso ultima posizione generata.");
        }
        
        return worldPos;
    }
}
