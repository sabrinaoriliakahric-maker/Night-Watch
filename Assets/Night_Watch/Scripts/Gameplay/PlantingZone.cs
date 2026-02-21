using UnityEngine;

public class PlantingZone : MonoBehaviour, IInteractable
{
    private bool planted = false;
    private Renderer rend;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
            rend.enabled = false; // Nascondi di giorno
    }

    private void Update()
    {
        if (GameManager.Instance != null &&
            GameManager.Instance.dayNightCycle != null &&
            rend != null)
        {
            // Mostra solo di notte
            rend.enabled = GameManager.Instance.dayNightCycle.CurrentPhase == NightWatchPhase.Night;
        }
    }

    public void Interact()
    {
        if (planted) return;

        // Solo se è notte
        if (GameManager.Instance == null ||
            GameManager.Instance.dayNightCycle == null ||
            GameManager.Instance.dayNightCycle.CurrentPhase != NightWatchPhase.Night)
        {
            return;
        }

        // Usa un seme raccolto dal player
        if (SeedManager.Instance != null && SeedManager.Instance.UseSeedForPlanting())
        {
            planted = true;
            FlowerManager.Instance?.SpawnFlower(transform.position);
            Debug.Log("Fiore piantato!");
        }
    }
}