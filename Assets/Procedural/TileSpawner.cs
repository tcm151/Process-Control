using System;
using System.Collections.Generic;
using System.Linq;
using ProcessControl.Tools;
using UnityEngine;
using Random = UnityEngine.Random;


namespace ProcessControl.Procedural
{
    public class TileSpawner
    {
        public static event Action<Vector2> onStartLocationDetermined;

        public static Vector2 CalculateSpawnLocation()
        {
            var openList = new List<Cell>();
            var closedList = new List<Cell>();
            var startingCell = TileGrid.GetCellAtCoordinates(Vector2Int.zero);
            openList.Add(startingCell);

            var steps = 1;
            while (openList.Count >= 1 && ++steps < 10_000)
            {
                var currentCell = openList.First();
                
                if (currentCell.biome != Biome.Ocean)
                {
                    Debug.Log($"Start coords: {currentCell.coords}");
                    if (Application.isPlaying) onStartLocationDetermined?.Invoke(currentCell.position);
                    // onStartLocationDetermined?.Invoke(currentCell.position);
                    return currentCell.position;
                }

                closedList.Add(currentCell);
                openList.Remove(currentCell);
                currentCell.neighbours.ForEach(n =>
                {
                    if (!closedList.Contains(n) && !openList.Contains(n))
                        openList.Add(n);
                });
            }

            Debug.Log("Unable to find world spawn...");
            return default;
        }

        public static Vector2 GenerateSpawn(Func<Cell, bool> predicate, Vector2Int origin = default, int range = 0)
        {
            var openList = new List<Cell>();
            var closedList = new List<Cell>();

            var offset = (Random.insideUnitCircle.normalized * Random.value * range).FloorToInt();
            var startingCell = TileGrid.GetCellAtCoordinates(origin + offset);
            openList.Add(startingCell);

            var steps = 0;
            while ((openList.Count >= 1) && (steps++ < 10_000))
            {
                var currentCell = openList.TakeFirst();
                
                if (predicate(currentCell)) return currentCell.position;

                closedList.Add(currentCell);
                currentCell.neighbours.ForEach(n =>
                    {
                        if (!closedList.Contains(n) && !openList.Contains(n)) openList.Add(n);
                    }
                );
            }

            Debug.Log("No spawn was found...");
            return default;
        }
    }
}