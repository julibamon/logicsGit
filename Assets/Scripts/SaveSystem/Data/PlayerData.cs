using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerData
{
    public int maxHealth;
    public int currentHealth;

    //posicion para checkpoint
    public String currentNameScene;
    public float checkpointX; //posX        //separamos en floats en vez de Vector2 porque unity lo serializa mejor en JSON
    public float checkpointY; //posY


    //listas de habilidades y objetos del player
    public List<String> skillsList = new List<String>(); //aunque solo vaya a tener 1 habilidad conseguible, el doble salto, quiero que el proyecto sea muy escalable
    public List<String> itemsList = new List<String>();

}
