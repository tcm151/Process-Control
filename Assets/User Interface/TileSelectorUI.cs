using UnityEngine;
using ProcessControl.Procedural;
using ProcessControl.Construction;
using UnityEngine.EventSystems;

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
            
            ConstructionManager.OnBuildModeChanged += UpdateVisible;
        }

        private void UpdateVisible(bool truth) => renderer.enabled = truth;

        private void Update()
        {
            // if (!EventSystem.current.IsPointerOverGameObject()) return;
            
            var cell = TileGrid.GetCellUnderMouse();
            if (cell is { }) transform.position = cell.position;
        }
    }
}
