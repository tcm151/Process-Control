using UnityEngine;
using ProcessControl.Procedural;
using ProcessControl.Construction;
#pragma warning disable 108,114

namespace ProcessControl
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class TileSelectorUI : MonoBehaviour
    {
        private Camera camera;
        private SpriteRenderer renderer;

        private void Awake()
        {
            camera = Camera.main;
            renderer = GetComponent<SpriteRenderer>();
            
            BuildManager.OnBuildModeChanged += UpdateVisible;
        }

        private void UpdateVisible(bool truth) => renderer.enabled = truth;

        private void Update()
        {
            var cell = ProceduralGrid.GetCellUnderMouse();
            transform.position = cell.position;
        }
    }
}
