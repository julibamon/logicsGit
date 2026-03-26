using UnityEngine;
using TMPro;
using System.Collections;
using UnityEditor.Search;
using Unity.Collections;

public class Viejitas : MonoBehaviour

{
    private bool isPlayerHere = false;

    private Animator animator;

    public GameObject upCanvas; //texto encima del npc
    public GameObject dialogueCanvas;

    private PlayerController playerController;

    [SerializeField, TextArea(4,7)] private string[] dialogueArea; //lineas de dialogo de las viejitas
    //TextArea(minimo,maximo) numero de lineas en el cuadro de texto

    [SerializeField] private TMP_Text dialogueText;

    private bool isTalking; //hemos empezado el dialogo
    private int lineIndex ; //indica qué linea de dialogo se está mostrando

    public float typingTime = 0.05f;

    private bool isShowingOptions=false; //para cuando estamos mostrando "opciones"(botones) y no texto del inspector

    private bool playerIsGrounded;
    private bool lineFinished; //cuadro de dialogo mostrado entero ya

    //floorPoints para reposicionar al player cuando esté en dialogo con las viejitas
    public GameObject floorPointLeft;
    public GameObject floorPointRight;


    //botones opciones recetas
    public GameObject tomatoIncorrectButton;
    public GameObject tomatoCorrectButton;

    public GameObject saltIncorrectButton;
    public GameObject saltCorrectButton;

    public GameObject oilIncorrectButton;
    public GameObject oilCorrectButton;

    public GameObject noIdeaButton;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if(isPlayerHere && Input.GetKeyDown(KeyCode.E) && playerController.isGrounded && !isShowingOptions) //tiene que estar en el suelo para poder interactuar y que no salgan opciones
        {
            if (!isTalking) //si no está hablando la [E] empieza la conversacion
            {
                StartCoroutine(MoveToTalkPoint());
            } else {
                HandleInput();
            
           }
        }
        //CONTROL ANIMACION VIEJITAS
        if(isTalking && !isShowingOptions && !lineFinished) //si estoy en dialogo y aun quedan cosas por decir
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
        if (AllDelivered())
        {
            lineIndex=12;
        }

        else if (GameController.Instance.currentSD.worldData.activatedEvents.Contains("ViejitasCheck")) //si ya conocemos a las viejitas
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

        //VOLVER A OPCIONES TRAS ELEGIR CORRECTA O INCORRECTA (si no hemos entregado ya todas)
        if(lineIndex==7 || lineIndex == 9)
        {
            lineIndex=6;
            StartCoroutine(LinesCoroutine());
            return;
        }
        //NO TENEMOS NI IDEA, se cierra dialogo directamente, o ya hemos entregado todo y solo nos dicen "ja,ja"
        if (lineIndex == 8 || (AllDelivered() && lineIndex == 12))
        {
            EndDialogue();
            return;
        }

        lineIndex++;

        StartCoroutine(LinesCoroutine());

        
    }

    //terminar dialogo
    private void EndDialogue()
    {
        isTalking = false; //dejamos de hablar si ya no quedan más lineas por decir
        dialogueCanvas.SetActive(false); //desactivamos el panel
        upCanvas.SetActive(true); //para volver a mostrar el canvas de arriba
        playerController.isInDialogue=false;
        lineIndex = 0;
        if (!GameController.Instance.currentSD.worldData.activatedEvents.Contains("ViejitasCheck"))
            {
                GameController.Instance.currentSD.worldData.activatedEvents.Add("ViejitasCheck");
            }
    }
    private void HandleInput() //manejo inputs
{
    if (!Input.GetKeyDown(KeyCode.E)) {
        return;
        }

    if (isShowingOptions) {
        return;
    }
    if (!lineFinished)
    {
        StopAllCoroutines();
        dialogueText.text = dialogueArea[lineIndex];
        lineFinished = true;

        if (lineIndex == 6 && !isShowingOptions && !AllDelivered())
    {
        ShowOptions();
    }
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
        //que aparezcan las opciones
        if (lineIndex == 6 && !isShowingOptions && !AllDelivered()) //que salgan las opciones abajo pero despues de que salga toda la pregunta
            {
                ShowOptions();
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

    //para contestar a las viejas
     private void ShowOptions()
    {
    isShowingOptions=true;
        //TOMATE
        if (!GameController.Instance.currentSD.worldData.activatedEvents.Contains("TomateOK")) //si no he entregado todavia
        {
            bool hasTomate = GameController.Instance.currentSD.worldData.itemsListW.Contains("RecetaTomate");
            tomatoCorrectButton.SetActive(hasTomate);
            tomatoIncorrectButton.SetActive(!hasTomate);
        }
        else //si ya he entregado, que no me salga la opcion
        {
            tomatoCorrectButton.SetActive(false);
            tomatoIncorrectButton.SetActive(false);
        }
    

    //SAL
        if (!GameController.Instance.currentSD.worldData.activatedEvents.Contains("SaltOK")){ //si no he entregado todavia
            bool hasSal = GameController.Instance.currentSD.worldData.itemsListW.Contains("RecetaSal");
            saltCorrectButton.SetActive(hasSal);
            saltIncorrectButton.SetActive(!hasSal);
            }
            else
            {
            saltCorrectButton.SetActive(false);
            saltIncorrectButton.SetActive(false);
        }
    //ACEITE
        if (!GameController.Instance.currentSD.worldData.activatedEvents.Contains("OilOK")){ //si no he entregado todavia
            bool hasAceite = GameController.Instance.currentSD.worldData.itemsListW.Contains("RecetaAceite");
            oilCorrectButton.SetActive(hasAceite);
            oilIncorrectButton.SetActive(!hasAceite);
        }
        else
        {
            oilCorrectButton.SetActive(false);
            oilIncorrectButton.SetActive(false);
        }
    //SKIP
    noIdeaButton.SetActive(!AllDelivered()); //depende de si se han entregado los 3 o no
    }

    //desactivar las opciones
    private void HideOptions()
    {
        tomatoCorrectButton.SetActive(false);
        tomatoIncorrectButton.SetActive(false);
        saltCorrectButton.SetActive(false);
        saltIncorrectButton.SetActive(false);
        oilCorrectButton.SetActive(false);
        oilIncorrectButton.SetActive(false);
        noIdeaButton.SetActive(false);
    }

    
    
    //CUANDO LE DAMOS A UNA OPCION CORRECTA
    public void CorrectOption(string ingrediente)
    {
        isShowingOptions=false;
        HideOptions();
        string receta = ingrediente + "OK";
        if (!GameController.Instance.currentSD.worldData.activatedEvents.Contains(receta))
        {
            GameController.Instance.currentSD.worldData.activatedEvents.Add(receta);
        }

        if (!AllDelivered()) //si no hemos entregado aun las tres
        {
            lineIndex = 9; //vamos a correcto normal
        }
        else
        {
            lineIndex = 10; //ya hemos entregado las 3
            GameController.Instance.currentSD.worldData.itemsListW.Add("Caldero");//te entregan el caldero
        }
        
        StopAllCoroutines();
        StartCoroutine(LinesCoroutine());
    }

    //CUANDO LE DAMOS A UNA OPCION INCORRECTA
    public void IncorrectOption()
    {
        isShowingOptions=false;
        HideOptions();

        lineIndex = 7; //vamos a incorrecto
        StopAllCoroutines();
        StartCoroutine(LinesCoroutine());
    }

    //CUANDO LE DAMOS A NO TENGO NI IDEA
    public void NoIdeaOption()
    {
        isShowingOptions = false;
        HideOptions();

        lineIndex = 8; //vamos a "no tienes cara de saber hacer un gzp"
        StopAllCoroutines();
        StartCoroutine(LinesCoroutine());
    }

    //método que comprueba si hemos entregado todos los ingredientes
    private bool AllDelivered()
    {
        var datos = GameController.Instance.currentSD.worldData.activatedEvents;

        return datos.Contains("TomateOK") && datos.Contains("SaltOK") && datos.Contains("OilOK");
    }
}