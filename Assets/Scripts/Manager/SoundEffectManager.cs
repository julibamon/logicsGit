using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SoundEffectManager : MonoBehaviour
{
    public static SoundEffectManager Instance;

    private static AudioSource audioSource;
    private static SoundEffectLibrary soundEffectLibrary;

    [SerializeField] private Slider sfxSlider;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
            soundEffectLibrary = GetComponent<SoundEffectLibrary>();

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    //SONIDOS PRIMER PLANO
    public void Play(string soundName, bool randomPitch)
    {
        AudioClip audioClip = soundEffectLibrary.GetRandomClip(soundName);
        if(audioClip != null)
        {
            if (randomPitch)
            {
                audioSource.pitch = Random.Range(0.7f, 1.01f);
            }
            
            audioSource.PlayOneShot(audioClip);
        }
    }


    //SONIDOS QUE NO SE SUPERPONEN (no son playoneshot) PARA DIALOGO, si suena uno se quita el anterior
        public void PlayDialogue(string soundName, bool randomPitch)
    {
        AudioClip audioClip = soundEffectLibrary.GetRandomClip(soundName);

        if(audioClip != null)
        {
            audioSource.Stop();

            if (randomPitch)
            {
                audioSource.pitch = Random.Range(0.7f, 1.01f);
            }
            else
            {
                audioSource.pitch = 1f;
            }

            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }

    //SONIDOS SEGÚN DISTANCIA
   public void PlayAtPosition(string soundName, Vector3 position, bool randomPitch)
{
    AudioClip clip = soundEffectLibrary.GetRandomClip(soundName);
    if (clip == null) return;

    float maxDistance = 6f;

    Vector3 camPos = Camera.main.transform.position;

    
    Vector2 cam2D = new Vector2(camPos.x, camPos.y);
    Vector2 pos2D = new Vector2(position.x, position.y);

    float dist = Vector2.Distance(cam2D, pos2D);

    float volume = Mathf.Clamp01(1f - (dist / maxDistance));

    GameObject go = new GameObject("2DSound");
    AudioSource source = go.AddComponent<AudioSource>();

    source.clip = clip;
    source.spatialBlend = 0f;
    source.volume = volume;

    if (randomPitch)
        source.pitch = Random.Range(0.7f, 1.01f);

    source.Play();

    Object.Destroy(go, clip.length);
}
  
}
