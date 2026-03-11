using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SlotMenu : MonoBehaviour
{

    public TMP_Text[] textSlotsArray; //los textos que se van a mostrar en la seleccion de slots
    public Button[] buttonSlotArray; //los botones, para cambiar los fondos, el aspecto etc

    void Start()
    {
        
        for(int i=0; i<textSlotsArray.Length; i++)
        {
            if (SaveSystem.SlotExists(i))
            {
                textSlotsArray[i].text="Cargar partida";
                buttonSlotArray[i].image.color=new Color(,,,);
            }
            else
            {
                textSlotsArray[i].text="Nueva partida";
                buttonSlotArray[i].image.color=new Color(,,,); 
            }
    }
    }
    public void SelectSlot(int slotId)
    {
        GameController.Instance.SlotSelectorPlayOrLoad(slotId);
    }
}