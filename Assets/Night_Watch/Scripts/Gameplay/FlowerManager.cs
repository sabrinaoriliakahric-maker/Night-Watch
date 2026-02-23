using UnityEngine;
using System.Collections.Generic;

public class FlowerManager : MonoBehaviour
{
    // Singleton semplice
    public static FlowerManager Instance;

    [Header("Prefab del fiore")]
    public GameObject flowerPrefab; // Trascina il prefab del fiore in Unity

    [Header("Effetto particellare quando appare un fiore")]
    public GameObject flowerSpawnVFXPrefab;

    // Lista per tracciare i fiori spawnati
    private List<GameObject> spawnedFlowers = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("[FlowerManager] Singleton creato");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Funzione per spawnare un fiore in una posizione (senza rotazione - per retrocompatibilità)
    public void SpawnFlower(Vector3 position)
    {
        SpawnFlower(position, Quaternion.identity);
    }

    // Funzione per spawnare un fiore in una posizione con rotazione
    public void SpawnFlower(Vector3 position, Quaternion rotation)
    {
        Debug.Log($"[FlowerManager] SpawnFlower chiamato alla posizione: {position}, rotazione: {rotation}");
        Debug.Log($"[FlowerManager] flowerPrefab è null? {flowerPrefab == null}");
        
        if (flowerPrefab != null)
        {
            // Usa la rotazione passata come parametro invece di Quaternion.identity
            GameObject flower = Instantiate(flowerPrefab, position, rotation);
            spawnedFlowers.Add(flower); // Aggiungi alla lista
            Debug.Log($"[FlowerManager] Fiore istanziato: {flower.name} - Totale fiori: {spawnedFlowers.Count}");
            
            // Spawna VFX se assegnato
            if (flowerSpawnVFXPrefab != null)
            {
                Instantiate(flowerSpawnVFXPrefab, position, Quaternion.identity);
                Debug.Log($"[FlowerManager] VFX fiore spawnato");
            }
            else
            {
                Debug.LogWarning("[FlowerManager] flowerSpawnVFXPrefab NON assegnato!");
            }
        }
        else
        {
            Debug.LogError("[FlowerManager] flowerPrefab NON assegnato! Assegnalo nell'Inspector di Unity!");
        }
    }

    /// <summary>
    /// Rimuove tutti i fiori spawnati (chiamato all'inizio del giorno)
    /// </summary>
    public void ClearAllFlowers()
    {
        Debug.Log($"[FlowerManager] ClearAllFlowers - Count PRIMA: {spawnedFlowers.Count}");
        
        // Rimuovi tutti i fiori dalla scena
        foreach (GameObject flower in spawnedFlowers)
        {
            if (flower != null)
            {
                Debug.Log($"[FlowerManager] Distruggo fiore: {flower.name}");
                Destroy(flower);
            }
        }
        
        spawnedFlowers.Clear();
        Debug.Log($"[FlowerManager] ClearAllFlowers - Count DOPO clear: {spawnedFlowers.Count}");
    }
}
