using UnityEngine;

[DisallowMultipleComponent]
public sealed class NearestAlongColumnSelector : MonoBehaviour, ITargetSelector
{
    private BoardGrid grid;

    [Tooltip("Only consider targets at the same or greater row index (forward along column).")]
    public bool aheadOnly = true;

    // Non-alloc physics buffer
    private readonly Collider[] _hits = new Collider[64];

    public void Initialize(BoardGrid g)
    {
        grid = g;
    }

    public IDamageable SelectTarget(Vector3 origin, float rangeMeters, LayerMask enemyLayer)
    {
        if (grid == null) return null;

        int count = Physics.OverlapSphereNonAlloc(origin, rangeMeters, _hits, enemyLayer);
        if (count <= 0) return null;

        var myCell = grid.WorldToCell(origin);

        IDamageable best = null;
        int bestRowDelta = int.MaxValue; 

        for (int i = 0; i < count; i++)
        {
            var c = _hits[i];
            if (c == null) continue;

            var dmg = c.GetComponentInParent<IDamageable>();
            if (dmg == null || !dmg.IsAlive) continue;

            //same column
            var tCell = grid.WorldToCell(dmg.Transform.position);
            if (tCell.x != myCell.x) continue;

            if (aheadOnly && tCell.y < myCell.y) continue;

            int rowDelta = Mathf.Abs(tCell.y - myCell.y);
            if (rowDelta < bestRowDelta)
            {
                bestRowDelta = rowDelta;
                best = dmg;
            }
        }

        return best;
    }
}
