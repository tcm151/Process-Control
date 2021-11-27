using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ProcessControl.Tools;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;


namespace ProcessControl.Procedural
{
    public static class CellSpawner
    {
        public static event Action<Vector2> onStartLocationDetermined;

        public static void CalculateSpawnLocation()
        {
            var timer = new Stopwatch();
            timer.Start();
            
            var openList = new List<Cell>();
            var closedList = new List<Cell>();
            var startingCell = CellGrid.GetCellAtCoordinates(Vector2Int.zero);
            openList.Add(startingCell);

            var steps = 1;
            while (openList.Count >= 1 && ++steps < 10_000)
            {
                var currentCell = openList.TakeAndRemoveFirst();
                
                if (currentCell.biome != Biome.Ocean)
                {
                    Debug.Log($"Start coords {currentCell.coords} |= {timer.ElapsedMilliseconds} ms");
                    if (Application.isPlaying) onStartLocationDetermined?.Invoke(currentCell.position);
                    return;
                }

                closedList.Add(currentCell);
                for (int i = 0; i < currentCell.neighbours.Length; i++)
                {
                    if (!closedList.Contains(currentCell.neighbours[i]) && !openList.Contains(currentCell.neighbours[i]))
                    {
                        if (i == 0 || i == 2 || i == 5 || i == 7)
                            openList.Add(currentCell.neighbours[i]);
                    }
                }
            }

            Debug.Log("Unable to find world spawn...");
        }

        public static Vector2 GenerateRandomSpawn(Func<Cell, bool> predicate, Vector2Int origin = default, int range = 0)
        {
            var openList = new List<Cell>();
            var closedList = new List<Cell>();
            var spawnOffset = (Random.insideUnitCircle.normalized * (Random.value * range)).FloorToInt();
            var startingCell = CellGrid.GetCellAtCoordinates(origin + spawnOffset);
            openList.Add(startingCell);

            var steps = 0;
            while ((openList.Count >= 1) && (steps++ < 10_000))
            {
                var currentCell = openList.TakeAndRemoveFirst();
                
                if (predicate(currentCell)) return currentCell.position;

                closedList.Add(currentCell);
                currentCell.neighbours.ForEachWhere(n => !closedList.Contains(n) && !openList.Contains(n), n => openList.Add(n));
            }

            Debug.Log("No spawn was found...");
            return default;
        }
    }
}