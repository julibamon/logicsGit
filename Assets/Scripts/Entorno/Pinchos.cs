using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pinchos : MonoBehaviour
{

public int cantDamage;
       //HACER DAÑO AL PLAYER AL COLISIONARLE
    private void OnCollisionStay2D(Collision2D coll) //usamos Stay y no Enter para no ser invulnerables para siempre si nos quedamos tocando al enemigo
    {
        if (coll.gameObject.CompareTag("Player")) //establecemos que solo ocurra si estamos ante el player (tag player) y estamos vivos
        {
            PlayerController pj = coll.gameObject.GetComponent<PlayerController>();
            if(pj != null)
            {
                pj.Damaged(cantDamage); //llamamos al metodo damaged del player y le pasamos la cantidad de daño que hace este enemigo
            }
        }
    }
}
