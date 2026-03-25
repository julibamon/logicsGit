using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeController : MonoBehaviour
{
    public static FadeController Instance;

    private Image fadeImage;
    public float fadeTime = 0.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        fadeImage = GameObject.Find("FadeImage")?.GetComponent<Image>();

        if (fadeImage != null)
        {
            SetAlpha(0f);
        }
    }

    private void SetAlpha(float a)
    {
        if (fadeImage == null) return;

        Color c = fadeImage.color;
        c.a = a;
        fadeImage.color = c;
    }

    public IEnumerator FadeOut()
    {
        float t = 0f;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            SetAlpha(Mathf.Lerp(0f, 1f, t / fadeTime));
            yield return null;
        }

        SetAlpha(1f);
    }

    public IEnumerator FadeIn()
    {
        float t = 0f;

        SetAlpha(1f);

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            SetAlpha(Mathf.Lerp(1f, 0f, t / fadeTime));
            yield return null;
        }

        SetAlpha(0f);
    }
}