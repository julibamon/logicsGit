using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PruebaJulia : MonoBehaviour

{
    public float speed = 100f; //la velocidad a la que se va a mover el objeto

    public Vector2 direction;
    // Start is called before the first frame update
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 movement = direction.normalized * speed * Time.deltaTime; //normalizamos el vector y lo multiplicamos por la velocidad

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("PULSASTE CLICK IZQUIERDO");
            direction.x = -direction.x;
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("PULSASTE CLICK DERECHO");
            direction.x = -direction.x;
        }
        transform.Translate(movement);
    }
}
