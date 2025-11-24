using UnityEngine;

[DisallowMultipleComponent]
public sealed class EnemyLifecycle : MonoBehaviour, IDamageable, IPoolable
{
    public EnemyType type;
    [SerializeField] private float currentHP;
    [SerializeField] private bool registered;
    public Transform Transform => transform;
    public bool IsAlive => currentHP > 0f;

    public int Attack => type.Attack;


    private void OnEnable()
    {
        currentHP = Mathf.Max(1f, type != null ? type.maxHP : 5f);
        if (!registered && EnemyRegistry.Instance != null)
        {
            EnemyRegistry.Instance.RegisterEnemy();
            registered = true;
        }
    }

    private void OnDisable()
    {
        if (registered && EnemyRegistry.Instance != null)
        {
            EnemyRegistry.Instance.UnregisterEnemy();
            registered = false;
        }
    }

    public void TakeDamage(float amount)
    {
        if (currentHP <= 0f) return;
        currentHP -= Mathf.Max(0f, amount);
        if (currentHP <= 0f)
        {
            currentHP = 0f;
            PoolManager.Instance.Release(gameObject);
        }
    }

    // IPoolable
    public void OnSpawned() {  }
    public void OnDespawned() {  }
}
