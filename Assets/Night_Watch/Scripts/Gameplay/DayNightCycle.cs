// Assets/Night_Watch/Scripts/Core/DayNightCycle.cs
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

    [HideInInspector] public NightWatchPhase CurrentPhase = NightWatchPhase.Day;

    private float phaseTimer;

    private void Start()
    {
        phaseTimer = dayDuration;
        UpdateLighting();
    }

    private void Update()
    {
        phaseTimer -= Time.deltaTime;

        if (phaseTimer <= 0)
        {
            SwitchPhase();
        }
    }

    private void SwitchPhase()
    {
        if (CurrentPhase == NightWatchPhase.Day)
        {
            CurrentPhase = NightWatchPhase.Night;
            phaseTimer = nightDuration;
        }
        else
        {
            CurrentPhase = NightWatchPhase.Day;
            phaseTimer = dayDuration;
        }

        UpdateLighting();
    }

    private void UpdateLighting()
    {
        if (CurrentPhase == NightWatchPhase.Day)
        {
            GetComponent<Light>().color = dayColor;
            GetComponent<Light>().intensity = dayIntensity;
        }
        else
        {
            GetComponent<Light>().color = nightColor;
            GetComponent<Light>().intensity = nightIntensity;
        }
    }
}
