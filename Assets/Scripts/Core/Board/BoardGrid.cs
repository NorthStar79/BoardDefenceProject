using UnityEngine;

[DisallowMultipleComponent]
public sealed class BoardGrid : MonoBehaviour
{
    [Header("Grid")]
    [Min(1)] public int columns = 4;
    [Min(1)] public int rows = 8;
    [Min(0.1f)] public float tileSize = 1.0f;

    public GameObject cellPrefab;
    public Transform cellsParent;

    [Header("Axes & Origin")]
    public Vector3 origin = Vector3.zero;
    public Vector3 xRight = Vector3.right;   // column direction
    public Vector3 zForward = Vector3.forward; // row direction

    public void GenerateGrid()
    {
        if (cellPrefab == null) { Debug.LogError("BoardGrid.GenerateGrid: cellPrefab is not assigned."); return; }
        if (cellsParent == null) { Debug.LogError("BoardGrid.GenerateGrid: cellsParent is not assigned."); return; }

        for (int row = 0; row < rows; row++)
        for (int col = 0; col < columns; col++)
        {
            Vector3 worldPos = CellToWorld(row, col);
            var cell = Instantiate(cellPrefab, worldPos, Quaternion.identity, cellsParent);
            cell.name = $"Cell_{row}_{col}";
        }
    }

    public Vector3 CellToWorld(int row, int col)
    {
        Vector3 p = origin;
        p += (col + 0.5f) * tileSize * xRight.normalized;
        p += (row + 0.5f) * tileSize * zForward.normalized;
        return p;
    }

    public Vector2Int WorldToCell(Vector3 world)
    {
        Vector3 local = world - origin;
        float colF = Vector3.Dot(local, xRight.normalized) / tileSize - 0.5f;
        float rowF = Vector3.Dot(local, zForward.normalized) / tileSize - 0.5f;
        int col = Mathf.Clamp(Mathf.RoundToInt(colF), 0, columns - 1);
        int row = Mathf.Clamp(Mathf.RoundToInt(rowF), 0, rows - 1);
        return new Vector2Int(col, row);
    }

    public bool IsBottomHalf(int rowIndex) => rowIndex < rows / 2;
    public float BlocksToMeters(float blocks) => blocks * tileSize;
}
