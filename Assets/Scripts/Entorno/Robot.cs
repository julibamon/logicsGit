using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Robot : MonoBehaviour
{

    public GameObject conjuntoA;
    public GameObject conjuntoB;

    private bool isActiveA = true;
    private bool isActiveB = false;
    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetButtonDown("Jump"))
        {
            conjuntoA.SetActive(isActiveA);
            conjuntoB.SetActive(isActiveB);

            isActiveA = !isActiveA;
            isActiveB = !isActiveB;

        }
    }
}
