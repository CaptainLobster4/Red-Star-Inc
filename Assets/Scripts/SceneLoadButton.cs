using UnityEngine;
using UnityEngine.SceneManagement;

// Attach to canvas
// type the scene name in the Inspector, then hook a Button's On Click to Load().
// The scene must be in File > Build Profiles > Scene List.
// Use several of these on one object if several buttons exist

public class SceneLoadButton : MonoBehaviour
{
    [Tooltip("Exact scene name, e.g. Collected Scene")]
    [SerializeField] private string sceneName = "Collected Scene";

    public void Load()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("SceneLoadButton: Scene Name is empty.");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogWarning("SceneLoadButton: scene '" + sceneName + "' isn't in the Build Profiles scene list (or the name is misspelled).");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }
}