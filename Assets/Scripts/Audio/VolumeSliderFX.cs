using UnityEngine;
using UnityEngine.UI;

public class VolumeSliderFX : MonoBehaviour
{
    private Slider slider;

    void Start()
    {
        slider = GetComponent<Slider>();

        //sincr volumen actual
        slider.value = AudioSettingsManager.Instance.sfxVolume;

        //cambios
        slider.onValueChanged.AddListener(OnVolumeChanged);
    }

    void OnVolumeChanged(float value)
    {
        SoundEffectManager.Instance.SetVolume(value);
    }
}