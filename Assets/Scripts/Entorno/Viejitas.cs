using UnityEngine;
using TMPro;
using System.Collections;

public class Viejitas : MonoBehaviour

{
    private bool isPlayerHere = false;

    private Animator animator;

    public GameObject upCanvas; //texto encima del comestible

    public GameObject dialogueCanvas;

    private PlayerController playerController;

    [SerializeField, TextArea(4,6)] private string[] dialogueArea; //lineas de dialogo de las viejitas
    //TextArea(minimo,maximo) numero de lineas en el cuadro de texto

    [SerializeField] private TMP_Text dialogueText;

    private bool isTalking; //hemos empezado el dialogo
    private int lineIndex ; //indica qué linea de dialogo se está mostrando

    public float typingTime = 0.05f;

    private bool playerIsGrounded;

    //floorPoints para reposicionar al player cuando esté en dialogo con las viejitas
    public GameObject floorPointLeft;
    public GameObject floorPointRight;


    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if(isPlayerHere && Input.GetKeyDown(KeyCode.E) && playerController.isGrounded) //tiene que estar en el suelo para poder interactuar
        {
            if (!isTalking) //si no está hablando la [E] empieza la conversacion
            {
                StartCoroutine(MoveToTalkPoint());
            } else if (dialogueText.text == dialogueArea[lineIndex]) //si ya hemos mostrado todos los caracteres de ese dialogo (la condicion coincide)
            {                                                         //la [E] pasa al siguiente dialogo
                NextDialogue(); //pasamos al siguiente dialogo

            }
            else //si aun no se han mostrado todos las frases del cuadro de dialogo, la [E] acelera el tipeo por si no quieres esperar
            {
                StopAllCoroutines();
                dialogueText.text = dialogueArea[lineIndex]; //mostramos todos los caracteres que faltan
            }
           
        }
        //CONTROL ANIMACION VIEJITAS
        if(isTalking && lineIndex < dialogueArea.Length && dialogueText.text != dialogueArea[lineIndex]) //si estoy en dialogo y aun quedan cosas por decir
        {
            animator.SetBool("isTalking", true); //que salga animacion de hablar
        }
        else
        {
            animator.SetBool("isTalking", false); //que vuelva a idle
        }
    }
    //corrutina de mover al player al lado de las viejas al punto concreto para hablar
    private IEnumerator MoveToTalkPoint()
    {
        isTalking=true;
        playerController.isInDialogue=true; //ponemos a true la variable is in dialogue para controlar en playercontroller y restringir movimiento etc

        Transform selectedPoint;

        if(playerController.transform.position.x < transform.position.x) //las viejitas estan a la derecha del floorpoint
        {
            selectedPoint = floorPointLeft.transform;
        }
        else    //las viejitas estan a la izq
        {
            selectedPoint = floorPointRight.transform;
        }

        //movemos al player
        while(Vector2.Distance(playerController.transform.position, selectedPoint.position)> 0.05f)
        {
            playerController.transform.position = Vector2.MoveTowards(playerController.transform.position, selectedPoint.position, 1.5f*Time.deltaTime);
        
            yield return null;        
        }

        playerController.LookAtNPC(transform.position); //hacemos flip hacia las viejas si hace falta

        StartDialogue();
    }



    //empezar dialogo
    public void StartDialogue()
    {
        dialogueCanvas.SetActive(true); //abrimos el canvas de dialogo
        upCanvas.SetActive(false);      //quitamos el mensaje encima de las viejas "Hablar [E]"
        if (GameController.Instance.currentSD.worldData.activatedEvents.Contains("ViejitasCheck")) //si ya conocemos a las viejitas
        {
            lineIndex=6; //empezamos el dialogo en index 6
        }
        else //si no las conocemos
        {
            lineIndex=0; //empezamos desde el principio
        }
        StartCoroutine(LinesCoroutine()); //llamamos a la corrutina

    }

    private void NextDialogue()
    {
        lineIndex++; //incrementamos el index del dialogo
        if (lineIndex < dialogueArea.Length) //si aún quedan lineas de dialogo por las que pasar, SEGUIMOS EN LA CONVERSACION
        {
            StartCoroutine(LinesCoroutine());
        }
        else //HEMOS TERMINADO LA CONVERSACION
        {
            isTalking = false; //dejamos de hablar si ya no quedan más lineas por decir
            dialogueCanvas.SetActive(false); //desactivamos el panel
            upCanvas.SetActive(true); //para volver a mostrar el canvas de arriba
            playerController.isInDialogue=false;
            lineIndex = 0;
            GameController.Instance.currentSD.worldData.activatedEvents.Add("ViejitasCheck");

        }
    }

    //corrutina para que se vaya escribiendo poco a poco el texto
    private IEnumerator LinesCoroutine()
    {
        dialogueText.text = string.Empty; //para que empiece como string vacio
        
        foreach (char ch in dialogueArea[lineIndex]) //por cada linea de dialogo
        {
            dialogueText.text += ch; //concatenamos cada character uno por uno
            yield return new WaitForSeconds(typingTime); //tiempo que tarda en escribirse cada character
        }
    }

    //lógica de entrar y salir del rango de las viejas

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

}