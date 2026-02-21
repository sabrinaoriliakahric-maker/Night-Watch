using UnityEngine;

public class PlantingZone : MonoBehaviour, IInteractable
{
    private bool planted = false;
    private Renderer rend;

    [Header("Effetto particellare quando si pianta un fiore")]
    public GameObject plantVFXPrefab;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    private void Start()
    {
        // All'inizio è disattivato (verrà attivato da PlantingZoneManager di notte)
        gameObject.SetActive(false);
    }

    public void Interact()
    {
        // Se già piantato, non fare nulla
        if (planted) return;

        // Verifica che sia notte
        if (GameManager.Instance == null ||
            GameManager.Instance.dayNightCycle == null ||
            GameManager.Instance.dayNightCycle.CurrentPhase != NightWatchPhase.Night)
        {
            Debug.Log("Puoi piantare solo di notte!");
            return;
        }

        // Usa un seme raccolto dal player
        if (SeedManager.Instance != null && SeedManager.Instance.UseSeedForPlanting())
        {
            planted = true;
            
            // Spawna il fiore
            FlowerManager.Instance?.SpawnFlower(transform.position);
            
            // Spawna effetto particellare se assegnato
            if (plantVFXPrefab != null)
            {
                Instantiate(plantVFXPrefab, transform.position, Quaternion.identity);
            }
            
            Debug.Log("Fiore piantato!");
            
            // Disattiva la zona dopo aver piantato
            gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Non hai semi da piantare!");
        }
    }

    /// <summary>
    /// Resetta la zona per il prossimo ciclo
    /// </summary>
    public void ResetZone()
    {
        planted = false;
    }
}
