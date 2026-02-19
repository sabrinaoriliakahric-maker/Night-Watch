using UnityEngine;

public enum GamePhase
{
    Day,
    Night
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GamePhase CurrentPhase = GamePhase.Day;
    public float phaseDuration = 20f; // secondi per fase

    private float phaseTimer;

    [Header("Riferimenti")]
    public SeedManager seedManager;
    public Light directionalLight;

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
        phaseTimer = phaseDuration;
        UpdateLighting();

        // Spawn semi iniziali solo se è giorno
        if (CurrentPhase == GamePhase.Day && seedManager != null)
        {
            seedManager.SpawnSeeds();
        }
    }

    private void Update()
    {
        phaseTimer -= Time.deltaTime;

        if (phaseTimer <= 0)
        {
            SwitchPhase();
            phaseTimer = phaseDuration;
        }
    }

    private void SwitchPhase()
    {
        if (CurrentPhase == GamePhase.Day)
        {
            CurrentPhase = GamePhase.Night;
            Debug.Log("Fase: Notte → Pianta i fiori con i semi raccolti");
        }
        else
        {
            CurrentPhase = GamePhase.Day;
            Debug.Log("Fase: Giorno → Spawn semi per raccogliere");

            // spawn nuovi semi di giorno
            if (seedManager != null)
            {
                seedManager.SpawnSeeds();
            }
        }

        UpdateLighting();
    }

    private void UpdateLighting()
    {
        if (directionalLight != null)
        {
            if (CurrentPhase == GamePhase.Day)
            {
                directionalLight.color = Color.white;
                directionalLight.intensity = 1f;
            }
            else
            {
                directionalLight.color = Color.blue;
                directionalLight.intensity = 0.4f;
            }
        }
    }
}
