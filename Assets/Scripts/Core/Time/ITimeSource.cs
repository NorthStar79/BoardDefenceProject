public interface ITimeSource
{
    float DeltaTime { get; }
    float TimeScale { get; set; }
}
