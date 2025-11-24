using UnityEngine;

[CreateAssetMenu(menuName = "BoardDefense/EnemyType")]
public sealed class EnemyType : ScriptableObject
{
    public string displayName = "Enemy";
    [Min(1)] public float maxHP = 5f;
    [Min(0f)] public float moveSpeedBlocksPerSec = 1f;
    [Min(0f)] public float collisionRadiusBlocks = 0.3f;
    [Min(1)] public int Attack = 1;
    public GameObject prefab;
}
