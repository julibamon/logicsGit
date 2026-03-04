using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance; //para acceder desde cualquier script al game controller (único)

    public SaveData currentSD; //datos actuales, la partida "en curso"
    public int currentSlotId = -1; //se inicializa a -1 porque no hay ninguno seleccionado todavía


    private void Awake()
    {
        if (instance == null) //si no existe un gamecontroller
        {
           instance=this; //que sea este el gamecontrollercontin
           DontDestroyOnLoad(gameObject); //que no se destruya el gamecontroller al cambiar de escena

        }
        else
        {
            Destroy(gameObject); //si ya existe un gamecontroller, destruye este duplicado
        }
    }

    //crear nueva partida
    public void NewGame(int slotId)
    {
        currentSlotId = slotId;

        //datos iniciales personaje
        PlayerData playerData = new PlayerData();
        playerData.maxHealth=5;
        playerData.currentHealth=5;
        playerData.checkpointX=13.433f;
        playerData.checkpointY=-1.444f;
        playerData.currentNameScene=SceneManager.GetActiveScene().name; //escena inicial = escena activa

        //datos inciales del mundo
        WorldData worldData = new WorldData();

        //datos de guardado
        currentSD = new SaveData(); //inicializamos la variable creada al principio del script
        currentSD.playerData = playerData; //Asignamos al save data los valores para player y world que acabamos de crear (es decir, ''relacionamos todo'')
        currentSD.worldData = worldData; 

        //guardamos la nueva partida en el slot usando el metodo que creamos en savesystem
        SaveSystem.SaveGame(currentSD, currentSlotId);
        Debug.Log($"acabas de crear una nueva partida en el slot {slotId}");
    
    }


    //guardar partida (no para inicializar, sino es una partida que ya tiene datos)
    public void SaveGame()
    {
        if (currentSlotId < 0)
        {
            Debug.LogWarning("Ningún slot seleccionado donde guardar la partida");
        }
        else
        {
            SaveSystem.SaveGame(currentSD, currentSlotId);
        }
    }

    //cargar partida
    public void LoadGame(int slotId)
    {
        if (!SaveSystem.SlotExists(slotId))
        {
            Debug.LogWarning($"El slot {slotId} no existe");
        }
        else
        {
            currentSlotId = slotId; //hago esto porque este es el punto donde asigno que el currentslot es el de la partida que se está jugando, porque es la que estoy cargando, y me sirve para saber cual es el currentslot al guardar
            currentSD = SaveSystem.LoadGame(slotId);

            Debug.Log($"Cargada la partida del slot {slotId}");

        }
    }


}
