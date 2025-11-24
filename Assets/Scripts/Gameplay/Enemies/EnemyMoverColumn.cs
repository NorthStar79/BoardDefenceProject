using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(EnemyLifecycle))]
public sealed class EnemyMoverColumn : MonoBehaviour
{
    public EnemyType type;
    [HideInInspector] public BoardGrid grid;
    private int targetRow;
    private int col;


    public void InitializeAtTopRowColumn(int startRow, int column, BoardGrid g)
    {
        grid = g;
        targetRow = Mathf.Clamp(startRow, 0, grid.rows - 1);
        col = Mathf.Clamp(column, 0, grid.columns - 1);
        transform.position = grid.CellToWorld(targetRow, col);
    }

    private void Update()
    {
        if (grid == null || type == null) return;

        float speedMeters = grid.BlocksToMeters(Mathf.Max(0f, type.moveSpeedBlocksPerSec));
        float step = speedMeters * Time.deltaTime;

        if (targetRow > 0)
        {
            Vector3 goal = grid.CellToWorld(targetRow, col);
            Vector3 to = goal - transform.position;

            if (to.magnitude <= step + 0.0001f)
            {
                transform.position = goal;
                targetRow--;
            }
            else
            {
                transform.position += to.normalized * step;
            }
        }
        else
        {
            Vector3 forwardBeyond = -grid.zForward.normalized;
            transform.position += forwardBeyond * step;
        }
    }
}
