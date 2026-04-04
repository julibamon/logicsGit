using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatroll : MonoBehaviour
{

    public float speed = 1f;
    public float minX;
    public float maxX;
    public float waitingTime = 2f;

    private GameObject target;
    //REFERENCIA A ANIMATOR EN UNITY
    private Animator animator;
    private Weapon weapon;


//awake-> para conseguir referencias
    void Awake(){
        animator= GetComponent<Animator>();
        //lo cogemos desde el hijo ya que enemy no tiene directamente el weapon sino que es un anexo a él
        weapon = GetComponentInChildren<Weapon>();
    }
    // Start is called before the first frame update
    void Start()
    {
       updateTarget();
       StartCoroutine("PatrolToTarget");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void updateTarget(){
        //primero crea target a la izquierda
        if(target == null){
            target = new GameObject("Target"); //objeto invisible "target" usado como limite de movimiento en x
            target.transform.position = new Vector2(minX, transform.position.y);
            transform.localScale = new Vector3(-1,1,1);
            return; //para que si es la primera vez no ejecute el resto
        }
        //si estamos en la izq, creamos target a la derecha
        if(target.transform.position.x==minX){
            target.transform.position = new Vector2(maxX, transform.position.y);
            transform.localScale = new Vector3(1,1,1);
        }
        //si estamos a la derecha, cambia target a la izq
        else if(target.transform.position.x==maxX){
            target.transform.position = new Vector2(minX, transform.position.y);
            transform.localScale = new Vector3(-1,1,1);
        }
    }
    //corrutina (se marca con IEnumerator)->funcion que nos permite ejecutar pasos incluyendo tiempos de espera intercalando
    //las acciones, por ejemplo este metodo cuando lo llamemos lo primero que vamos a hacer va a ser preguntar:
    //mientras la distancia sea menor que 0,05f (mientras no esté pegado), vamos a movernos
    private IEnumerator PatrolToTarget(){
        while(Vector2.Distance(transform.position, target.transform.position)> 0.05f){

            //ACTUALIZA EL ANIMATOR
            animator.SetBool("Idle", false);

            Vector2 direction = target.transform.position - transform.position;
            float xDirection = direction.x;
            
            transform.Translate(direction.normalized*speed*Time.deltaTime);

        //IMPORTANTE
            yield return null; //no sigas ejecutando codigo, y vuelve a ejecutar el metodo
        }

        Debug.Log("Llegamos al target");

        transform.position = new Vector2(target.transform.position.x,transform.position.y); //reajusta los 0.05
        updateTarget();
        //ACTUALIZAMOS ANIMATOR CUANDO ESTOY PARADO (para poner la animacion de parado)
        animator.SetBool("Idle", true);

        //DISPARAMOS AL DARNOS LA VUELTA (cambiamos la animacion de parado->disparando)
         animator.SetTrigger("New Trigger");
        

        //vamos a esperar un momentito parados antes de volver a llamar a la corrutina
        Debug.Log("Esperando a waitingTime: " + waitingTime + "segundos");
        yield return new WaitForSeconds(waitingTime);

        //cuando hemos esperado 2 segundos, updateamos el target al otro lado, así permitimos que el enemigo se siga moviendo
        //y ocurra lo mismo pero en el otro lado
        Debug.Log("Hemos esperado suficiente, updateamos el target");
        StartCoroutine("PatrolToTarget"); //llamamos a la corrutina de nuevo, basicamente
        
    }
    //FUNCION PARA LLAMAR DESDE EL EVENTO DE LA ANIMACION (pa que dispare desde el primer frame de shooting)
    void CanShoot(){
        if(weapon){
            weapon.Shoot();
        
        }
    }
}