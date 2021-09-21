using UnityEngine;
using UnityEngine.UI;
using ProcessControl.Construction;
using ProcessControl.Graphs;
using ProcessControl.Industry.Machines;
using ProcessControl.Industry.Conveyors;


namespace ProcessControl.UI
{
    public class BuildToolbarItem : MonoBehaviour
    {
        public Node node;
        public Edge edge;
        public bool isEdge;
        
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            
            button.onClick.AddListener(() =>
            {
                BuildManager.SetNode(node);
                BuildManager.SetEdge(edge);
                BuildManager.SetEdgeMode(isEdge);
            });
        }
    }
}
