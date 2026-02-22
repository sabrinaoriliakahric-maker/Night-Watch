using UnityEngine;

public class FlowerManager : MonoBehaviour
{
    // Singleton semplice
    public static FlowerManager Instance;

    [Header("Prefab del fiore")]
    public GameObject flowerPrefab; // Trascina il prefab del fiore in Unity

    [Header("Effetto particellare quando appare un fiore")]
    public GameObject flowerSpawnVFXPrefab;

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

    // Funzione per spawnare un fiore in una posizione
    public void SpawnFlower(Vector3 position)
    {
        Debug.Log($"[FlowerManager] SpawnFlower chiamato alla posizione: {position}");
        
        if (flowerPrefab != null)
        {
            GameObject flower = Instantiate(flowerPrefab, position, Quaternion.identity);
            Debug.Log($"[FlowerManager] Fiore istanziato: {flower.name} alla posizione: {flower.transform.position}");
            
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
            Debug.LogError("[FlowerManager] ❌ flowerPrefab NON assegnato! Assegnalo nell'Inspector di Unity!");
        }
    }
}
