using UnityEngine;

[DisallowMultipleComponent]
public sealed class ProjectileController : MonoBehaviour, IPoolable
{
    [Min(0f)] public float speed = 10f;
    [Min(0.01f)] public float hitRadius = 0.25f;
    [Min(0f)] public float maxLifetimeSeconds = 4f;

    private IDamageable _target;
    private float _life;


    public void Initialize(IDamageable target, float damage, float projectileSpeed)
    {
        _target = target;
        _pendingDamage = Mathf.Max(0f, damage);
        speed = Mathf.Max(0f, projectileSpeed);
        _life = 0f;
    }

    private float _pendingDamage;

    private void OnEnable() { _life = 0f; }
    private void Update()
    {
        _life += Time.deltaTime;
        if (_life >= maxLifetimeSeconds) { Despawn(); return; }

        transform.position += transform.forward * (speed * Time.deltaTime);

        if (_target == null || !_target.IsAlive) return;

        float sqr = (transform.position - _target.Transform.position).sqrMagnitude;
        if (sqr <= hitRadius * hitRadius)
        {
            _target.TakeDamage(_pendingDamage);
            Despawn();
        }
    }

    private void Despawn()
    {
        PoolManager.Instance.Release(gameObject);
    }

    // IPoolable
    public void OnSpawned() { _life = 0f; }
    public void OnDespawned() { _target = null; _pendingDamage = 0f; }
}
