using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingRaycast2D : MonoBehaviour
{
    private Animator animator;
    private Weapon weapon;
    void Awake(){
        animator = GetComponent<Animator>();
        weapon = GetComponentInChildren<Weapon>();

    }
    // Start is called before the first frame update
    void Start()
    {
        animator.SetBool("Idle", true);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1")){
            animator.SetTrigger("New Trigger");
        }
    }
    void CanShoot(){
        if(weapon !=null){

            //llamamos a la corrutina del rayo (dentro del weapon está configurado el line renderer acuérdate)
            StartCoroutine(weapon.ShootWithRaycast());

        }
    }
}
