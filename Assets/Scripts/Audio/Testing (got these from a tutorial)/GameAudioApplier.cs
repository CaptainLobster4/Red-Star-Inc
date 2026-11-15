using System.Collections;
using FMODUnity;
using UnityEngine;

// Created automatically by GameAudio when the game starts. Waits until FMOD's master banks
// are loaded (buses don't exist before that), applies the saved volumes, then removes itself.
public class GameAudioApplier : MonoBehaviour
{
    private IEnumerator Start()
    {
        DontDestroyOnLoad(gameObject);

        while (!RuntimeManager.HaveMasterBanksLoaded)
        {
            yield return null;
        }

        GameAudio.ApplyAll();
        Destroy(gameObject);
    }
}