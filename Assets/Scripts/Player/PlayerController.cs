using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


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
    public bool isGrounded;
    //COYOTE TIME
    public float coyoteTime=0.15f;
    public float jumpLockCont;
    private float coyoteTimeCont = 0f;



    public bool isInDialogue = false; //para controlar si el player está en dialogo o no

    //attacking
    private bool isAttacking;
    public GameObject swordHitBox; //hitbox de la espada al atacar
    public int playerDamage; //cuánto daño hace la espada del player


    //health y saveData
    public int maxHealth;
    public int currentHealth;

    private List<String> skillsList;

    private List<String> itemsList;

    public RectTransform healthBar; //RectTransform de los corazones llenos
    public RectTransform deadBar; //RectTransform de los corazones vacíos
    public float widthPerHealth = 18f; //para que la imagen tiled de los corazones se multiplique dinámicamente

    //tiempo de invulnerabilidad tras recibir daño
    private float invulnerableTime = 1f;
    private bool isInvulnerable =false;

    //Estoy recibiendo daño?
    private bool isHurted;

    //MENU DE MUERTE
    public GameObject DeathMenu;
    public Transform canvasMessagePlayer; //los mensajes de ("has obtenido doble salto"), etc

    //plataforma móvil
    private PlataformaMovil currentPlatform;

    
    void Awake()
    {
        rigidbody2 = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        jumpsLeft = extraJumps;

        
        if (GameController.Instance.useNextSpawn)// Si venimos de un TP, la posición la dicta nextSpawnPosition
        {
            ApplySaveDataToPlayerWhenTP();
        }
        else //si no venimos del TP cargamos los datos de guardado
        {
                    ApplySaveDataToPlayer();

        }

    }

    // Update is called once per frame
    void Update() //aqui metemos qué teclas interactuan
    {
        if (isInDialogue) //si el player está en un dialogo
        {
            movement=Vector2.zero; //que no se pueda mover el player
            rigidbody2.velocity=Vector2.zero;
            return; //que no siga ejecutandose codigo
        }
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

        //COYOTE TIME
        if (isGrounded && jumpLockCont <= 0)
        {
            coyoteTimeCont = coyoteTime; //si aun estamos en el suelo tenemos el tiempo completo disponible
        }
        else
        {
            coyoteTimeCont -= Time.deltaTime; //si hemos salido del suelo vamos descontando
            jumpLockCont -= Time.deltaTime;
        }

        //DOBLE SALTO, RESETEO AL TOCAR EL SUELO
        if (isGrounded)
        {
            jumpsLeft = extraJumps;
        }

        //isJumping?
         // SALTO Y DOBLE SALTO
        if(Input.GetButtonDown("Jump") && !isAttacking)
        {
            if(coyoteTimeCont >0f)
            {
                rigidbody2.velocity = new Vector2(rigidbody2.velocity.x, 0f);
                rigidbody2.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                AnimSoundNo("Jump"); //sonido jump
                coyoteTimeCont = 0f; //para evitar multiples altos
                jumpLockCont = coyoteTime;
            }
            else if(jumpsLeft > 0 && skillsList.Contains("DoubleJump")) //si tenemos los saltos reiniciados y hemos cogido la habilidad
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
            Vector2 finalVelocity= new Vector2(horizontalVelocity, rigidbody2.velocity.y); //velocity.y porque sino siempre va a flotar si lo ponemos a 0
 
            //plataformas moviles
            if(currentPlatform !=null && isGrounded) //si estoy encima de una plataforma movil
            {
                finalVelocity.x += currentPlatform.PlatformVelocity.x;
                //rigidbody2.sharedMaterial.friction = 5f;
            } else
            {
                //rigidbody2.sharedMaterial.friction = 0.0f;

            }
            rigidbody2.velocity = finalVelocity;
            
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
       if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Idle") && !isInDialogue) { //si venimos de idle Y NO ESTAMOS DIALOGANDO, entonces podemos hacer longidle
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

        //que no se gire el canvas de arriba del personaje (has obtenido doble salto)
        float canvasScaleX = canvasMessagePlayer.localScale.x;
        canvasScaleX = canvasScaleX * -1f;

        canvasMessagePlayer.localScale = new Vector3(canvasScaleX, canvasMessagePlayer.localScale.y, canvasMessagePlayer.localScale.z);
    }

    //para hacer flip cuando estamos en un dialogo y mirar hacia el npc (metodo referenciado en los scripts de los npc)
    public void LookAtNPC(Vector2 npcPosition)
    {
        if((npcPosition.x>transform.position.x && !facingRight) || (npcPosition.x < transform.position.x && facingRight))
        {
            Flip(); //hacemos flip si estamos "dados la vuelta" respecto al npc
           
        }
        animator.Play("Idle");
    }

    public void LookAtAlquimista()
    {
        if(!facingRight)
        {
            Flip(); //hacemos flip si estamos mirando a la izquierda
           
        }
        animator.Play("Idle");
    }

    //para que salga el numero de corazones segun la vida
    public void UpdateHealthUI()
    {
        float healthWidth = currentHealth * widthPerHealth;

        healthBar.sizeDelta = new Vector2(healthWidth, healthBar.sizeDelta.y);

        float deadWidth = (maxHealth-currentHealth)*widthPerHealth;
        deadBar.sizeDelta = new Vector2(deadWidth, deadBar.sizeDelta.y);
    }

    //DAR LOS DATOS AL PLAYER, QUE VIENEN DEL SAVEDATA (donde se dan los valores reales a las variables)
    public void ApplySaveDataToPlayer()
    {
        if(GameController.Instance!=null && GameController.Instance.currentSD != null)
        {
            PlayerData PlayerData = GameController.Instance.currentSD.playerData;

            maxHealth=PlayerData.maxHealth;
            currentHealth=PlayerData.currentHealth; //para recuperar todos los corazones
            skillsList = PlayerData.skillsList;
            itemsList = PlayerData.itemsList;
            

        
          
                transform.position = new Vector3(PlayerData.checkpointX,PlayerData.checkpointY,-1.46f); 
            UpdateHealthUI();
        }
    }

    //Dar los datos al player, pero no desde el savedata, para cuando venimos de un TP (cogemos los datos de currentSD)
        public void ApplySaveDataToPlayerWhenTP()
    {
        if(GameController.Instance!=null && GameController.Instance.currentSD != null)
        {
           transform.position = new Vector3(GameController.Instance.nextSpawnPosition.x, GameController.Instance.nextSpawnPosition.y, -1.46f);
            GameController.Instance.useNextSpawn = false;

            Debug.Log("Teletransportandome tengo "+ GameController.Instance.currentHealthTP+" puntos de vida");
            currentHealth = GameController.Instance.currentHealthTP;

            

            maxHealth = GameController.Instance.currentSD.playerData.maxHealth;
            skillsList = GameController.Instance.currentSD.playerData.skillsList;
            itemsList = GameController.Instance.currentSD.playerData.itemsList;

        UpdateHealthUI();

            if (GameController.Instance.flipOnSpawn)
            {
                Flip();
            }
        }
    }


    //CAMBIAR LOS DATOS DEL PLAYER CUANDO QUERAMOS GUARDAR EN SAVEDATA
    public void UpdateDataPlayer()
    {
        if (GameController.Instance != null && GameController.Instance.currentSD != null)
    {
        // Tomamos los valores del jugador y los volcamos en el save
        GameController.Instance.currentSD.playerData.maxHealth = maxHealth;
        GameController.Instance.currentSD.playerData.currentHealth = currentHealth;
        GameController.Instance.currentSD.playerData.checkpointX = transform.position.x;
        GameController.Instance.currentSD.playerData.checkpointY = transform.position.y;
        GameController.Instance.currentSD.playerData.skillsList = skillsList;
        GameController.Instance.currentSD.playerData.currentNameScene = SceneManager.GetActiveScene().name;

        Debug.Log("SaveData actualizado desde PlayerController");
    }
    else
    {
        Debug.LogWarning("No se pudo actualizar SaveDatan porque GameController o currentSD es null");
    }
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

            if (currentHealth == 0)
            {
                Die();
                return;
            }
            StartCoroutine(coroutineInvulnerable()); //empieza tiempo de invulnerabilidad
        }
    }



    //método morir
    private void Die()
    {
        PlaySounds("Death");
        Time.timeScale = 0f; //se pausa el juego
        DeathMenu.SetActive(true); //se despliega el menú
        if (MusicManager.Instance != null)
    {
        MusicManager.Instance.SetDeathFilter(true);
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

    //PLATAFORMAS MOVILES 
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            currentPlatform = collision.gameObject.GetComponent<PlataformaMovil>();
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform")){
            currentPlatform=null;
        }
    }

    //para reproducir los sonidos del player

    //pitch random true
    public void PlaySoundsPitch(string soundName)
    {
        SoundEffectManager.Instance.Play(soundName, true);
    }

    //pitch random false
    public void PlaySounds(string soundName)
    {
        SoundEffectManager.Instance.Play(soundName, false);
        
    }

    //sonido ambiental (pasos) con pitch cambiado
    public void AnimSound(string soundName)
{
    SoundEffectManager.Instance.PlayAtPosition(soundName, transform.position, true);
}

    public void AnimSoundNo(string soundName)
{
    SoundEffectManager.Instance.PlayAtPosition(soundName, transform.position, false);
}
}

