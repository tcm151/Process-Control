using System;
using System.Linq;
using System.Collections.Generic;
using ProcessControl.Tools;


namespace ProcessControl.Pathfinding
{
    public class Path<T>
    {
        public T currentPoint;
        public readonly List<T> pathPoints;
        
        // public event Action onPathFailed;
        public event Action onPathCompleted;

        public bool completed;

        public int Count => pathPoints.Count;
        public T Destination => pathPoints.Last();
        
        public Path(List<T> newPath)
        {
            pathPoints = newPath;
            currentPoint = pathPoints.First();
        }

        public T NextPoint()
        {
            if (currentPoint.GetType() == pathPoints.Last().GetType())
            {
                onPathCompleted?.Invoke();
                return default;
            }
            
            return pathPoints.NextAfter(currentPoint);
        }
    }
}