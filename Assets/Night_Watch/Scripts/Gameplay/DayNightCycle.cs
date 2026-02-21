using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Durata fase in secondi")]
    public float dayDuration = 20f;
    public float nightDuration = 20f;

    [Header("Colori luce")]
    public Color dayColor = Color.white;
    public Color nightColor = Color.blue;

    [Header("Intensità luce")]
    public float dayIntensity = 1f;
    public float nightIntensity = 0.4f;

    [Header("Luce da controllare")]
    public Light sceneLight;

    [HideInInspector] public NightWatchPhase CurrentPhase = NightWatchPhase.Day;

    private float phaseTimer;

    private void Start()
    {
        if (sceneLight == null)
        {
            Debug.LogWarning("DayNightCycle: nessuna luce assegnata!");
        }

        StartDay(); // Avvia sempre la fase di giorno all'inizio
    }

    private void Update()
    {
        phaseTimer -= Time.deltaTime;

        if (phaseTimer <= 0f)
        {
            SwitchPhase();
        }
    }

    private void SwitchPhase()
    {
        if (CurrentPhase == NightWatchPhase.Day)
        {
            StartNight();
        }
        else
        {
            StartDay();
        }
    }

    private void StartDay()
    {
        CurrentPhase = NightWatchPhase.Day;
        phaseTimer = dayDuration; // usa il valore dall’Inspector
        UpdateLighting();
    }

    private void StartNight()
    {
        CurrentPhase = NightWatchPhase.Night;
        phaseTimer = nightDuration; // usa il valore dall’Inspector
        UpdateLighting();
    }

    private void UpdateLighting()
    {
        if (sceneLight == null) return;

        if (CurrentPhase == NightWatchPhase.Day)
        {
            sceneLight.color = dayColor;
            sceneLight.intensity = dayIntensity;
        }
        else
        {
            sceneLight.color = nightColor;
            sceneLight.intensity = nightIntensity;
        }
    }
}
