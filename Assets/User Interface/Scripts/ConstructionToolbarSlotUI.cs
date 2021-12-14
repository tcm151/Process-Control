using System.Threading.Tasks;
using ProcessControl.Graphs;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using ProcessControl.Industry;
using UnityEngine;


namespace ProcessControl.UI
{
    public class ConstructionToolbarSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public Schematic schematic;
        public bool isEdge;
        public bool isNode => !isEdge;
        
        private Button button;
        private Image[] images;

        private bool cancelShow;

        private void Awake() => Initialize();
        private void OnValidate() => Initialize();

        public async void OnPointerEnter(PointerEventData eventData)
        {
            cancelShow = false;
            await Task.Delay(500);
            if (cancelShow) return;
            TooltipPanel.ShowTooltip(schematic.name, schematic.description);
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            cancelShow = true;
            TooltipPanel.HideTooltip(0.33f);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (schematic is null) return;
            ConstructionManager.SetPart(schematic);
            ConstructionManager.SetEdgeMode(isEdge);
        }

        private void Initialize()
        {
            if (schematic is null) return;
            // if (schematic.entity is Node) return;
            images = GetComponentsInChildren<Image>();
            images[1].sprite = schematic.sprite;
        }

    }
}
