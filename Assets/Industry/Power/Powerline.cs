using ProcessControl.Graphs;
using UnityEngine;


namespace ProcessControl.Industry.Power
{
    public class Powerline : Edge
    {
        override public float Length => 0f;
        override public Vector3 Center => transform.position;
    }
}