using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float speed; //la velocidad a la que se va a mover el objeto
    
    public Vector2 direction; 

    //AUTODESTRUIR
    public float livingTime = 3f;

    //PARA QUE EL COLOR CAMBIE CUANDO ESTE APUNTO DE EXPLOTAR LA BALA
    private SpriteRenderer renderer;
    public Color initialColor=Color.white;
    public Color finalColor=Color.red;
    private float startingTime;
    void Awake(){
        renderer = GetComponent<SpriteRenderer>();
    }

        // Start is called before the first frame update
    void Start()
    {
        //PARA QUE EL COLOR CAMBIE CUANDO ESTE APUNTO DE EXPLOTAR LA BALA
        startingTime = Time.time;
        //AUTODESTRUIR
        Destroy(this.gameObject, livingTime);
    }

    // Update is called once per frame
    void Update()
    {
        //¿cuanto se tiene que mover en cada frame?
    Vector2 movement = direction.normalized * speed * Time.deltaTime; //normalizamos el vector y lo multiplicamos por la velocidad

   
    //transform.position = new Vector2(transform.position.x + movement.x, transform.position.y + movement.y); 
    transform.Translate(movement);

    //PARA QUE EL COLOR CAMBIE CUANDO ESTE APUNTO DE EXPLOTAR LA BALA
    float timeSinceStarted = Time.time - startingTime;
    float pcentCompleted= timeSinceStarted/livingTime;

    renderer.color=Color.Lerp(initialColor,finalColor,pcentCompleted);
    
}
}
