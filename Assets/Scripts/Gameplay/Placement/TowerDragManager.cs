using System;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class TowerDragManager : MonoBehaviour
{
    [Header("Refs")]
    public Camera worldCamera;
    public BoardGrid grid;
    public PlacementValidator validator;
    public PlacementInventory inventory;
    public PrefabCatalog catalog;

    [Header("Ghost")]
    public Color validColor = new Color(0f, 1f, 0f, 0.35f);
    public Color invalidColor = new Color(1f, 0f, 0f, 0.35f);
    public Material GhostMaterial;
    public float yOffset = 0.0f;

    public event Action<DefenseItemType> PlacementDone;

    private bool _dragActive;
    private DefenseItemType _dragType;
    private GameObject _ghost;
    private GhostTint _ghostTint;
    private Vector3 _snappedPos;
    private bool _canPlace;
    private Plane _groundPlane;

    private void Awake()
    {
        if (worldCamera == null) worldCamera = Camera.main;
        if (grid == null) grid = FindFirstObjectByType<BoardGrid>();
        if (validator == null) validator = FindFirstObjectByType<PlacementValidator>();
        if (inventory == null) inventory = FindFirstObjectByType<PlacementInventory>();
        if (catalog == null)
        {
            var all = Resources.FindObjectsOfTypeAll<PrefabCatalog>();
            if (all != null && all.Length > 0) catalog = all[0];
        }
        _groundPlane = new Plane(Vector3.up, grid != null ? grid.origin : Vector3.zero);
    }

    private void Update()
    {
        if (!_dragActive) return;

        Vector2 screen = Input.touchCount > 0 ? (Vector2)Input.GetTouch(0).position : (Vector2)Input.mousePosition;
        Ray ray = worldCamera != null ? worldCamera.ScreenPointToRay(screen) : new Ray(Vector3.zero, Vector3.down);
        if (_groundPlane.Raycast(ray, out float enter))
        {
            Vector3 hit = ray.GetPoint(enter);
            var cell = grid.WorldToCell(hit);
            _snappedPos = grid.CellToWorld(cell.y, cell.x) + Vector3.up * yOffset;

            if (_ghost != null)
            {
                _ghost.transform.position = _snappedPos;
            }     
            

            _canPlace = validator != null && validator.CanPlace(_dragType, _snappedPos);
            if (_ghostTint != null) _ghostTint.SetTint(_canPlace ? validColor : invalidColor);
        }

        bool released = false;
        if (Input.touchCount > 0)
        {
            var t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled) released = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            released = true;
        }

        if (released)
        {
            _ghostTint.SetGhostMode(false);
            EndDrag(true);
        }
    }

    public void BeginDrag(DefenseItemType type, Vector2 initialScreenPosition)
    {
        if (type == null || grid == null || catalog == null) return;
        if (inventory != null && inventory.Remaining(type) <= 0) return;

        CancelGhost();

        _dragActive = true;
        _dragType = type;

        var towerPrefab = catalog.GetDefensePrefab(type);
        if (towerPrefab == null)
        {
            Debug.LogError($"TowerDragManager: No prefab found for {type.name} in PrefabCatalog.");
            _dragActive = false;
            return;
        }

        _ghost = Instantiate(towerPrefab);
        _ghost.name = $"{towerPrefab.name}_Ghost";
        foreach (var c in _ghost.GetComponentsInChildren<Collider>(true)) c.enabled = false;
        foreach (var mb in _ghost.GetComponentsInChildren<MonoBehaviour>(true)) mb.enabled = false;
        
        _ghostTint = _ghost.GetComponent<GhostTint>();
        if (_ghostTint == null) _ghostTint = _ghost.AddComponent<GhostTint>();
        _ghostTint.ghostMaterialOverride = GhostMaterial;
        _ghostTint.SetGhostMode(true);
        _ghostTint.SetTint(invalidColor);

    }

    public void CancelDrag() => EndDrag(false);

    private void EndDrag(bool place)
    {
        if (!_dragActive) { CancelGhost(); return; }

        if (place && _canPlace && validator != null)
        {
            if (validator.TryPlace(_dragType, _snappedPos, out var snapped))
            {
                var towerPrefab = catalog.GetDefensePrefab(_dragType);
                var tower = Instantiate(towerPrefab, snapped, Quaternion.identity);
                PlacementDone?.Invoke(_dragType);
            }
        }

        _dragActive = false;
        _dragType = null;
        CancelGhost();
    }

    private void CancelGhost()
    {
        if (_ghost != null) Destroy(_ghost);
        _ghost = null;
        _ghostTint = null;
    }
}
