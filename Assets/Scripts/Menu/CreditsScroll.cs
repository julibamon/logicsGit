
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class CreditsScroll : MonoBehaviour
{
    public RectTransform scrollTransform;

    public GameObject grid;

    public float speed = 39f;

    public float endY = 2000f;

    public string mainMenuSceneName = "Menu";

    private bool scrolling = false;

    public void StartScroll()
    {
        scrolling = true;
    }

    void Update()
    {
        if (!scrolling) return;

        scrollTransform.anchoredPosition += Vector2.up * 100f * Time.deltaTime;

        if (scrollTransform.anchoredPosition.y >= endY)
        {
            StartCoroutine(End());
            
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }

   private IEnumerator End()
    {
        grid.SetActive(false);
        yield return new WaitForSeconds(6f);
        SceneManager.LoadScene(mainMenuSceneName);
    }
}