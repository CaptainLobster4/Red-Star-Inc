using FMODUnity;
using UnityEngine;

public enum AudioChannel
{
    Master,
    Music,
    SFX
}


public static class GameAudio
{
    
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

    // Applies the volumes saved by Fmod on load
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void ApplySavedVolumesOnStart()
    {
        new GameObject("GameAudioApplier").AddComponent<GameAudioApplier>();
    }
}