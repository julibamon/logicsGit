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
        }
        else
        {
            Destroy(gameObject);
        }
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
}