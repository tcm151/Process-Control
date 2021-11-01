using System;
using UnityEngine;
using UnityEngine.UI;
using ProcessControl.Construction;
using ProcessControl.Graphs;


namespace ProcessControl.UI
{
    public class BuildToolbarItem : MonoBehaviour
    {
        public Node node;
        public Edge edge;
        public bool isEdge;
        
        private Button button;
        private Image[] images;

        private void Awake() => Initialize();
        private void OnValidate() => Initialize();

        private void Initialize()
        {
            if (node is null) return;

            button = GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                ConstructionManager.SetNode(node);
                ConstructionManager.SetEdge(edge);
                ConstructionManager.SetEdgeMode(isEdge);
            });

            if (edge is null) return;
            images = GetComponentsInChildren<Image>();
            images[1].sprite = (edge is { })
                ? edge.GetComponent<SpriteRenderer>().sprite
                : node.GetComponent<SpriteRenderer>().sprite;
        }
        
    }
}
