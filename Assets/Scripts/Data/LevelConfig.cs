using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BoardDefense/LevelConfig")]
public sealed class LevelConfig : ScriptableObject
{
    [System.Serializable]
    public class TowerInventoryEntry
    {
        public DefenseItemType towerType;
        [Min(0)] public int count = 0;
    }

    public List<WaveConfig> waves = new List<WaveConfig>();
    [Min(1)] public int baseHP = 10;
    public List<TowerInventoryEntry> towerInventory = new List<TowerInventoryEntry>();
}
