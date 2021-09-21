// using System;
// using System.Collections.Generic;
// using UnityEngine;
// using ProcessControl.Tools;
// using ProcessControl.Graphs;
// using ProcessControl.Industry.Machines;
// using ProcessControl.Industry.Resources;
//
// [SelectionBase]
// public class TestMachine : Node, IInput, IOutput
// {
//     protected const int TicksPerSecond = 64;
//     
//     //> MACHINE DATA CONTAINER
//     [Serializable] new public class Data
//     {
//         public bool sleeping;
//         public int ticks;
//         public int sleepThreshold = 256;
//
//         [Header("Inventory")]
//         public int inventorySize = 8;
//         // public List<Resource> inventory = new List<Resource>();
//         
//         [Header("Input")]
//         public bool inputEnabled = true;
//         public int maxInputs = 1;
//         public IInput currentInput;
//         public List<IInput> inputs = new List<IInput>();
//         // public int inputInventorySize = 8;
//         public List<Resource> inputInventory = new List<Resource>();
//         
//         [Header("IOutput")]
//         public bool outputEnabled = true;
//         public int maxOutputs = 1;
//         public IOutput currentOutput;
//         public List<IOutput> outputs = new List<IOutput>();
//         // public int outputInventorySize = 8;
//         public List<Resource> outputInventory = new List<Resource>();
//     }
//     [SerializeField] internal Data node;
//
//     public bool AvailableInput => node.inputs is { } && node.inputs.Count < node.maxInputs;
//     public bool AvailableOutput => node.outputs is { } && node.outputs.Count < node.maxOutputs;
//     
//     //> IO INTERFACE
//     public IInput Input => node.currentInput;
//     public IOutput Output => node.currentOutput;
//     
//     //> PROPERTIES
//     virtual public bool InputFull => node.inputInventory.Count >= node.inventorySize;
//     virtual public bool OutputFull => node.outputInventory.Count >= node.inventorySize;
//     virtual public bool Full => node.inputInventory.Count >= node.inventorySize;
//     virtual public bool InputEmpty => node.inputInventory.Count == 0;
//     virtual public bool OutputEmpty => node.outputInventory.Count == 0;
//     virtual public bool Empty => node.outputInventory.Count == 0;
//     virtual public int InventorySize => node.inventorySize;
//
//     //> DESTORY AND CLEANUP MACHINE
//     override public void OnDestroy()
//     {
//         // node.inputs.ForEach(Destroy);
//         // node.outputs.ForEach(Destroy);
//         // node.inventory.ForEach(Destroy);
//         node.inputInventory.ForEach(Destroy);
//         node.outputInventory.ForEach(Destroy);
//         Destroy(gameObject);
//     }
//
//     //> CONNECT INPUT
//     virtual public void ConnectInput(IInput input)
//     {
//         if (!ConnectEdge(input as Edge)) return;
//         node.inputs.Add(input);
//         node.currentInput = node.inputs[0];
//     }
//
//     //> CONNECT OUTPUT
//     virtual public void ConnectOutput(IOutput output)
//     {
//         if (!ConnectEdge(output as Edge)) return;
//         node.outputs.Add(output);
//         node.currentOutput = node.outputs[0];
//     }
//     
//     virtual public void DisconnectInput(IInput input)
//     {
//         if (!DisconnectEdge(input as Edge)) return;
//         node.inputs.Remove(input);
//         // node.currentInput = (node.inputs.Count >= 1) ? node.inputs[0] : null;
//     }
//
//     virtual public void DisconnectOutput(IOutput output)
//     {
//         if (!DisconnectEdge(output as Edge)) return;
//         node.outputs.Remove(output);
//         // node.currentOutput = (node.inputs.Count >= 1) ? node.outputs[0] : null;
//     }
//     
//     //> DEPOSIT RESOURCE
//     virtual public void Deposit(Resource resource)
//     {
//         resource.data.position = Position;
//         // node.inventory.Add(resource);
//         node.inputInventory.Add(resource);
//         resource.SetVisible(false);
//         NextInput();
//     }
//
//
//     //> WITHDRAW RESOURCE
//     virtual public Resource Withdraw()
//     {
//         var resource = node.outputInventory.TakeFirst();
//         // var resource = node.inventory.TakeFirst();
//         resource.SetVisible(true);
//         NextOutput();
//         return resource;
//     }
//
//     protected void NextInput()
//     {
//         if (!node.inputEnabled || node.currentInput is null || node.maxInputs == 1) return;
//         var index = node.inputs.IndexOf(node.currentInput);
//         node.currentInput = (index < node.inputs.Count - 1) ? node.inputs[++index] : node.inputs[0];
//     }
//
//     protected void NextOutput()
//     {
//         if (!node.outputEnabled || node.currentOutput is null || node.maxOutputs == 1) return;
//         var index = node.outputs.IndexOf(node.currentOutput);
//         node.currentOutput = (index < node.outputs.Count - 1) ? node.outputs[++index] : node.outputs[0];
//     }
//
//     
//     //> FIXED CALCULATION INTERVAL
//     virtual protected void FixedUpdate()
//     {
//         if (node.sleeping) return;
//         if (node.ticks >= node.sleepThreshold) node.sleeping = true;
//         
//         
//     }
// }
//
// public class TestConveyor : Edge, IInput, IOutput
// {
//     override public void OnDestroy() { }
//
//     private bool full;
//     private TestMachine input;
//     private TestMachine output;
//
//     public bool Full => full;
//     public bool Empty => !full;
//
//     public IInput Input => input;
//     public IOutput Output => output;
//
//     public void ConnectInput(IInput newInput) => input = newInput as TestMachine;
//     public void ConnectOutput(IOutput newOutput) => output = newOutput as TestMachine;
//
//     public void Deposit(Resource resource) { }
//     public Resource Withdraw() => null;
// }
//
// public class TestBuildManager : MonoBehaviour
// {
//     public TestMachine currentMachine;
//     public TestConveyor currentConveyor;
//
//     public void TestBuild()
//     {
//         var firstNode = Factory.Spawn("Testing", currentMachine, Vector3.zero);
//         var secondNode = Factory.Spawn("Testing", currentMachine, Vector3.one * 2);
//
//         var conveyor = Factory.Spawn("Testing", currentConveyor, Vector3.one);
//         
//         firstNode.ConnectOutput(conveyor);
//         conveyor.ConnectInput(firstNode);
//         conveyor.ConnectOutput(secondNode);
//         secondNode.ConnectInput(conveyor);
//
//     }
// }