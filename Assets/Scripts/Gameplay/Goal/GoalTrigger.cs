using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[DisallowMultipleComponent]
public sealed class GoalTrigger : MonoBehaviour
{
    public BaseHealthSystem baseHealth;

    private void Awake()
    {
        var col = GetComponent<BoxCollider>();
        col.isTrigger = true;
        if (baseHealth == null) baseHealth = FindFirstObjectByType<BaseHealthSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (baseHealth == null) return;
        var dmg = other.GetComponentInParent<IDamageable>();
        if (dmg != null)
        {
            baseHealth.Damage(Mathf.Max(1, dmg.Attack));
            PoolManager.Instance.Release((dmg as Component).gameObject);
        }
    }
}
