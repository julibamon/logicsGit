using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenusFirstButton : MonoBehaviour
{

public Button first;

    void OnEnable()
    {
        if(first != null)
        {
            EventSystem.current.SetSelectedGameObject(null); //reseteamos lo anterior
            EventSystem.current.SetSelectedGameObject(first.gameObject); //ponemos el asignado a first
        }
    }
}
