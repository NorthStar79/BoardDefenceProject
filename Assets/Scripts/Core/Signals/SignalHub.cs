using System;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public sealed class SignalHub : MonoBehaviour
{
    public static SignalHub Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // --- Events ---
    public event Action<int,int,int> BaseDamaged;           // newHP, maxHP, delta(negative)
    public event Action BaseDestroyed;
    public event Action<int,int> WaveStarted;               // index, total
    public event Action AllWavesCompleted;
    public event Action LevelWon;
    public event Action LevelLost;

    // --- Raisers ---
    public void RaiseBaseDamaged(int newHP, int maxHP, int delta) => BaseDamaged?.Invoke(newHP, maxHP, delta);
    public void RaiseBaseDestroyed() => BaseDestroyed?.Invoke();
    public void RaiseWaveStarted(int index, int total) => WaveStarted?.Invoke(index, total);
    public void RaiseAllWavesCompleted() => AllWavesCompleted?.Invoke();
    public void RaiseLevelWon() => LevelWon?.Invoke();
    public void RaiseLevelLost() => LevelLost?.Invoke();
}
