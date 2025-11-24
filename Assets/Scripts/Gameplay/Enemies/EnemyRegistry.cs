using UnityEngine;

[DefaultExecutionOrder(-900)]
public sealed class EnemyRegistry : MonoBehaviour
{
    public static EnemyRegistry Instance { get; private set; }
    [SerializeField] private int alive;
    public int AliveCount => alive;

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterEnemy() { alive++; }
    public void UnregisterEnemy() { alive = Mathf.Max(0, alive - 1); }
}
