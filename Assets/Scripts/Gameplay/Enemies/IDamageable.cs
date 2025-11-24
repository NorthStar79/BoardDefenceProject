using Unity.VisualScripting;
using UnityEngine;

public interface IDamageable
{
    int Attack { get; }
    Transform Transform { get; }
    bool IsAlive { get; }
    void TakeDamage(float amount);
}
