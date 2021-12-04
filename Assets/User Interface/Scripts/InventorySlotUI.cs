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

        public void Set(Stack stack)
        {
            if (stack is {})
            {
                icon.enabled = true;
                icon.sprite = stack.item.sprite;
                count.text = stack.amount.ToString();
            }
            else
            {
                icon.enabled = false;
                count.text = "";
            }
        }
    }
}
