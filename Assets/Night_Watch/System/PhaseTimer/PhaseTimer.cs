using UnityEngine;
using System.Collections;
using static GameManager;

public class PhaseTimer : MonoBehaviour
{
    public float phaseDuration = 20f;
    private float timer;

    private void Start()
    {
        GameManager.Instance.OnPhaseChanged += OnPhaseChanged;
    }

    void OnPhaseChanged(GamePhase phase)
    {
        if (phase == GameManager.GamePhase.Day || phase == GameManager.GamePhase.Night)
        {
            StopAllCoroutines();
            StartCoroutine(TimerRoutine());
        }
    }

    IEnumerator TimerRoutine()
    {
        timer = phaseDuration;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        if (GameManager.Instance.CurrentPhase == GameManager.GamePhase.Day)
            GameManager.Instance.SetPhase(GameManager.GamePhase.Night);
        else
            GameManager.Instance.SetPhase(GameManager.GamePhase.Ship);
    }
}
