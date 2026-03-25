using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance; //para acceder desde cualquier script al game controller (único)

    public SaveData currentSD; //datos actuales, la partida "en curso"
    public int currentSlotId = -1; //se inicializa a -1 porque no hay ninguno seleccionado todavía

    public PlayerData playerData;

    //para cambiar entre escenas
    public Vector2 nextSpawnPosition;
    public bool useNextSpawn=false;

    //para saber si flipear al personaje tras moverse entre escenas
    public bool flipOnSpawn = false;


    


    private void Awake()
    {
        if (Instance == null) //si no existe un gamecontroller
        {
           Instance=this; //que sea este el gamecontrollercontin
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
        playerData = new PlayerData();
        playerData.maxHealth=5;
        playerData.currentHealth=5;
        playerData.checkpointX=-14.221f;
        playerData.checkpointY=-2.241f;
        playerData.currentNameScene="SampleScene"; //escena inicial, SampleScene

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
            Debug.Log("Guardada la partida");
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

            playerData = currentSD.playerData;

            Debug.Log($"Cargada la partida del slot {slotId}");

        }
    }


    //Cargar partida si existe o crear una nueva DESDE EL MENÚ DE SELECCIÓN DE SLOTS

public void SlotSelectorPlayOrLoad(int slotId)
    {
        if (SaveSystem.SlotExists(slotId)) //EXISTE PARTIDA EN EL SLOT SELECCIONADO
        {
            LoadGame(slotId);
            Debug.Log($"Existía el slot {slotId} seleccionado desde el menú, por eso cargamos la partida");
        }
        else
        {
            NewGame(slotId); //NO EXISTE LA PARTIDA EN EL SLOT SELECCIONADO
            Debug.Log($"No existía el slot {slotId} seleccionado desde el menú, por eso creamos una partida nueva");
        }
        
        SceneManager.LoadScene(playerData.currentNameScene); //cargamos la escena correpondiente
    }
    


}
