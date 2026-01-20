using UnityEngine;

public class GateController : MonoBehaviour
{
    public static GateController Instance;
    public Transform left;
    public Transform right;

    public float baseGap = 2f;
    public float expansion = 1.5f;

    void Awake() => Instance = this;

    public void UpdateGate(int flowers)
    {
        float gap = baseGap + flowers * expansion;
        left.localPosition = new Vector3(0, 0, -gap);
        right.localPosition = new Vector3(0, 0, gap);
    }
}
