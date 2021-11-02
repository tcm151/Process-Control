using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using Debug = UnityEngine.Debug;
using ProcessControl.Tools;
using ProcessControl.Procedural;
#pragma warning disable 108,114


namespace ProcessControl.Pathfinding
{
    public class PathfindingVisualizer : MonoBehaviour
    {
        private Camera camera;

        public TextMesh tileTextPrefab;
        private List<TextMesh> textObjects = new List<TextMesh>();

        public bool noPause;
        
        public List<TileBase> tiles;
        private Tilemap tilemap;

        private Vector3 startPoint, endPoint;

        private void Awake()
        {
            camera = Camera.main;
            tilemap = GetComponentInChildren<Tilemap>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                startPoint = camera.MousePosition2D();
            }

            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                endPoint = camera.MousePosition2D();
                var path = FindPath(startPoint, endPoint);
                // do something else maybe
            }

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                tilemap.ClearAllTiles();
                textObjects.ForEach(t => Destroy(t.gameObject));
                textObjects.Clear();
            }
        }

        public void SetTileAndColor(Vector3 position, Color color)
        {
            tilemap.SetTile(position.FloorToInt(), tiles[0]);
            tilemap.SetTileFlags(position.FloorToInt(), TileFlags.None);
            tilemap.SetColor(position.FloorToInt(), color);
        }

        public async Task<List<Vector3>> FindPath(Vector3 start, Vector3 end)
        {
            var timer = new Stopwatch();
            timer.Start();
            
            var startCell = TileGrid.GetCellAtPosition(start);
            var endCell = TileGrid.GetCellAtPosition(end);
            if (startCell is null || endCell is null)
            {
                Debug.Log($"Start or End cell(s) did not exist...");
                return new List<Vector3> {start};
            }
            if (!endCell.buildable)
            {
                Debug.Log("End tile was not navigable");
                return new List<Vector3> {start};
            }
            
            SetTileAndColor(startCell.position, Color.blue);
            SetTileAndColor(endCell.position, Color.blue);
            
            endCell.pathInfo.Reset();
            startCell.pathInfo.Reset();
            
            var openList = new List<Cell> {startCell};
            var closedList = new List<Cell>();

            startCell.pathInfo.Set(0, DistanceBetween(startCell, endCell), null);
            // startCell.pathInfo.gCost = 0;
            

            var steps = 0;
            var minimumDistance = DistanceBetween(startCell, endCell);
            while (openList.Count > 0 && ++steps < minimumDistance * 32)
            {
                var currentCell = openList.OrderBy(pc => pc.pathInfo.fCost).First();
                
                SetTileAndColor(currentCell.position, Color.green);
                
                while (!Input.GetKeyDown(KeyCode.Space) !^ noPause) await Task.Yield();

                if (currentCell == endCell)
                {
                    var finalPath = RetracePath(endCell);
                    if (finalPath is null)
                    {
                        Debug.Log($"Path did not exist between {startCell.coords} and {endCell.coords}");
                        return new List<Vector3> { start };
                    }
                    
                    finalPath.ForEach(v => SetTileAndColor(v, Color.blue));
                    Debug.Log($"{finalPath.Count} m path in {timer.ElapsedMilliseconds} ms");
                    return finalPath;
                }
                
                if (!currentCell.buildable || currentCell.occupied)
                {
                    openList.Remove(currentCell);
                    closedList.Add(currentCell);
                    currentCell.pathInfo.Reset();
                    SetTileAndColor(currentCell.position, Color.red);
                    continue;
                }


                closedList.Add(currentCell);
                openList.Remove(currentCell);
                SetTileAndColor(currentCell.position, Color.gray);

                // add neighbours to open list
                currentCell.neighbours.ForEach(neighbourCell =>
                {
                    if (neighbourCell is null) return;
                    if (closedList.Contains(neighbourCell) || openList.Contains(neighbourCell)) return;
                    if (!neighbourCell.buildable || neighbourCell.occupied)
                    {
                        closedList.Add(neighbourCell);
                        neighbourCell.pathInfo.Reset();
                        SetTileAndColor(neighbourCell.position, Color.red);
                        return;
                    }

                    SetTileAndColor(neighbourCell.position, Color.magenta);
                    neighbourCell.pathInfo.Reset();
                    float gCost = currentCell.pathInfo.gCost + DistanceBetween(currentCell, neighbourCell);
                    if (gCost >= neighbourCell.pathInfo.gCost) return;
                    neighbourCell.pathInfo.Set(gCost, DistanceBetween(neighbourCell, endCell), currentCell);
                    // neighbourCell.pathInfo.previousInPath = currentCell;
                    // neighbourCell.pathInfo.gCost = gCost;
                    // neighbourCell.pathInfo.hCost = DistanceBetween(neighbourCell, endCell);
                    if (!openList.Contains(neighbourCell)) openList.Add(neighbourCell);

                    var tileText = Factory.Spawn(tileTextPrefab, neighbourCell.position);
                    tileText.text = neighbourCell.pathInfo.fCost.ToString("F1");
                    textObjects.Add(tileText);
                });

                await Task.Yield();
            }
            
            // no open nodes left
            Debug.Log($"Open list emptied after {timer.ElapsedMilliseconds} ms");
            return new List<Vector3> { start };
        }

        private static List<Vector3> RetracePath(Cell endCell)
        {
            var currentCell = endCell;
            var reversedPath = new List<Cell> { currentCell };

            while (currentCell.pathInfo.previousInPath is { })
            {
                reversedPath.Add(currentCell.pathInfo.previousInPath);
                currentCell = currentCell.pathInfo.previousInPath;
            }

            reversedPath.Reverse();
            reversedPath.RemoveAt(0);
            return reversedPath.ConvertAll(c => c.position);
        }

        private static readonly float D = 1;
        private static readonly float D2 = Mathf.Sqrt(2);
        
        private static float DistanceBetween(Cell first, Cell second)
        {
            var d = (first.position - second.position).Abs();
            return D * Mathf.Max(d.x, d.y) + (D2-D) * Mathf.Min(d.x, d.y);
        }
    }
}
