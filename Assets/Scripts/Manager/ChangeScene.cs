using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public string sceneToLoad; //escena a la que vamos a cambiar
    public Vector2 spawnPoint; //sitio en el que vamos a aparecer

    public bool flipOnSpawn = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")){
            GameController.Instance.nextSpawnPosition = spawnPoint;
            GameController.Instance.useNextSpawn = true;

            GameController.Instance.flipOnSpawn=flipOnSpawn;
            Debug.Log("Tocando el TP");
            StartCoroutine(Transition(sceneToLoad));
        }

    
    }

    IEnumerator Transition(string sceneName)
    {
        yield return StartCoroutine(FadeController.Instance.FadeOut());

        SceneManager.LoadScene(sceneName);

        yield return null;

        yield return StartCoroutine(FadeController.Instance.FadeIn());

    }

    
}