using UnityEngine;

public class FlowerManager : MonoBehaviour
{
    public static FlowerManager Instance;
    public int FlowerCount { get; private set; }

    void Awake() => Instance = this;

    public void SpawnFlower(Vector3 pos)
    {
        FlowerCount++;
        GateController.Instance.UpdateGate(FlowerCount);
    }

    public void ResetFlowers()
    {
        FlowerCount = 0;
    }
}
