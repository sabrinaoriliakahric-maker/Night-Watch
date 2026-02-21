using System.Collections.Generic;
using UnityEngine;

public class SeedManager : MonoBehaviour
{
    public static SeedManager Instance;

    [Header("Prefab da spawnare")]
    public GameObject seedPrefab;

    [Header("Numero di semi da spawnare")]
    public int numberOfSeeds = 5;

    [Header("Raggio di spawn casuale")]
    public float spawnRadius = 10f;

    [Header("Altezza di spawn delle istanze")]
    public float spawnHeight = 0.5f;

    [Header("Area di spawn dei semi")]
    public BoxCollider seedSpawnArea;

    [Header("Effetto particellare quando spawna un seme")]
    public GameObject spawnVFXPrefab;

    // Lista dei semi attualmente presenti
    private List<GameObject> spawnedSeeds = new List<GameObject>();

    // Conteggio semi raccolti dal player
    private int collectedSeeds = 0;

    [Header("Conteggio semi raccolti dal player")]
    public int CollectedSeeds => collectedSeeds; // solo get, nessun errore di protezione

    // Lista pubblica di sola lettura dei semi spawnati
    public List<GameObject> SpawnedSeeds => spawnedSeeds;

    private bool isSpawningActive = false;
    
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

        // Se la fase è cambiata da Day a Night, elimina i semi non raccolti
        if (previousPhase == NightWatchPhase.Day && currentPhase == NightWatchPhase.Night)
        {
            OnNightStart();
        }

        // Abilita/disabilita interazione dei semi in base al Day/Night
        if (currentPhase == NightWatchPhase.Day)
        {
            EnableSeedInteraction(true);
        }
        else
        {
            EnableSeedInteraction(false);
        }

        previousPhase = currentPhase;
    }

    /// <summary>
    /// Called when night starts - clear uncollected seeds
    /// </summary>
    private void OnNightStart()
    {
        if (spawnedSeeds.Count > 0)
        {
            Debug.Log($"Notte! Eliminati {spawnedSeeds.Count} semi non raccolti.");
            ClearAllSeeds();
        }
    }

    /// <summary>
    /// Spawn dei semi
    /// </summary>
    public void SpawnSeeds()
    {
        if (GameManager.Instance.currentState != GameState.Gameplay || seedPrefab == null) return;

        ClearAllSeeds();
        spawnedSeeds.Clear();

        for (int i = 0; i < numberOfSeeds; i++)
        {
            Vector3 spawnPos = GetSpawnPosition();
            GameObject newSeed = Instantiate(seedPrefab, spawnPos, Quaternion.identity);
            spawnedSeeds.Add(newSeed);
            
            // Spawn effetto particellare per ogni seme
            if (spawnVFXPrefab != null)
            {
                Instantiate(spawnVFXPrefab, spawnPos, Quaternion.identity);
            }
        }

        isSpawningActive = true;
    }

    /// <summary>
    /// Pulisce tutti i semi spawnati
    /// </summary>
    public void ClearAllSeeds()
    {
        foreach (var seed in spawnedSeeds)
        {
            if (seed != null)
                Destroy(seed);
        }
        spawnedSeeds.Clear();
        isSpawningActive = false;
    }

    /// <summary>
    /// Abilita/disabilita interazione semi (collider e script)
    /// </summary>
    private void EnableSeedInteraction(bool enable)
    {
        foreach (var seed in spawnedSeeds)
        {
            if (seed != null)
            {
                Collider col = seed.GetComponent<Collider>();
                if (col != null)
                    col.enabled = enable;

                MonoBehaviour[] scripts = seed.GetComponents<MonoBehaviour>();
                foreach (var script in scripts)
                    script.enabled = enable;
            }
        }
    }

    /// <summary>
    /// Posizione casuale per spawn dei semi
    /// </summary>
    private Vector3 GetSpawnPosition()
    {
        if (seedSpawnArea != null)
        {
            // Spawna all'interno del Box Collider della SeedSpawnArea
            Vector3 randomPoint = seedSpawnArea.center + new Vector3(
                Random.Range(-seedSpawnArea.size.x / 2, seedSpawnArea.size.x / 2),
                Random.Range(-seedSpawnArea.size.y / 2, seedSpawnArea.size.y / 2),
                Random.Range(-seedSpawnArea.size.z / 2, seedSpawnArea.size.z / 2)
            );
            return seedSpawnArea.transform.TransformPoint(randomPoint);
        }
        else
        {
            // Fallback al metodo originale
            Vector2 circle = Random.insideUnitCircle * spawnRadius;
            return new Vector3(circle.x, spawnHeight, circle.y);
        }
    }

    // -------------------------------
    // Gestione raccolta semi
    // -------------------------------

    /// <summary>
    /// Raccoglie un seme specifico (giorno)
    /// </summary>
    public void CollectSeed(GameObject seed)
    {
        if (spawnedSeeds.Contains(seed))
        {
            spawnedSeeds.Remove(seed);
            Destroy(seed);
            collectedSeeds++;
            Debug.Log("Seme raccolto! Totale: " + collectedSeeds);
        }
    }

    /// <summary>
    /// Usa un seme raccolto per piantare un fiore (notte)
    /// </summary>
    /// <returns>True se c'era un seme da usare</returns>
    public bool UseSeedForPlanting()
    {
        if (collectedSeeds > 0)
        {
            collectedSeeds--;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Numero di semi ancora presenti nello scenario
    /// </summary>
    public int totalSeeds => spawnedSeeds.Count;
}