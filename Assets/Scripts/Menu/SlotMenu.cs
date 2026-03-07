using UnityEngine;

public class SlotMenu : MonoBehaviour
{
    public void SelectSlot(int slotId)
    {
        GameController.Instance.SlotSelectorPlayOrLoad(slotId);
    }
}