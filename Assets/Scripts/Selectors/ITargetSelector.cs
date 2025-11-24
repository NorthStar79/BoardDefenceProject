using UnityEngine;

public interface ITargetSelector
{
    public void Initialize(BoardGrid grid);
    IDamageable SelectTarget(Vector3 origin, float rangeMeters, LayerMask enemyLayer);
}
