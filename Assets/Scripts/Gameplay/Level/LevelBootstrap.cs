using UnityEngine;

[DisallowMultipleComponent]
public sealed class LevelBootstrap : MonoBehaviour
{
    private LevelConfig level;

    [Header("Scene Services")]
    public BaseHealthSystem baseHealth;
    public PlacementInventory placementInventory;
    public SpawnSystem spawner;
    public BoardGrid grid;

    [Header("Options")]
    public bool resetTimeScaleOnStart = true;

    public void InitializeLevel(LevelConfig levelConfig)
    {
        level = levelConfig;

        if (resetTimeScaleOnStart) Time.timeScale = 1f;

        if (level == null)
        {
            Debug.LogError("LevelBootstrap: LevelConfig is not assigned.");
            return;
        }

        if(grid == null)
        {
            Debug.LogError("LevelBootstrap: grid is not assigned.");
            return;
        }
        else
        {
            grid.GenerateGrid();
        }

        if (baseHealth != null)
        {
            baseHealth.ResetHP(level.baseHP);
        }
        else
        {
            Debug.LogWarning("LevelBootstrap: BaseHealthSystem not assigned.");
        }

        if (placementInventory != null)
        {
            placementInventory.LoadFromLevel(level);
        }
        else
        {
            Debug.LogWarning("LevelBootstrap: PlacementInventory not assigned (tower counts won't be enforced).");
        }

        if (spawner != null)
        {
            spawner.Begin();
        }
        else
        {
            Debug.LogWarning("LevelBootstrap: SpawnSystem not assigned (waves won't start).");
        }
    }
}
