using FMODUnity;
using UnityEngine;

public enum AudioChannel
{
    Master,
    Music,
    SFX
}

// Player volume settings, saved on the device and applied to the FMOD buses.
// Not a MonoBehaviour: nothing to attach. The saved volumes are applied automatically when the game starts.
//
// IMPORTANT: the bus paths below must match the buses in your FMOD Studio project
// (FMOD Studio > Mixer window > the bus names). "bus:/" is the Master bus.
// Anything that should follow a slider must be routed to that bus in FMOD Studio, and the banks rebuilt.
public static class GameAudio
{
    // Bus names from the FMOD project (found with FmodBusLister).
    // Music slider = MX. SFX slider = SFX plus UI (button clicks). Master = the master bus.
    public static string[] GetBusPaths(AudioChannel channel)
    {
        switch (channel)
        {
            case AudioChannel.Music: return new[] { "bus:/MX" };
            case AudioChannel.SFX: return new[] { "bus:/SFX", "bus:/UI" };
            default: return new[] { "bus:/" };
        }
    }

    private const string KeyPrefix = "Audio_";
    private static bool warned = false;

    // 0 (silent) to 1 (full). Defaults to full.
    public static float GetVolume(AudioChannel channel)
    {
        return PlayerPrefs.GetFloat(KeyPrefix + channel, 1f);
    }

    // Changes the volume right away and remembers it. Call Save() when the player is done (the menu does).
    public static void SetVolume(AudioChannel channel, float volume)
    {
        volume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(KeyPrefix + channel, volume);
        ApplyToFmod(channel, volume);
    }

    public static void Save()
    {
        PlayerPrefs.Save();
    }

    public static void ResetToDefaults()
    {
        foreach (AudioChannel channel in System.Enum.GetValues(typeof(AudioChannel)))
        {
            SetVolume(channel, 1f);
        }
        Save();
    }

    // Applies every saved volume to FMOD (done once at start, after FMOD has loaded its banks)
    public static void ApplyAll()
    {
        foreach (AudioChannel channel in System.Enum.GetValues(typeof(AudioChannel)))
        {
            ApplyToFmod(channel, GetVolume(channel));
        }
    }

    private static void ApplyToFmod(AudioChannel channel, float volume)
    {
        foreach (string path in GetBusPaths(channel))
        {
            try
            {
                FMOD.Studio.Bus bus = RuntimeManager.GetBus(path);
                bus.setVolume(volume);
            }
            catch (System.Exception e)
            {
                if (!warned)
                {
                    warned = true;
                    Debug.LogWarning("GameAudio: couldn't set the volume on bus '" + path + "'. Check the bus names in GameAudio.GetBusPaths against your FMOD Studio project and rebuild the banks. " + e.Message);
                }
            }
        }
    }

    // Starts a small helper that waits for FMOD's banks to load, then applies the saved volumes
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void ApplySavedVolumesOnStart()
    {
        new GameObject("GameAudioApplier").AddComponent<GameAudioApplier>();
    }
}