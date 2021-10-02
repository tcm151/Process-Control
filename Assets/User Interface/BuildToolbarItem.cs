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

        private void Awake()
        {
            if (node is null && edge is null) return;

            button = GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                BuildManager.SetNode(node);
                BuildManager.SetEdge(edge);
                BuildManager.SetEdgeMode(isEdge);
            });

            images = GetComponentsInChildren<Image>();
            images[1].sprite = (edge is { })
            ? edge.GetComponent<SpriteRenderer>().sprite
            : node.GetComponent<SpriteRenderer>().sprite;
        }
    }
}
