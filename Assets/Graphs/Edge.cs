using System;
using ProcessControl.Machines;


namespace ProcessControl.Graphs
{
    abstract public class Edge : Entity
    {
        [Serializable]
        public class Data
        {
            public Node input;
            public Node output;
        }
        public Data edgeData;
        
        // abstract public bool Full {get;}
        // abstract public bool Empty {get;}
        // abstract public int InventorySize {get;}

        override public void Delete()
        {
            
        }
    }
}