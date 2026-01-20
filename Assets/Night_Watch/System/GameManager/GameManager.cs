using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GamePhase CurrentPhase { get; private set; }
    public int Level { get; private set; } = 1;

    public event Action<GamePhase> OnPhaseChanged;
    public event Action<int> OnLevelChanged;
    public event Action OnGameOver;
public enum GamePhase
{
    Day,
    Night,
    Ship,
    GameOver
}

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
    }

    public void SetPhase(GamePhase phase)
    {
        CurrentPhase = phase;
        OnPhaseChanged?.Invoke(phase);
    }

    public void NextLevel()
    {
        Level++;
        OnLevelChanged?.Invoke(Level);
        SetPhase(GamePhase.Day);
    }

    public void GameOver()
    {
        SetPhase(GamePhase.GameOver);
        OnGameOver?.Invoke();
    }
}
