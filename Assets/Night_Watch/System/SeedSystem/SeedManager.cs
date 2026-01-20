using UnityEngine;
using System.Collections.Generic;

public class SeedManager : MonoBehaviour
{
    public static SeedManager Instance;
    public int Seeds { get; private set; }

    private List<Seed> activeSeeds = new();

    void Awake() => Instance = this;

    public void Register(Seed seed) => activeSeeds.Add(seed);

    public void CollectSeed(Seed seed)
    {
        Seeds++;
        activeSeeds.Remove(seed);
        Destroy(seed.gameObject);
    }
}
