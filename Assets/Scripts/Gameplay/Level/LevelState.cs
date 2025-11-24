using UnityEngine;

[DisallowMultipleComponent]
public sealed class LevelState : MonoBehaviour
{
    private bool wavesDone;
    private bool ended;

    private void OnEnable()
    {
        SignalHub.Instance.BaseDestroyed += OnLose;
        SignalHub.Instance.AllWavesCompleted += OnWavesDone;
    }

    private void OnDisable()
    {
        SignalHub.Instance.BaseDestroyed -= OnLose;
        SignalHub.Instance.AllWavesCompleted -= OnWavesDone;
    }

    private void Update()
    {
        if (!ended && wavesDone && EnemyRegistry.Instance != null && EnemyRegistry.Instance.AliveCount == 0)
        {
            ended = true;
            SignalHub.Instance.RaiseLevelWon();
        }
    }

    private void OnWavesDone() { wavesDone = true; }
    private void OnLose()
    {
        if (ended) return;
        ended = true;
        SignalHub.Instance.RaiseLevelLost();
    }
}
