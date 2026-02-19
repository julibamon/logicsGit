using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject bulletPrefab; //asignamos desde la escena que nos referimos a bullet (se crea automaticamente el atributo en el inspector y arrastramos el prefab)

    //cogemos el punto de fuego que es desde donde se va a instanciar
    private Transform firePoint; //transform es una cosa que tienen los objetos referente a su posicion

    public GameObject shooter; //quién posee el arma

    //lo que se instanciará una vez la bala choque contra algo (enemigo, muro, etc)
    public GameObject explosionEffect;

    //imita una línea que cruza la pantalla imitando un disparo
    public LineRenderer lineRenderer;
    void Awake(){ //sitio para coger otro tipo de componentes o objetos dentro de nuestro objeto
        firePoint = transform.Find("PuntoFuego");
    }

    // Start is called before the first frame update
    void Start()
    {
         //metodo que permite ejecutar las funciones con un espacio de tiempo determinado entre ellas. (nombre funcion en string, tiempo)
       //Invoke("Shoot", 1f);
       //Invoke("Shoot", 2f);
       //Invoke("Shoot", 3f);
    }

    // Update is called once per frame
    void Update()
    {
       //Lerp-> metodo para linear interpolation
       
    }
    public void Shoot(){ //METODO PARA MOVER LA BALA
    if(bulletPrefab != null && firePoint != null && shooter != null){//siempre antes de instanciar un prefab

     //INSTANCIAR LA BALA(prefab) EN EL ARMA (objeto a instanciar, posicion cuando se instancie, rotacion cuando se instancie)
        GameObject myBullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity) as GameObject;

        //esto es como un import pa usar codigo de otro script
        BulletScript bulletComponent = myBullet.GetComponent<BulletScript>();

        //IF PARA DETERMINAR HACIA DONDE SE DISPARA DEPENDIENDO DE HACIA DONDE ESTÉ MIRANDO EL SHOOTER
        if(shooter.transform.localScale.x < 0f){ 
            //left
            //direction es un atributo de BulletScript (de tipo Vector2)
                bulletComponent.direction=Vector2.left; //new Vector2(-1f, 0f)
        }else{
            //right
            bulletComponent.direction=Vector2.right; //new Vector2(1f, 0f)

        }

    }
    }

    public IEnumerator ShootWithRaycast(){
        if(explosionEffect != null && lineRenderer != null){

            //para que salga un rayo, primera variable es el punto desde el que va a salir y la segunda la direccion 
            // (right es eje x, no confundir con sentido)
            RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, firePoint.right);
           
           
            if(hitInfo){
                //example code
                //if (hitInfo.collider.tag == "Player"){
                    //Transform player = hitInfo.transform;
                    //player.GetComponent<PlayerHealth>().ApplyDamage(5);
                //  }
                
                //Instantiate explosion on hit point
                //hintInfo.point devuelve el punto donde ha colisionado el rayo, 
                Instantiate(explosionEffect, hitInfo.point, Quaternion.identity);

                //para marcar la posicion del line renderer (en este ejemplo las inicializamos aqui por ello hemos marcado en el linerenderer en unity
                // la opcion use world space) posicion 0-> inicio, posicion 1-> final de la linea
                lineRenderer.SetPosition(0, firePoint.position);
                lineRenderer.SetPosition(1, hitInfo.point); //punto donde toca con collider

                
            } else {
                lineRenderer.SetPosition(0, firePoint.position);
                lineRenderer.SetPosition(1, hitInfo.point + Vector2.right *100); //si no ha tocado ningun collider, que se estire y ya
            }
            lineRenderer.enabled=true;

            yield return null;

            lineRenderer.enabled = false;
        }
    }
}
