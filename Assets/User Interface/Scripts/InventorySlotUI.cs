using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ProcessControl.Industry;


namespace ProcessControl.UI
{
    public class InventorySlotUI : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI count;

        public void Set(ItemAmount itemAmount)
        {
            if (itemAmount is {})
            {
                icon.enabled = true;
                icon.sprite = itemAmount.item.sprite;
                count.text = itemAmount.amount.ToString();
            }
            else
            {
                icon.enabled = false;
                count.text = "";
            }
        }
    }
}
