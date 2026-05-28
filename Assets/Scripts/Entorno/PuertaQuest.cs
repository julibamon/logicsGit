using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuertaQuest : MonoBehaviour
{

    public GameObject rangoColliderInvisible;
    public GameObject puerta;
    private bool isPlayerHere = false;

    public GameObject canvas; //texto encima de la puerta

    private PlayerController playerController;




    // Start is called before the first frame update

    void Start()
    {

        CheckState();
    }

    void Update()
    {
        if(isPlayerHere && Input.GetKeyDown(KeyCode.E))
        {
            OpenDoor();
        }
    }

    public void CheckState()
    {
        if (GameController.Instance.currentSD.worldData.activatedEvents.Contains("MariOpened"))
        {
            rangoColliderInvisible.SetActive(false);
            puerta.SetActive(false);
        }
    }

    public void OpenDoor()
    {
        if (GameController.Instance.currentSD.worldData.itemsListW.Contains("MariKey"))
        {
            Play("Object");
            GameController.Instance.currentSD.worldData.activatedEvents.Add("MariOpened");
            rangoColliderInvisible.SetActive(false);
            puerta.SetActive(false);
        }
        else
        {
            MessageMenu.Instance.ShowMessage("Vaya...parece que el candado está cerrado");
        }


    }


        //lógica de entrar en rango de la puerta

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

    public void Play(string soundName) { 
        SoundEffectManager.Instance.Play(soundName, false);
     } 

}
