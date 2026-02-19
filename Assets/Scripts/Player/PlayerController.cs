using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //para que entre en la animacion longidle
    public float longIdleTime = 5f;
    //velocidad player
    public float speed = 2.5f;
    //velocidad salto
    public float jumpForce= 6f;
    //doble salto
    [SerializeField] private int extraJumps = 1;
    private int jumpsLeft;

    private Rigidbody2D rigidbody2;
    private Animator animator;
    //longidle
    private float longIdleTimer;
    private Vector2 movement;
    //para que se de la vuelta
    private bool facingRight=true;

    //para saber si toca suelo el floorpoint (interesante para hacer el salto únicamente cuando esté en superficie)
    public Transform groundCheck;
    public LayerMask groundLayer; //con esto checkeamos qué layer de la lista de layers es el suelo
    public float groundCheckRadius; //para ver cómo es de grande nuestro groundcheck
    private bool isGrounded;

    //attacking
    private bool isAttacking;
    public GameObject swordHitBox; //hitbox de la espada al atacar
    public int playerDamage; //cuánto daño hace la espada del player


    //health
    public int maxHealth;
    public int currentHealth;

    public RectTransform healthBar; //RectTransform de los corazones llenos
    public RectTransform deadBar; //RectTransform de los corazones vacíos
    public float widthPerHealth = 18f; //para que la imagen tiled de los corazones se multiplique dinámicamente

    //tiempo de invulnerabilidad tras recibir daño
    private float invulnerableTime = 1f;
    private bool isInvulnerable =false;

    //Estoy recibiendo daño?
    private bool isHurted;


    void Awake()
    {
        rigidbody2 = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        jumpsLeft = extraJumps;

        //health
        UpdateHealthUI();
    }

    // Update is called once per frame
    void Update() //aqui metemos qué teclas interactuan
    {
        //MOVEMENT (solo cuando no está atacando)
        if(isAttacking==false){

                float horizontalInput = Input.GetAxisRaw("Horizontal"); //para que no haya retraso en el movimiento se usa getAxisRaw y no getAxis (en joystick creo q no funciona)
            movement = new Vector2(horizontalInput, 0f);

        //FLIP (para cambiar el sentido del personaje visualmente al darle a la izquierda)
            if(horizontalInput<0f && facingRight == true){ //el personaje está intentando ir a la izquierda pero estamos mirando a la derecha?
                Flip();
            } else if(horizontalInput>0f && facingRight == false){ //estamos intentando ir a la derecha pero miramos a la izq?
                Flip();
            }
        }
        

        //is grounded? con physics2d mandamos rayos 2d (los raycast)
        //el metodo overclapcircle nos permite pintar ciertas bolas en el punto que le digamos con el radio que le digamos
        // y le pedimos que checkee con una layer (en este caso groundLayer)
        //devuelve true si se encuentra más de una layer ( o una entiendo ) con la que esté interactuando
        isGrounded = Physics2D.OverlapCircle(groundCheck.position,groundCheckRadius,groundLayer);

        //DOBLE SALTO, RESETEO AL TOCAR EL SUELO
        if (isGrounded)
        {
            jumpsLeft = extraJumps;
        }

        //isJumping?
         // SALTO Y DOBLE SALTO
        if(Input.GetButtonDown("Jump") && !isAttacking)
        {
            if(isGrounded)
            {
                rigidbody2.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
            else if(jumpsLeft > 0)
            {
                // reset vertical para consistencia
                rigidbody2.velocity = new Vector2(rigidbody2.velocity.x, 0f);
                rigidbody2.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                jumpsLeft--;

                // activar animación de doble salto
                animator.SetTrigger("DoubleJump");
            }
        }

        //wanna attack?
        if (Input.GetButtonDown("Fire1") && isGrounded && !isAttacking )
        {
            movement = Vector2.zero;
            rigidbody2.velocity = Vector2.zero;
            animator.SetTrigger("Attack");
            
            
         }
        //Lógica activación trigger recibir daño y salir de isHurted
        if (isHurted)
        {
            movement = Vector2.zero;
            rigidbody2.velocity = Vector2.zero;
            animator.SetTrigger("Hit");
            isHurted=false;
         }

    }
    void FixedUpdate() //donde se mueve cualquier elemento del juego realmente
    {
        if(isAttacking==false){
            //movimiento de desplazamiento del personaje
            float horizontalVelocity = movement.normalized.x * speed;
            rigidbody2.velocity= new Vector2(horizontalVelocity, rigidbody2.velocity.y); //velocity.y porque sino siempre va a flotar si lo ponemos a 0
        }
        
    }
    
    void LateUpdate() //antes de pintar en pantalla, codigo relacionado con animaciones
    {
        //siempre y cuando el jugador no esté moviendo al player, que vuelva a estado idle
        animator.SetBool("Idle", movement == Vector2.zero);
        //que se actualice el bool isgrounded
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("VerticalVelocity", rigidbody2.velocity.y); //actualizar velocidad en eje y (estamos en el aire)

        //para el attacking
        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack")){
            isAttacking = true;
        }else{
            isAttacking = false;
        }
        //long idle
       if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Idle")) {
			longIdleTimer += Time.deltaTime;

			if (longIdleTimer >= longIdleTime) {
				animator.SetTrigger("LongIdle");
			}
		} else {
			longIdleTimer = 0f;
		}
    }
    private void Flip(){
        facingRight = !facingRight;
        float localScaleX= transform.localScale.x;
        localScaleX = localScaleX * -1f; //para inventir el valor se multiplica por -1 (aqui es donde literalmente le damos la vuelta al pj)
        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z); //aqui lo aplicamos
    }

    //para que salga el numero de corazones segun la vida
    void UpdateHealthUI()
    {
        float healthWidth = currentHealth * widthPerHealth;

        healthBar.sizeDelta = new Vector2(healthWidth, healthBar.sizeDelta.y);

        float deadWidth = (maxHealth-currentHealth)*widthPerHealth;
        deadBar.sizeDelta = new Vector2(deadWidth, deadBar.sizeDelta.y);
    }

    public void Damaged(int cant)
    {
        if (!isInvulnerable) //podemos recibir daño
        {
            currentHealth = currentHealth - cant; //según el daño que nos quita el enemigo/entorno

            if (currentHealth < 0) //para no quedarnos con valores negativos de vida
            {
                currentHealth = 0;
            }
            isHurted=true;
            UpdateHealthUI(); //que se actualicen los corazones :)
            StartCoroutine(coroutineInvulnerable()); //empieza tiempo de invulnerabilidad
        }
    }

    private IEnumerator coroutineInvulnerable()
    {
        isInvulnerable = true; //ponemos a true la invulnerabilidad en este punto ( ya que acabamos de recibir daño)
        yield return new WaitForSeconds(invulnerableTime); //esperamos un tiempo hasta dejar de ser invulnerables
        isInvulnerable = false; //dejamos de ser invulnerables
    }
    

    //activar la hitbox de la espada al empezar el ataque (llamado por animation event)
    public void SwordHitBoxON()
    {
        swordHitBox.SetActive(true);
    }

    //desactivar la hitbox de la espada al terminar el ataque (llamado por animation event)
    public void SwordHitBoxOFF()
    {
        swordHitBox.SetActive(false);
    }
}

