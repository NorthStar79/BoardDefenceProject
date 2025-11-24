using UnityEngine;

[DisallowMultipleComponent]
public sealed class TowerShooter : MonoBehaviour
{
    public DefenseItemType type;
    public LayerMask enemyLayer;
    public Transform firePoint;
    public ITargetSelector selector;
    [HideInInspector] public BoardGrid grid;

    private float _cooldown;

    private void Awake()
    {
        if (firePoint == null) firePoint = transform;
        if (grid == null) grid = FindFirstObjectByType<BoardGrid>();  //TODO EMRE inject this
        if (selector == null) selector = GetComponent<ITargetSelector>();
        if (selector == null) selector = gameObject.AddComponent<NearestAlongColumnSelector>();
        selector.Initialize(grid);
    }

    private void Update()
    {
        if (type == null || type.projectilePrefab == null || selector == null || grid == null) return;

        _cooldown -= Time.deltaTime;
        if (_cooldown > 0f) return;

        float rangeMeters = grid.BlocksToMeters(type.rangeBlocks);
        var target = selector.SelectTarget(transform.position, rangeMeters, enemyLayer);
        if (target == null) return;

        if (type.direction == FireDirectionMode.Forward)
        {
            var myCell = grid.WorldToCell(transform.position);
            var tCell = grid.WorldToCell(target.Transform.position);
            if (tCell.y <= myCell.y) return;
        }

        _cooldown = Mathf.Max(0f, type.intervalSeconds);

        Vector3 dir = (target.Transform.position - firePoint.position).normalized;
        if (dir.sqrMagnitude < 0.0001f) dir = transform.forward;
        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);

        GameObject proj =PoolManager.Instance.Get(type.projectilePrefab, firePoint.position, rot);

        var pc = proj.GetComponent<ProjectileController>();
        if (pc == null) pc = proj.AddComponent<ProjectileController>();
        float projSpeedMeters = grid.BlocksToMeters(type.projectileSpeedBlocksPerSec);
        pc.Initialize(target, type.damage, projSpeedMeters);
    }

    void OnTriggerEnter(Collider other)
    {
       if(other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        } 
    }
}
