using System;
using System.Collections.Generic;
using ProcessControl.Tools;


namespace ProcessControl.Machines
{
    public class Edge : Entity, IO
    {
        [Serializable] public class Data
        {
            public int ticks;
            public bool sleeping;
            
            public Node input;
            public Node output;

            public List<Resource> inventory = new List<Resource>();
        }
        
        public Data edge;

        override public void Delete() { }

        public bool Full => edge.inventory.Count >= InventorySize;
        public int InventorySize => (int) Node.DistanceBetween(edge.input, edge.output);


        private void Awake()
        {
            edge = new Data();
        }

        public void ConnectInput(Edge input)
        {
            edge.input = input.edge.output;
        }

        public void ConnectOutput(Edge output)
        {
            edge.output = output.edge.input;
        }

        public void Deposit(Resource resource) => edge.inventory.Add(resource);
        public Resource Withdraw() => edge.inventory.TakeFirst();
    }
}