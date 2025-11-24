using UnityEngine;

[DisallowMultipleComponent]
public sealed class BaseHealthSystem : MonoBehaviour
{
    private int maxHP = 10;
    private int currentHP;


    private void Awake()
    {
        currentHP = Mathf.Max(1, maxHP);
    }

    public void ResetHP(int value)
    {
        maxHP = Mathf.Max(1, value);
        currentHP = maxHP;
       SignalHub.Instance.RaiseBaseDamaged(currentHP, maxHP, 0);
    }

    public void Damage(int amount)
    {
        amount = Mathf.Max(0, amount);
        currentHP = Mathf.Max(0, currentHP - amount);
        SignalHub.Instance.RaiseBaseDamaged(currentHP, maxHP, -amount);
        if (currentHP <= 0) SignalHub.Instance.RaiseBaseDestroyed();
    }
}
