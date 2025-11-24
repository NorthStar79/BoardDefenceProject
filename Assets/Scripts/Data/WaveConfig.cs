using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BoardDefense/WaveConfig")]
public sealed class WaveConfig : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        public EnemyType enemyType;
        [Min(1)] public int count = 5;
        [Min(0f)] public float spawnEverySeconds = 0.5f;
    }

    public List<Entry> entries = new List<Entry>();
    [Min(0f)] public float preWaveDelaySeconds = 0f;
    [Min(0f)] public float postWaveDelaySeconds = 0f;
}
