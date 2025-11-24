using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-999)]
public sealed class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    [System.Serializable]
    public class PoolConfig
    {
        public GameObject prefab;
        [Min(0)] public int prewarm = 0;
        [Min(1)] public int hardCap = 32;
        [Tooltip("If true, instances can exceed hardCap with a warning; otherwise Get returns null at cap.")]
        public bool allowOverCapInstantiate = false;
    }

    [Header("Pools")]
    public List<PoolConfig> pools = new List<PoolConfig>();

    private readonly Dictionary<GameObject, Pool> _prefabToPool = new Dictionary<GameObject, Pool>();
    private readonly Dictionary<GameObject, Pool> _instanceToPool = new Dictionary<GameObject, Pool>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (var cfg in pools)
        {
            if (cfg?.prefab == null) continue;
            if (_prefabToPool.ContainsKey(cfg.prefab)) continue;

            var pool = new Pool(cfg.prefab, cfg.hardCap, cfg.allowOverCapInstantiate);
            _prefabToPool[cfg.prefab] = pool;

            // Prewarm
            for (int i = 0; i < cfg.prewarm; i++)
            {
                var go = pool.CreateNewInstance();
                Release(go);
            }
        }
    }


    public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null)
        {
            Debug.LogError("PoolManager.Get: prefab is null.");
            return null;
        }

        if (!_prefabToPool.TryGetValue(prefab, out var pool))
        {
            // Lazy-create pool with default cap
            pool = new Pool(prefab, 32, false);
            _prefabToPool[prefab] = pool;
        }

        var go = pool.Get(position, rotation);
        if (go != null) _instanceToPool[go] = pool;
        return go;
    }

    public void Release(GameObject instance)
    {
        if (instance == null) return; // destroyed or missing

        if (!_instanceToPool.TryGetValue(instance, out var pool))
        {
            var marker = instance.GetComponent<PoolMarker>();
            if (marker != null && marker.Origin != null && _prefabToPool.TryGetValue(marker.Origin, out pool))
            {
                _instanceToPool[instance] = pool;
            }
        }

        if (pool == null)
        {
            if (instance != null) instance.SetActive(false);
            return;
        }

        pool.Release(instance);
    }

    // -------------------- Internal Pool --------------------
    private sealed class Pool
    {
        private readonly GameObject _prefab;
        private readonly Stack<GameObject> _stack = new Stack<GameObject>(64);
        private int _liveCount;
        private readonly int _cap;
        private readonly bool _allowOverCap;

        public Pool(GameObject prefab, int cap, bool allowOverCap)
        {
            _prefab = prefab;
            _cap = Mathf.Max(1, cap);
            _allowOverCap = allowOverCap;
            _liveCount = 0;
        }

        public GameObject CreateNewInstance()
        {
            var go = Object.Instantiate(_prefab);
            go.name = _prefab.name + "_Pooled";

            var marker = go.GetComponent<PoolMarker>();
            if (marker == null) marker = go.AddComponent<PoolMarker>();
            marker.Origin = _prefab;

            _liveCount++;
            return go;
        }

        public GameObject Get(Vector3 position, Quaternion rotation)
        {
            GameObject go = null;

            while (_stack.Count > 0)
            {
                go = _stack.Pop();
                if (go != null) break;            
                _liveCount = Mathf.Max(0, _liveCount - 1);
                go = null;
            }

            if (go == null)
            {
                if (_liveCount < _cap)
                {
                    go = CreateNewInstance();
                }
                else
                {
                    Debug.LogWarning($"[Pool] Hard cap reached for '{_prefab.name}' ({_cap}). " +
                                    (_allowOverCap ? "Instantiating beyond cap." : "Returning null."));
                    if (_allowOverCap)
                    {
                        go = CreateNewInstance();
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            //position + activate
            go.transform.SetPositionAndRotation(position, rotation);

            var poolables = go.GetComponentsInChildren<IPoolable>(true);
            foreach (var p in poolables) p.OnSpawned();

            go.SetActive(true);
            return go;
        }

        public void Release(GameObject go)
        {
            if (go == null)
            {
                _liveCount = Mathf.Max(0, _liveCount - 1);
                return;
            }

            var poolables = go.GetComponentsInChildren<IPoolable>(true);
            foreach (var p in poolables) p.OnDespawned();

            go.SetActive(false);
            _stack.Push(go);
        }
    }
}
public sealed class PoolMarker : MonoBehaviour
{
    public GameObject Origin;
}
