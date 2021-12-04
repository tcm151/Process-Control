using ProcessControl.Graphs;
using UnityEngine;


namespace ProcessControl.Industry.Power
{
    public class Battery: Node
    {
        [Header("Battery")]
        private float energy;
        private float maxEnergy;
    }
}