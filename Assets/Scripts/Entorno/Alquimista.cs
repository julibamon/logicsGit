using UnityEngine;
using TMPro;
using System.Collections;
using UnityEditor.Search;
using Unity.Collections;
using Unity.VisualScripting;

public class Alquimista : MonoBehaviour
{
    private bool isPlayerHere = false;

    private Animator animator;

    public GameObject upCanvas; //texto encima del npc
    public GameObject dialogueCanvas;

    private PlayerController playerController;

    //puerta que se abre
    public GameObject puerta;

    [SerializeField, TextArea(4,7)] private string[] dialogueArea; //lineas de dialogo del npc
    //TextArea(minimo,maximo) numero de lineas en el cuadro de texto

    [SerializeField] private TMP_Text dialogueText;

    private bool isTalking; //hemos empezado el dialogo
    private int lineIndex ; //indica qué linea de dialogo se está mostrando

    public float typingTime = 0.05f;

    private bool isShowingOptions=false; //para cuando estamos mostrando "opciones"(botones) y no texto del inspector

    private bool playerIsGrounded;
    private bool lineFinished; //cuadro de dialogo mostrado entero ya

    private bool isInside;

    //floorPoints para reposicionar al player cuando esté en dialogo con las viejitas
    public GameObject floorPointOUT;
    public GameObject floorPointIN;

    //COLLIDERS
    public BoxCollider2D colliderOUT;
    public BoxCollider2D colliderIN;

    //botones de las opciones
    public GameObject giveCalderoButton;
    public GameObject noIdeaButton;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void Start()
    {
        isInside = GameController.Instance.currentSD.worldData.activatedEvents.Contains("AlquimistaKEY");
        animator.SetBool("isInside", isInside);

    if (isInside)
{   
        upCanvas.transform.localPosition = new Vector3(0.1f,-0.045f,0f);
    }

        UpdateColliders();
    }

    // Update is called once per frame
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
        //CONTROL ANIMACION HABLAR
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

        if(!isInside) //si estoy fuera
        {
            selectedPoint = floorPointOUT.transform;
        }
        else    //si estoy dentro de la casa
        {
            selectedPoint = floorPointIN.transform;
        }

        //movemos al player
        while(Vector2.Distance(playerController.transform.position, selectedPoint.position)> 0.05f)
        {
            playerController.transform.position = Vector2.MoveTowards(playerController.transform.position, selectedPoint.position, 1.5f*Time.deltaTime);
        
            yield return null;        
        }

        playerController.LookAtAlquimista(); //hacemos flip para mirar a la derecha si hace falta y activamos la animacion Idle del player

        StartDialogue();
    }

//empezar dialogo
    public void StartDialogue()
    {
        dialogueCanvas.SetActive(true); //abrimos el canvas de dialogo
        upCanvas.SetActive(false);      //quitamos el mensaje encima del NPC "Hablar [E]"
        if (GameController.Instance.currentSD.worldData.activatedEvents.Contains("AlquimistaCALDERO"))
        {
            lineIndex=4;
        }

        else if (GameController.Instance.currentSD.worldData.itemsListW.Contains("RecetaSal")) //si ya me ha dado la receta
        {
            lineIndex=3; //empezamos el dialogo en index 3
        }
        else if(isInside)
        {
            lineIndex=2; //está dentro y te da la receta
        } else if (GameController.Instance.currentSD.worldData.activatedEvents.Contains("AlquimistaKEY"))
        {
            lineIndex=1; //te da las gracias por haberle dado la llave
        }
        else
        {
            lineIndex = 0; //dialogo inicial inicial (esta fuera y no hemos encontrado la llave)
        }
        StartCoroutine(LinesCoroutine()); //llamamos a la corrutina

    }

private void NextDialogue()
    {

        //NO TENEMOS NADA QUE DARLE,no tenemos la llave, o porque ya se ha completado la quest
        if ((lineIndex == 0 && !GameController.Instance.currentSD.worldData.itemsListW.Contains("HouseKEY"))  || lineIndex == 4)
        {
            EndDialogue();
            return;
        }
        if(lineIndex == 0 && GameController.Instance.currentSD.worldData.itemsListW.Contains("HouseKEY")) //momento en el que le damos la llave
        {
            lineIndex++;
            StartCoroutine(LinesCoroutine()); //hacemos esto para que pueda salir el siguiente dialogo
            return;
        }
        if (lineIndex == 1) //le acabamos de dar la llave
        {
            EndDialogue();//se cierra el dialogo
            upCanvas.SetActive(false);
            animator.SetBool("isWalking", true);//se activa el booleano iswalking
            animator.SetBool("isInside", true);//isInside se pone a true
            colliderOUT.enabled=false;
            StartCoroutine(goingHomeCoroutine()); //corrutina y se activa el segundo collider
            return;
        }
        if(lineIndex == 2) //cuando entra y le hablamos ("por las molestias")
        {
            GameController.Instance.currentSD.worldData.itemsListW.Add("RecetaSal"); // me da la receta
            lineIndex++; //va al dialogo 3 ("tambien he perdido mi caldero")
            return;
            
        }
        

        StartCoroutine(LinesCoroutine());

        
    }

    public IEnumerator goingHomeCoroutine()
    {
        yield return new WaitForSeconds(5f);
        isInside = true;
        UpdateColliders();
        upCanvas.transform.localPosition = new Vector3(0.1f,-0.045f,0f); //movemos el canvas
        GameController.Instance.currentSD.worldData.activatedEvents.Add("AlquimistaKEY"); //activamos el evento  el alquimista ha recibido las llaves
        animator.SetBool("isWalking", false);//se desactiva el booleano iswalking     
    }

    void UpdateColliders()
    {
        colliderOUT.enabled=!isInside;
        colliderIN.enabled=isInside;
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

    if (isShowingOptions) {
        return;
    }
    if (!lineFinished)
    {
        StopAllCoroutines();
        dialogueText.text = dialogueArea[lineIndex];
        lineFinished = true;

        if (lineIndex == 3 && !isShowingOptions && !GameController.Instance.currentSD.worldData.activatedEvents.Contains("AlquimistaCALDERO"))
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
        if (lineIndex == 3 && !isShowingOptions && !GameController.Instance.currentSD.worldData.activatedEvents.Contains("AlquimistaCALDERO")) //que salgan las opciones abajo pero despues de que salga toda la pregunta
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

    //para entregar o no el caldero
     private void ShowOptions()
    {
    isShowingOptions=true;
       
        if (GameController.Instance.currentSD.worldData.itemsListW.Contains("Caldero")) //si tengo el caldero
        {
            giveCalderoButton.SetActive(true);
        }
       
    //SKIP
    noIdeaButton.SetActive(!GameController.Instance.currentSD.worldData.activatedEvents.Contains("AlquimistaCALDERO")); //depende de si se han entregado los 3 o no
    }

    private void HideOptions()
    {
        giveCalderoButton.SetActive(false);
        noIdeaButton.SetActive(false);
    }
    //CUANDO LE DAMOS EL CALDERO
    public void CorrectOption()
    {
        isShowingOptions=false;
        HideOptions();
        if (!GameController.Instance.currentSD.worldData.activatedEvents.Contains("AlquimistaCALDERO"))
        {
            GameController.Instance.currentSD.worldData.activatedEvents.Add("AlquimistaCALDERO");
        }

        lineIndex = 4; //ya hemos entregado las 3
        float posYPuerta = puerta.transform.position.y;
        puerta.transform.position=new Vector3(puerta.transform.position.x,17.65f,puerta.transform.position.z);
        
        
        StopAllCoroutines();
        StartCoroutine(LinesCoroutine());
    }

    //cuando le damos a "No"
    public void NoIdeaOption()
    {
        isShowingOptions = false;
        HideOptions();
        StopAllCoroutines();
        EndDialogue();
    }

}
