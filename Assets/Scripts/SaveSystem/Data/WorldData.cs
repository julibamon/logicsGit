using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class WorldData
{
   public List<String> defeatedBosses = new List<String>(); //Lista bosses derrotados
   public List<String> itemsListW = new List<String>(); //objetos recogidos del mundo

    public List<String> activatedEvents = new List<String>();
}
