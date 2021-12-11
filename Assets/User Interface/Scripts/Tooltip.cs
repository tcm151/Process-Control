using System.Threading;
using System.Threading.Tasks;
using ProcessControl.Industry;
using UnityEngine;
using UnityEngine.EventSystems;


namespace ProcessControl.UI
{
    public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        protected bool cancelShow;
        
        // public string header;
        // [TextArea] public string content;

        // virtual protected void ShowTooltip(string header = null, string content = null)
        // {
        //     TooltipWindow.ShowTooltip(header, content);
        // }
        
        virtual public async void OnPointerEnter(PointerEventData eventData)
        {
            cancelShow = false;
            await Task.Delay(500);
            if (cancelShow) return;
            TooltipWindow.ShowTooltip(null, null);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            cancelShow = true;
            TooltipWindow.HideTooltip();
        }
    }
    
}