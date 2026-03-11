using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SlotMenu : MonoBehaviour
{

    public TMP_Text[] textSlotsArray; //los textos que se van a mostrar en la seleccion de slots
    public Button[] buttonSlotArray; //los botones, para cambiar los fondos, el aspecto etc
    public Button[] buttonSlotDeleteArray; //botones de eliminar partida

    void Start()
    {
        RefreshSlots();
    }
    public void SelectSlot(int slotId)
    {
        GameController.Instance.SlotSelectorPlayOrLoad(slotId);
    }
    void RefreshSlots()
    {
            for(int i=0; i<buttonSlotArray.Length; i++)
        {
            if (SaveSystem.SlotExists(i))
            {
                textSlotsArray[i].text="Cargar partida";
                buttonSlotArray[i].image.color=Color.white;
                buttonSlotDeleteArray[i].gameObject.SetActive(true);
            }
            else
            {
                textSlotsArray[i].text="Vacío";
                buttonSlotArray[i].image.color=new Color(0.4279091f, 0.3737985f, 0.4528302f, 1f);
                buttonSlotDeleteArray[i].gameObject.SetActive(false); //desactivar botón borrar partida si no hay partida

            }
        }
    }
    public void DeleteSlot(int slotId)
    {
        SaveSystem.DeleteSlotData(slotId);
        RefreshSlots(); //para que se actualice la UI al hacer el cambio (borrado de partida)
    }
}