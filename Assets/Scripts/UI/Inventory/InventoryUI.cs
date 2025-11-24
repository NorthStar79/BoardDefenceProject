using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class InventoryUI : MonoBehaviour
{
    [Header("References")]
    public PlacementInventory inventory;
    public GameObject itemButtonPrefab; //InventoryItemView
    public Transform container;
    public TowerDragManager dragManager;

    private readonly List<InventoryItemView> _spawned = new List<InventoryItemView>();

    private void OnEnable()
    {
        if (dragManager != null) dragManager.PlacementDone += OnPlacementDone;
    }

    private void OnDisable()
    {
        if (dragManager != null) dragManager.PlacementDone -= OnPlacementDone;
    }

    void Start()
    {
        Rebuild();
    }

    private void OnPlacementDone(DefenseItemType t)
    {
        RefreshCounts();
    }

    public void Rebuild()
    {
        if (container == null || itemButtonPrefab == null || inventory == null) return;

        for (int i = container.childCount - 1; i >= 0; i--)
            Destroy(container.GetChild(i).gameObject);
        _spawned.Clear();

        foreach (var e in inventory.stock)
        {
            if (e.type == null) continue;
            var go = Instantiate(itemButtonPrefab, container);
            var view = go.GetComponent<InventoryItemView>();
            view.Bind(e.type, inventory.Remaining(e.type));
            view.onBeginDragItem = HandleBeginDrag;
            _spawned.Add(view);
        }
    }

    private void RefreshCounts()
    {
        if (inventory == null) return;
        for (int i = 0; i < _spawned.Count; i++)
        {
            var v = _spawned[i];
            if (v == null || v.Type == null) continue;
            v.SetCount(inventory.Remaining(v.Type));
        }
    }

    private void HandleBeginDrag(DefenseItemType type, Vector2 screenPosition)
    {
        if (dragManager != null)
        {
            dragManager.BeginDrag(type, screenPosition);
        }
    }
}
