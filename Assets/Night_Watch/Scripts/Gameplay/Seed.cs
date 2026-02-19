using UnityEngine;

public class Seed : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        if (SeedManager.Instance != null)
        {
            SeedManager.Instance.CollectSeed();
        }

        Destroy(gameObject);
    }
}
