using UnityEngine;

[DisallowMultipleComponent]
public sealed class PlacementValidator : MonoBehaviour
{
    public BoardGrid grid;
    public PlacementInventory inventory;

    private void Awake()
    {
        if (grid == null) grid = FindFirstObjectByType<BoardGrid>();
        if (inventory == null) inventory = FindFirstObjectByType<PlacementInventory>();
    }

    public bool CanPlace(DefenseItemType type, Vector3 worldPosition)
    {
        if (grid == null || type == null || inventory == null) return false;
        var cell = grid.WorldToCell(worldPosition);
        if (!grid.IsBottomHalf(cell.y)) return false;
        if (inventory.Remaining(type) <= 0) return false;
        return true;
    }

    public bool TryPlace(DefenseItemType type, Vector3 worldPosition, out Vector3 snappedPosition)
    {
        snappedPosition = worldPosition;
        if (!CanPlace(type, worldPosition)) return false;
        var cell = grid.WorldToCell(worldPosition);
        snappedPosition = grid.CellToWorld(cell.y, cell.x);
        return inventory.TryConsume(type);
    }
}
