using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingKeys : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //RATON
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("PULSASTE BOTON");
        }
        if (Input.GetMouseButton(0))
        {
            Debug.Log("BOTON ESTA PULSANDOSE");
        }
        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("BOTON LIBRE");
        }

        // teclado

        if (Input.GetKeyDown(KeyCode.Space))
        { // mejor usar inputs de unity
            Debug.Log("Salta1");

        }
        //para usar los inputs
        if (Input.GetButtonDown("Jump"))
        {
            Debug.Log("Salta");

        }
        //AXIS(eje) PA MOVIMIENTO
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if(horizontal < 0f || horizontal > 0f){
            Debug.Log("Horizontal axis is "+horizontal);
        }
        if(vertical < 0f || vertical > 0f){
            Debug.Log("vertical axis is "+vertical);
        }

    }
}
