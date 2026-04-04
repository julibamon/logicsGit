using UnityEngine;

public class Gramola : MonoBehaviour

{
    public Animator animator;
    private bool isPlayerHere = false;

    public GameObject canvas; //texto encima de la gramola

    private PlayerController playerController;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if(isPlayerHere && Input.GetKeyDown(KeyCode.E))
        {
            ActivateGramola();
        }
    }

    public void ActivateGramola()
    {
        animator.SetTrigger("Clicked");

        //recuperar vida maxima
        playerController.currentHealth=playerController.maxHealth;
        playerController.UpdateHealthUI();

       playerController.UpdateDataPlayer(); //llamamos al metodo UpdateDataPlayer definido en player para actualizar los datos de guardado

        GameController.Instance.SaveGame();

        Debug.Log("Partida guardada en una gramola");
    }

    //lógica de entrar en rango de la gramola y salir de él

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