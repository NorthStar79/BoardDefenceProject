using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class MainMenuController : MonoBehaviour
{
    [Header("Data")]
    public LevelDatabase database;

    [Header("UI")]
    public Transform listContainer;
    public GameObject levelButtonPrefab;

    private void OnEnable()
    {
        Rebuild();
    }

    public void Rebuild()
    {
        if (listContainer == null || levelButtonPrefab == null || database == null) return;

        for (int i = listContainer.childCount - 1; i >= 0; i--)
            Destroy(listContainer.GetChild(i).gameObject);

        for (int i = 0; i < database.Count; i++)
        {
            var entry = database.Get(i);
            if (entry == null) continue;

            var go = Object.Instantiate(levelButtonPrefab, listContainer);
            
            var view = go.GetComponent<LevelButtonView>();
            if (view == null)
            {
                Debug.LogError("MainMenuController: LevelButtonPrefab must have LevelButtonView component.");
                continue;
            }

            view.Bind(entry.displayName, entry.thumbnail, () =>
            {
                SceneLoader.Load(entry.gameplaySceneName, entry.level);
            });
        }
    }
}
