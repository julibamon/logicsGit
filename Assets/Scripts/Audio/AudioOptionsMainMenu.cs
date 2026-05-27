using UnityEngine;
using UnityEngine.UI;

public class AudioOptionsMainMenu : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
       /*  musicSlider.SetValueWithoutNotify(AudioSettingsManager.Instance.musicVolume);
        sfxSlider.SetValueWithoutNotify(AudioSettingsManager.Instance.sfxVolume); */
    }

   /*  public void ChangeMusicVolume(float value)
    {
        AudioSettingsManager.Instance.musicVolume = value;

        if (MusicManager.Instance != null)
        {
            //MusicManager.Instance.SetVolume();
        }
    } */

    /* public void ChangeSFXVolume(float value)
    {
        AudioSettingsManager.Instance.sfxVolume = value;

        if (SoundEffectManager.Instance != null)
        {
            //SoundEffectManager.Instance.SetVolume();
        }
    } */
}