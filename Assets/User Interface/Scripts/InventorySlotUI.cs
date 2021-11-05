using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ProcessControl.Industry;


namespace ProcessControl
{
    public class InventorySlotUI : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI count;

        public void Set(ItemAmount inventorySlot)
        {
            
            if (inventorySlot is {})
            {
                icon.enabled = true;
                icon.sprite = inventorySlot.item.sprite;
                count.text = inventorySlot.amount.ToString();
            }
            else
            {
                icon.enabled = false;
                count.text = "";
            }
        }
    }
}
