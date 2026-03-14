using UnityEngine;

public class ComestibleDobleSalto : MonoBehaviour

{
    private bool isPlayerHere = false;

    public GameObject canvas; //texto encima del comestible

    private PlayerController playerController;

    public GameObject comestible;

    void Update()
    {
        if(isPlayerHere && Input.GetKeyDown(KeyCode.E))
        {
            EatComestible();
        }
    }

    public void EatComestible()
    {

        comestible.SetActive(false);
        Debug.Log("Comido el comestible");
    }

    //lógica de entrar en rango del comestible

    void OnTriggerEnter2D(Collider2D colliderPlayer)
    {
        if (colliderPlayer.CompareTag("Player"))
        {
            isPlayerHere= true;
            playerController = colliderPlayer.GetComponent<PlayerController>(); //para inicializar el player
            canvas.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D colliderPlayer)
    {
        if (colliderPlayer.CompareTag("Player"))
        {
            isPlayerHere=false;
            playerController=null;
            canvas.SetActive(false);
        }
    }

}