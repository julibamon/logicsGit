using UnityEngine;
using TMPro;
using System.Collections;

public class Viejitas : MonoBehaviour

{
    private bool isPlayerHere = false;

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



    void Update()
    {
        if(isPlayerHere && Input.GetKeyDown(KeyCode.E) && playerController.isGrounded) //tiene que estar en el suelo para poder interactuar
        {
            if (!isTalking) //si no está hablando la [E] empieza la conversacion
            {
                StartDialogue();
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
    }

    public void StartDialogue()
    {
        playerController.isInDialogue=true; //ponemos a true la variable is in dialogue para controlar en playercontroller y restringir movimiento etc
        isTalking = true;
        playerController.LookAtNPC(transform.position); //hacemos flip hacia las viejas si hace falta
        dialogueCanvas.SetActive(true); //abrimos el canvas de dialogo
        upCanvas.SetActive(false);      //quitamos el mensaje encima de las viejas "Hablar [E]"
        lineIndex=0;
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