using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

[DisallowMultipleComponent]
public sealed class HudResult : MonoBehaviour
{
    public GameObject root;
    public TMP_Text title;
    public Button RetunToMenuButton;

    private SignalHub hub;

    private void Awake()
    {
        hub = FindFirstObjectByType<SignalHub>();
        if (root != null) root.SetActive(false);
        if (RetunToMenuButton != null) RetunToMenuButton.onClick.AddListener(RetunToMenu);
    }

    private void OnEnable()
    {
        if (hub == null) return;
        hub.LevelWon += OnWin;
        hub.LevelLost += OnLose;
    }

    private void OnDisable()
    {
        if (hub == null) return;
        hub.LevelWon -= OnWin;
        hub.LevelLost -= OnLose;
    }

    private void OnWin() { Show("VICTORY"); }
    private void OnLose() { Show("DEFEAT"); }

    private void Show(string text)
    {
        if (title != null) title.text = text;
        if (root != null) root.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RetunToMenu()
    {
        Time.timeScale = 1f;
        SceneLoader.Load("Bootstrap", null);
    }
}
