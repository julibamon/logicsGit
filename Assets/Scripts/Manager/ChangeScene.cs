using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public string sceneToLoad; //escena a la que vamos a cambiar
    public Vector2 spawnPoint; //sitio en el que vamos a aparecer

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")){
            GameController.Instance.nextSpawnPosition = spawnPoint;
            GameController.Instance.useNextSpawn = true;
            Debug.Log("Tocando el TP");
            SceneManager.LoadScene(sceneToLoad);
        }

    
    }

    
}