using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BoardDefense/PrefabCatalog")]
public sealed class PrefabCatalog : ScriptableObject
{
    [System.Serializable]
    public class EnemyMap { public EnemyType type; public GameObject prefab; }
    [System.Serializable]
    public class DefenseMap { public DefenseItemType type; public GameObject prefab; }

    public List<EnemyMap> enemies = new List<EnemyMap>();
    public List<DefenseMap> defenses = new List<DefenseMap>();

    public GameObject GetEnemyPrefab(EnemyType t)
    {
        for (int i = 0; i < enemies.Count; i++) if (enemies[i].type == t) return enemies[i].prefab;
        return null;
    }

    public GameObject GetDefensePrefab(DefenseItemType t)
    {
        for (int i = 0; i < defenses.Count; i++) if (defenses[i].type == t) return defenses[i].prefab;
        return null;
    }
}
