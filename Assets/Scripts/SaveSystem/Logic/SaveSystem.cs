using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;


public static class SaveSystem //la clase será estatica
{
    private static String GetPath(int slotId)
    {
        return Path.Combine(Application.persistentDataPath, $"slot{slotId}.json"); //Path viene de C#, me permite trabajar con rutas de archivos (importado en System.IO)
                                                                                    //.Combine me permite juntar esas dos partes en una misma ruta, no usamos "+" porque depende del S.O se usa / o \
                                                                                    //Application.persistentDataPath lo usa unity para decir dónde se estan guardando los archivos del juego
    }

    public static Boolean SlotExists(int slotId)
    {
        return File.Exists(GetPath(slotId)); //File se usa en C sharp para referirse a archivos
    }

    //GUARDAR LOS DATOS DEL JUEGO EN LA RUTA
    public static void SaveGame(SaveData data, int slotId)
    {
        String path = GetPath(slotId);
        data.slotIndex = slotId;
        Debug.Log("DEBUGGGGGGGG->Guardando JSON en: " + path);

        
        String json = JsonUtility.ToJson(data, true); //true es para ponerlo legible, pretty print

        File.WriteAllText(path, json);

        Debug.Log($"Guardado en la ruta {path} en el slot {slotId}");

        }

    //CARGAR LOS DATOS DEL JUEGO DE UN SLOT 
    public static SaveData LoadGame(int slotId)
    {
        String path = GetPath(slotId);

        if (!File.Exists(path)) //si no se encuentra la ruta
        {
            Debug.LogWarning($"No hay datos en el slot {slotId}");
            return null;
        }
        else
        {
            String json = File.ReadAllText(path); //si sí existe la ruta, es decir hay archivos guardados en ese slot, lee los datos y los pone en un String
            SaveData data = JsonUtility.FromJson<SaveData>(json); //transforma de json-> a save data

            Debug.Log($"Cargados los datos del slot {slotId}");

            return data;
        }

    }
    public static void DeleteSlotData(int slotId)
    {
        String path = GetPath(slotId);

        if (File.Exists(path))
        {
            File.Delete(path);

            Debug.Log($"Borrados los datos del slot {slotId}");

        }

    }
}
