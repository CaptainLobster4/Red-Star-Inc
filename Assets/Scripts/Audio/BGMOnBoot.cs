using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class PersistentMusicManager : MonoBehaviour
{
    public static PersistentMusicManager Instance { get; private set; }

    [SerializeField] private EventReference menuMusicEvent;
    private EventInstance musicInstance;

    private void Awake()
    {
        // If an instance already exists (e.g., returning to Main Menu), destroy this duplicate
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        StartMusic();
    }

    private void StartMusic()
    {
        musicInstance = RuntimeManager.CreateInstance(menuMusicEvent);
        musicInstance.start();
    }

    // Optional: Call this if you ever want to stop or fade out the music later
    public void StopMusic(bool allowFadeOut = true)
    {
        if (!musicInstance.isValid())
        {
            return;
        }

        musicInstance.stop(allowFadeOut ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT : FMOD.Studio.STOP_MODE.IMMEDIATE);
        musicInstance.release();
        musicInstance = default;
    }

    private void OnDestroy()
    {
        if (Instance != this)
        {
            return;
        }

        StopMusic();
        Instance = null;
    }
}
