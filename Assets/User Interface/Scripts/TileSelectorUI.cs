using UnityEngine;
using ProcessControl.Tools;
using ProcessControl.Industry;
using ProcessControl.Procedural;

#pragma warning disable 108,114

namespace ProcessControl
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class TileSelectorUI : MonoBehaviour
    {
        private Camera camera;
        private SpriteRenderer renderer;

        private Vector2Int currentCoords;
        
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

            var coords = camera.MousePosition2D().FloorToInt();
            if (coords == currentCoords) return;
            currentCoords = coords;
            
            var cell = TileGrid.GetCellUnderMouse();
            if (cell is { }) transform.position = cell.position;
        }
    }
}
