using System.Threading;
using System.Threading.Tasks;
using ProcessControl.Industry;
using ProcessControl.Tools;
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
        //     TooltipPanel.ShowTooltip(header, content);
        // }
        
        virtual public async void OnPointerEnter(PointerEventData eventData)
        {
            cancelShow = false;
            await Alerp.Delay(0.5f);
            if (cancelShow) return;
            TooltipPanel.ShowTooltip(null, null);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            cancelShow = true;
            TooltipPanel.HideTooltip(0.33f);
        }
    }
    
}