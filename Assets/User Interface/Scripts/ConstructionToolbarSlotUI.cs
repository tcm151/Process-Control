using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using ProcessControl.Graphs;
using ProcessControl.Industry;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;


namespace ProcessControl.UI
{
    public class ConstructionToolbarSlotUI : Tooltip
    {
        public Schematic schematic;
        public bool isEdge;
        public bool isNode => !isEdge;
        
        private Button button;
        private Image[] images;

        private void Awake() => Initialize();
        private void OnValidate() => Initialize();

        override public async void OnPointerEnter(PointerEventData eventData)
        {
            cancelShow = false;
            await Task.Delay(500);
            if (cancelShow) return;
            TooltipWindow.ShowTooltip(schematic.name, schematic.description);
        }

        private void Initialize()
        {
            if (schematic is null) return;

            button = GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                ConstructionManager.SetPart(schematic);
                ConstructionManager.SetEdgeMode(isEdge);
            });

            // if (part.entity is Node) return;
            images = GetComponentsInChildren<Image>();
            images[1].sprite = schematic.sprite;
        }
        
    }
}
