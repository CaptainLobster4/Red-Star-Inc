using UnityEngine;
using UnityEngine.SceneManagement;

// Temporary debugging helper: prints one Console line for every scene that loads.
// Not attached to anything. Delete it when the wrong-scene bug is found.
public static class SceneLoadLogger
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("[SceneLog] loaded '" + scene.name + "' (build index " + scene.buildIndex + ", " + mode + ") at " + Time.realtimeSinceStartup.ToString("F1") + "s");
    }
}