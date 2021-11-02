using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace ProcessControl
{
    public class InventorySlotUI : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI count;

        public void Set(KeyValuePair<Item, int> inventorySlot)
        {
            if (inventorySlot.Value >= 1)
            {
                icon.enabled = true;
                icon.sprite = inventorySlot.Key.sprite;
                count.text = inventorySlot.Value.ToString();
            }
            else
            {
                icon.enabled = false;
                count.text = "";
            }
        }
    }
}
