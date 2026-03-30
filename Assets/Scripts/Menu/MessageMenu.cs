using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageMenu : MonoBehaviour
{

    public GameObject panel; //el panel que vamos a activar/desactivar
    public TMP_Text msgText;
    public float msgDuration = 5f;

    private Coroutine currentCoroutine;
    
    public static MessageMenu Instance;

    void Awake()
{
    if (Instance == null)
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    else
    {
        Destroy(gameObject);
    }
}
    public void ShowMessage(string texto)
    {
        msgText.text= texto;

        panel.SetActive(true);

        //reiniciamos corrutina si ya habia un mensaje
        if(currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        currentCoroutine = StartCoroutine(HideMessage());
    }

    private IEnumerator HideMessage()
    {
        yield return new WaitForSeconds(msgDuration);
        panel.SetActive(false);
    }


    }

