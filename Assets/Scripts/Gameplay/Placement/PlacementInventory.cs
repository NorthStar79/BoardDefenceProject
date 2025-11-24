using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class PlacementInventory : MonoBehaviour
{
    [System.Serializable]
    public class Entry { public DefenseItemType type; public int count; }

   [HideInInspector] public List<Entry> stock = new List<Entry>();

    public void LoadFromLevel(LevelConfig level)
    {
        stock.Clear();
        foreach (var e in level.towerInventory)
        {
            stock.Add(new Entry { type = e.towerType, count = e.count });
        }
    }

    public bool TryConsume(DefenseItemType t)
    {
        for (int i = 0; i < stock.Count; i++)
        {
            if (stock[i].type == t && stock[i].count > 0)
            {
                stock[i].count--;
                return true;
            }
        }
        return false;
    }

    public int Remaining(DefenseItemType t)
    {
        for (int i = 0; i < stock.Count; i++)
            if (stock[i].type == t) return stock[i].count;
        return 0;
    }
}
