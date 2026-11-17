using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Put this on settings panel 
// slider ui objects go in slider spots

public class AudioSettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Percent labels")]
    [SerializeField] private TextMeshProUGUI masterLabel;
    [SerializeField] private TextMeshProUGUI musicLabel;
    [SerializeField] private TextMeshProUGUI sfxLabel;

    private void OnEnable()
    {
        Hook(masterSlider, masterLabel, AudioChannel.Master);
        Hook(musicSlider, musicLabel, AudioChannel.Music);
        Hook(sfxSlider, sfxLabel, AudioChannel.SFX);
    }

    private void OnDisable()
    {
        Unhook(masterSlider);
        Unhook(musicSlider);
        Unhook(sfxSlider);

        GameAudio.Save(); 
    }

    
    private void OnApplicationPause(bool paused)
    {
        if (paused)
        {
            GameAudio.Save();
        }
    }

    private void Hook(Slider slider, TextMeshProUGUI label, AudioChannel channel)
    {
        if (slider == null) return;

        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.wholeNumbers = false;
        slider.SetValueWithoutNotify(GameAudio.GetVolume(channel));
        UpdateLabel(label, slider.value);

        slider.onValueChanged.RemoveAllListeners();
        slider.onValueChanged.AddListener(value =>
        {
            GameAudio.SetVolume(channel, value);
            UpdateLabel(label, value);
        });
    }

    private void Unhook(Slider slider)
    {
        if (slider != null)
        {
            slider.onValueChanged.RemoveAllListeners();
        }
    }

    private void UpdateLabel(TextMeshProUGUI label, float value)
    {
        if (label != null)
        {
            label.text = Mathf.RoundToInt(value * 100f) + "%";
        }
    }

    // Button On Click: sets every volume back to 100%
    public void ResetToDefaults()
    {
        GameAudio.ResetToDefaults();
        OnEnable(); 
    }
}