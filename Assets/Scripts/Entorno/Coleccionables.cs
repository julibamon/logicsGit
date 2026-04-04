using UnityEngine;

public class Coleccionables : MonoBehaviour

{
    private bool isPlayerHere = false;

    public GameObject canvas; //texto encima del comestible

    private PlayerController playerController;

    public GameObject coleccionable;

    public string nombreObjeto;

    void Start()
    {
        if (GameController.Instance.currentSD.worldData.itemsListW.Contains(nombreObjeto)) //desactivamos el coleccionable si ya lo tenemos
        {
            coleccionable.SetActive(false);

        }
    }

    void Update()
    {
        if(isPlayerHere && Input.GetKeyDown(KeyCode.E))
        {
            GetColeccionable();
        }
    }

    public void GetColeccionable()
    {

        coleccionable.SetActive(false);
        Debug.Log("Cogido el coleccionable"+ nombreObjeto);
        GameController.Instance.currentSD.worldData.itemsListW.Add(nombreObjeto); // añadimos el coleccionable a la lista de objetos
        if(nombreObjeto == "RecetaAceite")
        {
            MessageMenu.Instance.ShowMessage("...Ah... conque esa es la cantidad de aceite que habría que echarle a un buen gazpacho...");

        } else if(nombreObjeto == "RecetaTomate")
        {
            MessageMenu.Instance.ShowMessage("...Ah... conque esa es la cantidad de tomate que habría que echarle a un buen gazpacho...");

        }  else if(nombreObjeto == "HouseKEY")
        {
            MessageMenu.Instance.ShowMessage("...Vaya...¿Y estas llaves?...Alguien las habrá perdido");

        }



    }

    //lógica de entrar en rango del objeto

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