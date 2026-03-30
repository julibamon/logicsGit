using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuertaQuest : MonoBehaviour
{
    // Start is called before the first frame update

    private Vector3 closedPosition;
    private bool isOpen = false;
    void Start()
    {
        closedPosition = transform.position;

        CheckState();
    }

    public void CheckState()
    {
        if (GameController.Instance.currentSD.worldData.activatedEvents.Contains("AlquimistaCALDERO"))
        {
            OpenDoor();
        }
    }

    public void OpenDoor()
    {
        transform.position=new Vector3(transform.position.x,17.65f,transform.position.z);
        isOpen = true;
    }

}
