using UnityEngine;
using Grid = ProcessControl.Terrain.Grid;


namespace ProcessControl.Machines
{
    abstract public class Entity : MonoBehaviour
    {
        // public Grid.Cell cell;

        abstract public void Delete();
    }
}