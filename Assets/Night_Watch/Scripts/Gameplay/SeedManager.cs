using UnityEngine;

public class SeedManager : MonoBehaviour
{
    public static SeedManager Instance;

    [Header("Semi raccolti")]
    [HideInInspector]
    public int totalSeeds = 0;

    [Header("Spawn Settings")]
    public GameObject seedPrefab;
    public int seedCount = 5;
    public BoxCollider spawnArea;
    public float heightAboveGround = 0.5f;

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

    private void Start()
    {
        SpawnSeeds();
    }

    public void SpawnSeeds()
    {
        if (spawnArea == null || seedPrefab == null)
        {
            return;
        }

        for (int i = 0; i < seedCount; i++)
        {
            Vector3 localPos = new(
                Random.Range(-spawnArea.size.x / 2f, spawnArea.size.x / 2f),
                0f,
                Random.Range(-spawnArea.size.z / 2f, spawnArea.size.z / 2f)
            );

            Vector3 spawnPos = spawnArea.transform.TransformPoint(localPos);

            spawnPos.y = Physics.Raycast(spawnPos + (Vector3.up * 10f), Vector3.down, out RaycastHit hit, 50f)
                ? hit.point.y + heightAboveGround
                : heightAboveGround;

            _ = Instantiate(seedPrefab, spawnPos, Quaternion.identity);
        }
    }

    // ✅ Metodo per raccogliere semi
    public void CollectSeed()
    {
        totalSeeds++;
        Debug.Log("Semi raccolti: " + totalSeeds);
    }
}
