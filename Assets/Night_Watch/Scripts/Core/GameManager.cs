using UnityEngine;

public enum GameState
{
    Gameplay,
    Pausa,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Riferimento al DayNightCycle")]
    public DayNightCycle dayNightCycle;

    [Header("Stato iniziale")]
    public GameState currentState = GameState.Gameplay;

    private float savedPhaseTimer;
    private GameState previousState;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        previousState = currentState;

        if (currentState == GameState.Gameplay && dayNightCycle != null)
        {
            dayNightCycle.enabled = true;
            SeedManager.Instance?.SpawnSeeds(); // spawn semi all'avvio
        }
    }

    private void Update()
    {
        HandleInput();
        UpdateGameState();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Gameplay)
                SetGameState(GameState.Pausa);
            else if (currentState == GameState.Pausa)
                SetGameState(GameState.Gameplay);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentState == GameState.GameOver)
                SetGameState(GameState.Gameplay);
        }
    }

    private void UpdateGameState()
    {
        if (dayNightCycle == null) return;

        switch (currentState)
        {
            case GameState.Gameplay:
                // ✅ abilita il ciclo giorno/notte senza resettare il timer
                dayNightCycle.enabled = true;
                break;

            case GameState.Pausa:
                // salva il timer corrente e disabilita il ciclo
                savedPhaseTimer = GetPhaseTimer();
                dayNightCycle.enabled = false;
                break;

            case GameState.GameOver:
                dayNightCycle.enabled = false;
                dayNightCycle.CurrentPhase = NightWatchPhase.Day;
                SetPhaseTimer(dayNightCycle.dayDuration);
                dayNightCycle.GetType().GetMethod("UpdateLighting",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(dayNightCycle, null);

                // reset semi
                SeedManager.Instance?.ClearAllSeeds();
                break;
        }
    }

    private void SetGameState(GameState newState)
    {
        previousState = currentState;
        currentState = newState;
        Debug.Log("Stato di gioco: " + currentState);

        // SPAWN semi solo se entri in Gameplay da GameOver o all'avvio
        if (currentState == GameState.Gameplay && previousState != GameState.Pausa)
        {
            SeedManager.Instance?.SpawnSeeds();
        }
    }

    private float GetPhaseTimer()
    {
        var field = dayNightCycle.GetType().GetField("phaseTimer",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return field != null ? (float)field.GetValue(dayNightCycle) : 0f;
    }

    private void SetPhaseTimer(float value)
    {
        var field = dayNightCycle.GetType().GetField("phaseTimer",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
            field.SetValue(dayNightCycle, value);
    }
}
