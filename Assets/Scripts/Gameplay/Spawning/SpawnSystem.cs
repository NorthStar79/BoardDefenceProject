using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class SpawnSystem : MonoBehaviour
{
    public LevelConfig level;
    public PrefabCatalog catalog;
    public BoardGrid grid;

    private SignalHub hub;

    private void Awake()
    {
        if (grid == null) grid = FindFirstObjectByType<BoardGrid>();
        hub = FindFirstObjectByType<SignalHub>();
    }

    public void Begin()
    {
        StopAllCoroutines();
        StartCoroutine(Co_Run());
    }

    private IEnumerator Co_Run()
    {
        if (level == null || grid == null) yield break;
        int total = level.waves.Count;
        for (int i = 0; i < total; i++)
        {
            var wave = level.waves[i];
            if (wave.preWaveDelaySeconds > 0f) yield return new WaitForSeconds(wave.preWaveDelaySeconds);
            hub?.RaiseWaveStarted(i + 1, total);
            yield return StartCoroutine(Co_SpawnWave(wave));
            if (wave.postWaveDelaySeconds > 0f) yield return new WaitForSeconds(wave.postWaveDelaySeconds);
        }
        hub?.RaiseAllWavesCompleted();
    }

    private IEnumerator Co_SpawnWave(WaveConfig wave)
    {
        int topRow = grid.rows - 1;

        foreach (var e in wave.entries)
        {
            if (e.enemyType == null || e.count <= 0) continue;
            var prefab = catalog != null ? catalog.GetEnemyPrefab(e.enemyType) : e.enemyType.prefab;
            if (prefab == null) { Debug.LogError($"SpawnSystem: No prefab for {e.enemyType.name}"); continue; }

            for (int k = 0; k < e.count; k++)
            {
                int col = Random.Range(0, grid.columns);
                Vector3 pos = grid.CellToWorld(topRow, col);
                Quaternion rot = Quaternion.identity;

                GameObject go = PoolManager.Instance.Get(prefab, pos, rot);
                var mover = go.GetComponent<EnemyMoverColumn>();
                if (mover == null) mover = go.AddComponent<EnemyMoverColumn>();
                mover.type = e.enemyType;
                mover.grid = grid;
                mover.InitializeAtTopRowColumn(topRow, col, grid);

                if (e.spawnEverySeconds > 0f && k < e.count - 1)
                    yield return new WaitForSeconds(e.spawnEverySeconds);
            }
        }
    }
}
