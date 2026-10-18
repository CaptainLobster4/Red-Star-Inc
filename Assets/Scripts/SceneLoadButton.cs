using UnityEngine;
using UnityEngine.SceneManagement;

// Generic "go to this scene" helper. Put it on any object (the Canvas is fine),
// type the scene name in the Inspector, then hook a Button's On Click to Load().
// The scene must be in File > Build Profiles > Scene List.
// Use several of these on one object if you need several buttons.
//
// It also remembers which scene the player came FROM, so a screen like the
// Collection can have a Back button that returns to wherever they were:
//   - Buttons that open the screen call Load()  (this records the scene being left)
//   - The Back button calls LoadPrevious()      (goes to the recorded scene)
// If nothing was recorded (e.g. you pressed Play directly in the Collection scene,
// or the player arrived through another script), LoadPrevious() uses Scene Name as a fallback.
public class SceneLoadButton : MonoBehaviour
{
    [Tooltip("Load() opens this scene (e.g. Collected Scene). LoadPrevious() uses it only as a fallback when no previous scene is known.")]
    [SerializeField] private string sceneName = "Collected Scene";

    // Shared across all SceneLoadButtons; lives until the game closes
    private static string previousSceneName;

    public void Load()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("SceneLoadButton: Scene Name is empty.");
            return;
        }

        string current = SceneManager.GetActiveScene().name;
        if (current != sceneName)
        {
            previousSceneName = current;
        }

        LoadScene(sceneName);
    }

    // Go back to the scene the player was in before the last Load()
    public void LoadPrevious()
    {
        string target = string.IsNullOrEmpty(previousSceneName) ? sceneName : previousSceneName;

        if (string.IsNullOrEmpty(target))
        {
            Debug.LogWarning("SceneLoadButton: no previous scene recorded and Scene Name (fallback) is empty.");
            return;
        }

        LoadScene(target);
    }

    private void LoadScene(string target)
    {
        if (!Application.CanStreamedLevelBeLoaded(target))
        {
            Debug.LogWarning("SceneLoadButton: scene '" + target + "' isn't in the Build Profiles scene list (or the name is misspelled).");
            return;
        }

        SceneManager.LoadScene(target);
    }
}