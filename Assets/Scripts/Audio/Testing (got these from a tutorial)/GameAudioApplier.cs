using System.Collections;
using FMODUnity;
using UnityEngine;

// applies the saved info from game audio
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