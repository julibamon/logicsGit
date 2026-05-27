using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    private AudioSource audioSource;

    private AudioClip currentClip;

    //booleano que indica desde los TP si cambiamos la musica o continúa por donde iba
    private bool preserveMusicOnNextLoad = false;
    public AudioMixer mainMixer;


    

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);

            audioSource = GetComponent<AudioSource>();


            //SetVolume();
          


        }
        else
        {
            Destroy(gameObject);
        }
    }

    //CAMBIOS
    void Start()
        {
            //En start se pone el volumen del audio, que lo recoge de AudioSettingsManager, que mantiene el valor del volumen entre escenas
            SetVolume(AudioSettingsManager.Instance.musicVolume);
        }



    //metodo llamado por los TP para dejar la misma musica o no segun booleano
    public void SetPreserveMusic(bool preserve)
    {
        preserveMusicOnNextLoad = preserve;
    }

    //metodo play al que llamamos al cargar la escena
   public void PlaySceneMusic(AudioClip newClip) {
        if (!preserveMusicOnNextLoad && newClip == null)
        {
            audioSource.Stop();
            currentClip = null;
            preserveMusicOnNextLoad = false;
            return;
        }

        if (preserveMusicOnNextLoad && currentClip == newClip)
        {
            preserveMusicOnNextLoad = false;
            return;
        }

        if (newClip == null)
            return;
            currentClip = newClip;

        audioSource.Stop();
        audioSource.clip = newClip;
        //SetVolume();


        if (AudioSettingsManager.Instance != null)
            {
                SetVolume(AudioSettingsManager.Instance.musicVolume);
            }
        audioSource.Play();



        preserveMusicOnNextLoad = false;
    }   


//configuracion filtro pasa baja (para la muerte)
    public void SetDeathFilter(bool active)
{
    if (active)
    {
        mainMixer.SetFloat("DeathFilterCutoff", 780f);
    }
    else
    {
        mainMixer.SetFloat("DeathFilterCutoff", 22000f);
    }
}

//desactivar el filtro pasa baja al hacer replay
public void SetGameplayState()
{
    SetDeathFilter(false);
}

//cambiar volumen musica
/* public void SetVolume()
    {
        if (AudioSettingsManager.Instance == null) return;

        audioSource.volume = Mathf.Clamp01(AudioSettingsManager.Instance.musicVolume);
    }*/

 // Prueba control volumen


        //Método para controlar el volumen del mixer de audio
    public void SetVolume(float volume)
    {
        

        if (AudioSettingsManager.Instance != null)
        {
            AudioSettingsManager.Instance.musicVolume = volume;
        }
        float mixerVolume = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;

            mainMixer.SetFloat("MusicVolume", mixerVolume);
    }
}