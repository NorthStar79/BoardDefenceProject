using UnityEngine;

[DisallowMultipleComponent]
public sealed class NearestAlongAllDirectionsSelector : MonoBehaviour, ITargetSelector
{
    private BoardGrid grid;
    private readonly Collider[] _hits = new Collider[64];

     public void Initialize(BoardGrid g)
    {
        grid = g;
    }

    public IDamageable SelectTarget(Vector3 origin, float rangeMeters, LayerMask enemyLayer)
    {
        int count = Physics.OverlapSphereNonAlloc(origin, rangeMeters, _hits, enemyLayer);
        if (count <= 0) return null;

        IDamageable best = null;
        float bestSqr = float.PositiveInfinity;

        for (int i = 0; i < count; i++)
        {
            var c = _hits[i];
            if (c == null) continue;

            var dmg = c.GetComponentInParent<IDamageable>();
            if (dmg == null || !dmg.IsAlive) continue;

            float sqr = (dmg.Transform.position - origin).sqrMagnitude;
            if (sqr < bestSqr)
            {
                bestSqr = sqr;
                best = dmg;
            }
        }

        return best;
    }
}
