using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public string sceneToLoad; //escena a la que vamos a cambiar
    public Vector3 spawnPoint; //sitio en el que vamos a aparecer
    public PlayerController player;

    public bool flipOnSpawn = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")){
            GameController.Instance.nextSpawnPosition = spawnPoint;
            GameController.Instance.useNextSpawn = true;
            GameController.Instance.currentHealthTP = player.currentHealth; //aplico los puntos de vida al gamecontroller en una variable externa al player(da problemas el currentSD al caragr el player de nuevo)
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