using UnityEngine;

// Persists across scenes so the Collectibles menu can be checked from
// anywhere. Put this on an empty GameObject in your first-loaded scene.
public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Unlock(CollectibleData collectible)
    {
        if (collectible == null)
        {
            return;
        }

        PlayerPrefs.SetInt(GetKey(collectible), 1);
        PlayerPrefs.Save();
    }

    public bool IsUnlocked(CollectibleData collectible)
    {
        if (collectible == null)
        {
            return false;
        }

        return PlayerPrefs.GetInt(GetKey(collectible), 0) == 1;
    }

    private string GetKey(CollectibleData collectible)
    {
        return "Collectible_" + collectible.collectibleName;
    }
}