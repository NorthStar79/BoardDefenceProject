using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    private static LevelConfig _pendingLevel;

    public static void Load(string sceneName, LevelConfig level)
    {
        _pendingLevel = level;
        SceneManager.LoadSceneAsync(sceneName).completed += OnSceneLoaded;

    }

    private static void OnSceneLoaded(AsyncOperation operation)
    {
       var bootstrap = Object.FindFirstObjectByType<LevelBootstrap>();
        if (bootstrap != null)
        {
            bootstrap.InitializeLevel(_pendingLevel);
        }
    }

}
