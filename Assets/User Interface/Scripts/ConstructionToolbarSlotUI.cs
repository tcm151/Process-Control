using UnityEngine;
using UnityEngine.UI;
using ProcessControl.Graphs;
using ProcessControl.Industry;


namespace ProcessControl.UI
{
    public class ConstructionToolbarSlotUI : MonoBehaviour
    {
        public Part part;
        public bool isEdge;
        public bool isNode => !isEdge;
        
        private Button button;
        private Image[] images;

        private void Awake() => Initialize();
        private void OnValidate() => Initialize();

        private void Initialize()
        {
            if (part is null) return;

            button = GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                ConstructionManager.SetPart(part);
                ConstructionManager.SetEdgeMode(isEdge);
            });

            if (part.entity is Node) return;
            images = GetComponentsInChildren<Image>();
            images[1].sprite = part.sprite;
        }
        
    }
}
