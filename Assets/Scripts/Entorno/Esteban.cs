using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Esteban : MonoBehaviour
{
    private bool isPlayerHere = false;

    private Animator animator;

    public GameObject upCanvas; //texto encima del npc
    public GameObject dialogueCanvas;

    private PlayerController playerController;

    [SerializeField, TextArea(4,7)] private string[] dialogueArea; //lineas de dialogo del npc
    //TextArea(minimo,maximo) numero de lineas en el cuadro de texto

    [SerializeField] private TMP_Text dialogueText;

    private bool isTalking; //hemos empezado el dialogo
    private int lineIndex ; //indica qué linea de dialogo se está mostrando

    public float typingTime = 0.05f;
    private bool playerIsGrounded;
    private bool lineFinished; //cuadro de dialogo mostrado entero ya

    private bool isInside;

    //floorPoints para reposicionar al player cuando esté en dialogo con las viejitas
    public GameObject floorPoint;


    //COLLIDERS

    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void Start() //se decide de manera aleatoria si Esteban empieza sentado o de pie
    {
        int randomAnim = Random.Range(0, 2);
        if (randomAnim == 0){
            animator.SetBool("wantToSit", false);
        }else{
            animator.SetBool("wantToSit", true);

        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlayerHere && Input.GetKeyDown(KeyCode.E) && playerController.isGrounded) //tiene que estar en el suelo para poder interactuar
        {
            if (!isTalking) //si no está hablando la [E] empieza la conversacion
            {
                StartCoroutine(MoveToTalkPoint());
            } else {
                HandleInput();
            
           }
        }
        //CONTROL ANIMACION HABLAR
        if(isTalking  && !lineFinished) //si estoy en dialogo y aun quedan cosas por decir
        {
            animator.SetBool("isTalking", true); //que salga animacion de hablar
        }
        else
        {
            animator.SetBool("isTalking", false); //que vuelva a idle
        }
    }

    //corrutina de mover al player al lado del npc al punto concreto para hablar
    private IEnumerator MoveToTalkPoint()
    {
        isTalking=true;
        playerController.isInDialogue=true; //ponemos a true la variable is in dialogue para controlar en playercontroller y restringir movimiento etc

        Transform selectedPoint = floorPoint.transform;

        //movemos al player
        while(Vector2.Distance(playerController.transform.position, selectedPoint.position)> 0.05f)
        {
            playerController.transform.position = Vector2.MoveTowards(playerController.transform.position, selectedPoint.position, 1.5f*Time.deltaTime);
        
            yield return null;        
        }

        playerController.LookAtNPC(transform.position); //hacemos flip para mirar a la derecha si hace falta y activamos la animacion Idle del player

        StartDialogue();
    }

//empezar dialogo
    public void StartDialogue()
    {   //empezamos el dialogo random entre las opciones del dialogueArea

        dialogueCanvas.SetActive(true); //abrimos el canvas de dialogo
        upCanvas.SetActive(false);      //quitamos el mensaje encima del NPC "Hablar [E]"

        int numRandom = Random.Range(0, dialogueArea.Length); //el ultimo no incluido [0, fin)

        lineIndex = numRandom;

        PlayDialogue("EstebanVoice");

        StartCoroutine(LinesCoroutine()); //llamamos a la corrutina

    }

private void NextDialogue()
    {

        //No vamos a pasar por otros dialogos porque este NPC solo te dice una cosa por habitacion, asi que al darle a la [E] cierra dialogo
        EndDialogue();

        
    }


    //terminar dialogo
    private void EndDialogue()
    {
        isTalking = false; //dejamos de hablar si ya no quedan más lineas por decir
        dialogueCanvas.SetActive(false); //desactivamos el panel
        upCanvas.SetActive(true); //para volver a mostrar el canvas de arriba
        playerController.isInDialogue=false;
        lineIndex = 0;
    }

     private void HandleInput() //manejo inputs
{
    if (!Input.GetKeyDown(KeyCode.E)) {
        return;
        }
    if (!lineFinished)
    {
        StopAllCoroutines();
        dialogueText.text = dialogueArea[lineIndex];
        lineFinished = true;

    
        return;
    }

    NextDialogue();
}
    //corrutina para que se vaya escribiendo poco a poco el texto
    private IEnumerator LinesCoroutine()
    {   
        lineFinished=false;
        dialogueText.text = string.Empty; //para que empiece como string vacio
        
        foreach (char ch in dialogueArea[lineIndex]) //por cada linea de dialogo
        {
            dialogueText.text += ch; //concatenamos cada character uno por uno
            yield return new WaitForSeconds(typingTime); //tiempo que tarda en escribirse cada character
        }
        lineFinished=true;
        
    }

    //lógica de entrar y salir del rango del npc

    void OnTriggerEnter2D(Collider2D colliderPlayer)
    {
        if (colliderPlayer.CompareTag("Player"))
        {
            isPlayerHere= true;
            playerController = colliderPlayer.GetComponent<PlayerController>(); //para inicializar el player
            upCanvas.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D colliderPlayer)
    {
        if (colliderPlayer.CompareTag("Player"))
        {
            isPlayerHere=false;
            playerController=null;
            upCanvas.SetActive(false);
        }
    }

        //sonido en primer plano
    public void PlayDialogue(string soundName) { 
        SoundEffectManager.Instance.PlayDialogue(soundName, false);
     } 
     

}
