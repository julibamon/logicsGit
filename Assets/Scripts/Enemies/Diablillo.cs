using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Diablillo : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public float speed = 0.5f;
    public float wallAware = 0.5f;
    public LayerMask groundLayer; //qué tipo de elemento (capa) se considera muro (ground, el suelo)


    private Rigidbody2D rigidbody2;
    private Animator animator;
    private Vector2 movement;
    private Collider2D enemyCollider;


    private bool facingRight;


    //variables para la persecución enemigo->player
    public Transform player; //para asociar que hay que perseguir al elemento player
    PlayerController playerController; //para acceder a las variables del player (identificado en start, llamado en damage)
    public float detectionRange; //distancia a la que detecta el enemigo al player
    public float loseRange; //distancia a la que el enemigo deja de ver al player
    public float followSpeed; //velocidad al perseguir
    private bool isFollowing=false;

    //cuánto daño hace este enemigo
    public int cantDamage;
    //vida
    public int maxHealth;
    public int currentHealth;

    //invulnerabilidad
    public float invulnerableTime = 0.3f; //menos que el player
    private bool isInvulnerable =false;

    //impulso hacia atras al recibir daño del jugador
    public float pushBack = 20f; //fuerza de impulso hacia atras
    private bool isKnocked = false; //está knockeado?
    private bool isDead=false; //está muerto? (var usada para la animacion de mueriendo)


    void Awake()
    {
        rigidbody2 = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyCollider = GetComponent<Collider2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        playerController = player.GetComponent<PlayerController>(); //identificar al player (para hacerlo una sola vez), linea implementada para poder coger atributos del player (cantidad de daño que hace la espada)

        if (transform.localScale.x < 0f){
            facingRight=false;
        } else if(transform.localScale.x > 0f){
            facingRight=true;
        }
    //inicializar la vida
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead == false){
        //AQUI LA LÓGICA SERÍA, si está persiguiendo el enemigo al player, que persiga aún chocándose con la pared
        //si no está persiguiendo, que haga flip cada vez que el raycast detecte la pared

    //si está persiguiendo
    float distance2Player = Vector2.Distance(transform.position, player.position); //distancia a la que está el player y enemy
    if(distance2Player <= detectionRange) //si está dentro del área de detección
        {
            isFollowing = true;
        } else
        {
            isFollowing = false;
        }

    //si no está persiguiendo (idle)
        if (!isFollowing)
        {
            Vector2 direction = Vector2.right;

        if(facingRight == false){
            direction=Vector2.left;        
            }
       
            if(Physics2D.Raycast(transform.position, direction, wallAware, groundLayer)){
                Flip();
            }
        }
        }
        
        
    }
        void FixedUpdate() //donde se mueve cualquier elemento del juego realmente
    {
        if (!isKnocked && !isDead) //para dejar de mover el enemigo si está knockeado (o muerto obviamente)
        {
            
        float horizontalVelocity;

        if (isFollowing == true)
        {
            float direction2Player = Mathf.Sign(player.position.x-transform.position.x);
            //Mathf.Sign devuelve: 1-> si el player está a la dcha
                                // -1-> si el player está a la izqda
                                // 0-> si están en mismo punto
            horizontalVelocity= direction2Player*followSpeed;
            if(direction2Player>0 && !facingRight) //no debería ocurrir (está mirando al lado equivocado)
            {
                Flip();
            } else if(direction2Player <0 && facingRight) //también está mirando al lado equivocado, así que hacemos flip
            {
                Flip();
            }

    }else{
            //si no está following
        horizontalVelocity = speed;
        if(facingRight==false){ //para que cambie el sentido de la velocidad
            //movimiento de desplazamiento del personaje
           horizontalVelocity = speed * -1f;
           
        }
        
    }
     rigidbody2.velocity= new Vector2(horizontalVelocity, rigidbody2.velocity.y); //velocity.y porque sino siempre va a flotar si lo ponemos a 0

        }
    }
    
        private void Flip(){
        facingRight = !facingRight;
        float localScaleX= transform.localScale.x;
        localScaleX = localScaleX * -1f; //para inventir el valor se multiplica por -1 (aqui es donde literalmente le damos la vuelta al pj)
        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z); //aqui lo aplicamos
    }

    //RECIBIR DAÑO
       public void Damaged(int cant, Vector2 attackDirection) //RECIBIR DAÑO
    {
        if (!isInvulnerable && !isDead) //podemos recibir daño
        {
            currentHealth = currentHealth - cant; //según el daño que nos quita el player

            if (currentHealth <= 0) //para no quedarnos con valores negativos de vida
            {
                currentHealth = 0;
                isDead=true;
                rigidbody2.velocity=Vector2.zero; // que deje de moverse el enemigo
                rigidbody2.bodyType = RigidbodyType2D.Kinematic; //que el collider no interfiera el paso cuando el enemigo está muerto
                GetComponent<Collider2D>().isTrigger=true; //convertir el collider en trigger
                isFollowing=false; //no seguir al player
                isKnocked=false; //no hacer daño al player
                

                animator.SetTrigger("Die"); //LANZAR EL TRIGGER DE LA ANIMACIÓN
            } else{
            isKnocked = true; //para que deje de moverse en update, se quede quieto y entonces se eche para atrás tras recibir el golpe
             rigidbody2.velocity = new Vector2(attackDirection.x * pushBack, rigidbody2.velocity.y); //el enemigo se echa para atrás pero sin flipear aunque se mueva en dir opuesta
            
             
            StartCoroutine(coroutinePushBack());
            StartCoroutine(coroutineInvulnerable()); //empieza tiempo de invulnerabilidad
        }
        }
    }

    //SER INVULNERABLE
    private IEnumerator coroutineInvulnerable()
    {
        isInvulnerable = true; //ponemos a true la invulnerabilidad en este punto ( ya que acabamos de recibir daño)
        
        yield return new WaitForSeconds(invulnerableTime); //esperamos un tiempo hasta dejar de ser invulnerables

        isInvulnerable = false; //dejamos de ser invulnerables
    }

     //ENEMIGO QUIETO PARA RECIBIR EMPUJÓN
    private IEnumerator coroutinePushBack()
    {
        spriteRenderer.color = new Color(0.4279091f, 0.3737985f, 0.4528302f, 1f); //para que parpadee hacia este color al recibir daño (mientras es empujado)

        yield return new WaitForSeconds(0.2f); //esperamos un tiempo quietos para echarnos para atrás tras recibir empujón

        spriteRenderer.color = Color.white;
        isKnocked = false; //dejamos de estar paralizados
    }


    //HACER DAÑO AL PLAYER AL COLISIONARLE
    private void OnCollisionStay2D(Collision2D coll) //usamos Stay y no Enter para no ser invulnerables para siempre si nos quedamos tocando al enemigo
    {
        if (coll.gameObject.CompareTag("Player") && !isDead) //establecemos que solo ocurra si estamos ante el player (tag player) y estamos vivos
        {
            PlayerController pj = coll.gameObject.GetComponent<PlayerController>();
            if(pj != null)
            {
                pj.Damaged(cantDamage); //llamamos al metodo damaged del player y le pasamos la cantidad de daño que hace este enemigo
            }
        }
    }

    //RECIBIR DAÑO DE LA ESPADA DEL PERSONAJE
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Attack")) //para referirnos a la espada ya que es lo unico taggeado como Attack
        {
            float playerDirection = Mathf.Sign(transform.position.x - player.position.x);
            Vector2 attackDirection = new Vector2(playerDirection, 0f); //para saber la direccion a la que está el player (distancia hacia enemigo indica si esta a su izq o a su derecha)
            Damaged(playerController.playerDamage, attackDirection);

        }
    }

    //MORIR (DESACTIVARSE)
    public void DieAndDisable()
{
    gameObject.SetActive(false);
}

}
