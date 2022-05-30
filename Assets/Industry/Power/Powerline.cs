using ProcessControl.Graphs;
using UnityEngine;


namespace ProcessControl.Industry.Power
{
    public class Powerline : Edge
    {
        public override float Length => 0f;
        public override Vector3 Center => transform.position;
    }
}