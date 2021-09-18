// using System;
// using System.Collections.Generic;
// using ProcessControl.Graphs;
//
//
// namespace ProcessControl.Machines
// {
//     class ConveyorNode : Node, IO<Conveyor>
//     {
//         [Serializable] new public class Data
//         {
//             public bool sleeping;
//             public int ticks = 0;
//             
//             public int inventorySize = 64;
//             
//             public List<Conveyor> inputs = new List<Conveyor>();
//             public List<Conveyor> outputs = new List<Conveyor>();
//             public List<Resource> inventory = new List<Resource>();
//         }
//         public Data transportNode;
//
//         public bool Full => false;
//         public bool Empty => false;
//         public int InventorySize => 2;
//
//         virtual public Conveyor Input => transportNode.inputs[0];
//         virtual public Conveyor Output => transportNode.outputs[0];
//
//         virtual public void ConnectInput(Conveyor input) => transportNode.inputs[0] = input;
//         virtual public void ConnectOutput(Conveyor output) => transportNode.outputs[0] = output;
//
//         virtual public void Deposit(Resource resource) => transportNode.outputs[0].Deposit(resource);
//         virtual public Resource Withdraw() => transportNode.inputs[0].Withdraw();
//     }
// }