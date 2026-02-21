using UnityEngine;

public class FlowerManager : MonoBehaviour
{
    // Singleton semplice
    public static FlowerManager Instance;

    [Header("Prefab del fiore")]
    public GameObject flowerPrefab; // Trascina il prefab del fiore in Unity

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Funzione per spawnare un fiore in una posizione
    public void SpawnFlower(Vector3 position)
    {
        if (flowerPrefab != null)
        {
            _ = Instantiate(flowerPrefab, position, Quaternion.identity);
            Debug.Log("Fiore spawnato!");
        }
        else
        {
            Debug.LogWarning("Flower prefab non assegnato!");
        }
    }
}
