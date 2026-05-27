using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    private Slider slider;

    void Start()
    {
        slider = GetComponent<Slider>();

        //sincr volumen actual
        slider.value = AudioSettingsManager.Instance.musicVolume;

        //cambios
        slider.onValueChanged.AddListener(OnVolumeChanged);
    }

    void OnVolumeChanged(float value)
    {
        MusicManager.Instance.SetVolume(value);
    }
}