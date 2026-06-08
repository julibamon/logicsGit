using UnityEngine;

public class SceneMusicData : MonoBehaviour
{
    public AudioClip musicClip;

    void Start()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.SetGameplayState();
            MusicManager.Instance.PlaySceneMusic(musicClip);
        }
    }
}