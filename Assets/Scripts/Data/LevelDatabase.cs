using UnityEngine;

[CreateAssetMenu(menuName = "BoardDefense/Level Database")]
public sealed class LevelDatabase : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        public string displayName = "Level 1";
        public LevelConfig level;
        public string gameplaySceneName = "Gameplay";
        public Sprite thumbnail;
    }

    public Entry[] levels;

    public int Count => levels != null ? levels.Length : 0;
    public Entry Get(int index) => (levels != null && index >= 0 && index < levels.Length) ? levels[index] : null;
}
