using UnityEngine;
public sealed class UnityTimeSource : ITimeSource
{
    public float DeltaTime => Time.deltaTime;
    public float TimeScale { get => Time.timeScale; set => Time.timeScale = value; }
}
