using UnityEngine;

public enum FireDirectionMode { Forward, All }

[CreateAssetMenu(menuName = "BoardDefense/DefenseItemType")]
public sealed class DefenseItemType : ScriptableObject
{
    public string displayName = "Tower";
    [Min(0f)] public float damage = 1f;
    [Min(0f)] public float intervalSeconds = 3f;
    [Min(0f)] public float rangeBlocks = 4f;
    public FireDirectionMode direction = FireDirectionMode.Forward;
    [Tooltip("Does this tower gets killed when enemy touched it ?")]
    public bool canBeKilledByEnemy = true;

    [Header("Projectile")]
    public GameObject projectilePrefab;
    [Min(0f)] public float projectileSpeedBlocksPerSec = 8f;

   
}
