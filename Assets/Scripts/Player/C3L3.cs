using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C3L3 : MonoBehaviour
{
    public int myInteger = 5;
    public float myFloat = 3.5f;
    public bool myBoolean=true;
    public string myString = "Hello world";
    public int[] myArrayInt;

    private int _myPrivateInteger = 10;
    float _myPrivateFloat = -0.5f;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("esto es antes de sumar "+myInteger);
        myInteger += 10;
        Debug.Log("esto es despues de sumar "+myInteger);

        if(IsEven(myInteger)){
            MyDebug("Es Par");
        }
        else{
            MyDebug("Es Impar");
        }

        SpriteRenderer mySpriteRenderer = GetComponent<SpriteRenderer>(); //Si es null el juego peta

        if(mySpriteRenderer != null){
            MyDebug("Puedo Usar mySpriteRenderer");
        }
        else{
            MyDebug("El juego peta si usas este componente"); 
        }
        GameObject anObjectWithSpriteRenderer = FindObjectOfType<SpriteRenderer>().gameObject; //encuentra el primer objeto que tenga spriterender

        GameObject anObjectWithTag = GameObject.FindWithTag("Player");

        GameObject anObjectWithName = GameObject.Find("Name");

        //activar o desactivar componentes de un objeto
        if(mySpriteRenderer!=null){
            mySpriteRenderer.enabled=false; //desactiva el spriteRenderer (es una checkbox)
            
        }
        //Activar o desactivar un objeto
        if(anObjectWithName != null){
            anObjectWithName.SetActive(false);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool IsEven(int num){
        if(num%2==0){
            return true;
        }
        else{
            return false;
        }
    }

    void MyDebug(string message){
        Debug.Log(message);
    }
}
