// using System;
// using System.Collections.Generic;
// using UnityEngine;
// using ProcessControl.Tools;
// using ProcessControl.Graphs;
// using ProcessControl.Machines;
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
//     [SerializeField] internal Data machine;
//
//     public bool AvailableInput => machine.inputs is { } && machine.inputs.Count < machine.maxInputs;
//     public bool AvailableOutput => machine.outputs is { } && machine.outputs.Count < machine.maxOutputs;
//     
//     //> IO INTERFACE
//     public IInput Input => machine.currentInput;
//     public IOutput Output => machine.currentOutput;
//     
//     //> PROPERTIES
//     virtual public bool InputFull => machine.inputInventory.Count >= machine.inventorySize;
//     virtual public bool OutputFull => machine.outputInventory.Count >= machine.inventorySize;
//     virtual public bool Full => machine.inputInventory.Count >= machine.inventorySize;
//     virtual public bool InputEmpty => machine.inputInventory.Count == 0;
//     virtual public bool OutputEmpty => machine.outputInventory.Count == 0;
//     virtual public bool Empty => machine.outputInventory.Count == 0;
//     virtual public int InventorySize => machine.inventorySize;
//
//     //> DESTORY AND CLEANUP MACHINE
//     override public void OnDestroy()
//     {
//         // machine.inputs.ForEach(Destroy);
//         // machine.outputs.ForEach(Destroy);
//         // machine.inventory.ForEach(Destroy);
//         machine.inputInventory.ForEach(Destroy);
//         machine.outputInventory.ForEach(Destroy);
//         Destroy(gameObject);
//     }
//
//     //> CONNECT INPUT
//     virtual public void ConnectInput(IInput input)
//     {
//         if (!ConnectEdge(input as Edge)) return;
//         machine.inputs.Add(input);
//         machine.currentInput = machine.inputs[0];
//     }
//
//     //> CONNECT OUTPUT
//     virtual public void ConnectOutput(IOutput output)
//     {
//         if (!ConnectEdge(output as Edge)) return;
//         machine.outputs.Add(output);
//         machine.currentOutput = machine.outputs[0];
//     }
//     
//     virtual public void DisconnectInput(IInput input)
//     {
//         if (!DisconnectEdge(input as Edge)) return;
//         machine.inputs.Remove(input);
//         // machine.currentInput = (machine.inputs.Count >= 1) ? machine.inputs[0] : null;
//     }
//
//     virtual public void DisconnectOutput(IOutput output)
//     {
//         if (!DisconnectEdge(output as Edge)) return;
//         machine.outputs.Remove(output);
//         // machine.currentOutput = (machine.inputs.Count >= 1) ? machine.outputs[0] : null;
//     }
//     
//     //> DEPOSIT RESOURCE
//     virtual public void Deposit(Resource resource)
//     {
//         resource.data.position = Position;
//         // machine.inventory.Add(resource);
//         machine.inputInventory.Add(resource);
//         resource.SetVisible(false);
//         NextInput();
//     }
//
//
//     //> WITHDRAW RESOURCE
//     virtual public Resource Withdraw()
//     {
//         var resource = machine.outputInventory.TakeFirst();
//         // var resource = machine.inventory.TakeFirst();
//         resource.SetVisible(true);
//         NextOutput();
//         return resource;
//     }
//
//     protected void NextInput()
//     {
//         if (!machine.inputEnabled || machine.currentInput is null || machine.maxInputs == 1) return;
//         var index = machine.inputs.IndexOf(machine.currentInput);
//         machine.currentInput = (index < machine.inputs.Count - 1) ? machine.inputs[++index] : machine.inputs[0];
//     }
//
//     protected void NextOutput()
//     {
//         if (!machine.outputEnabled || machine.currentOutput is null || machine.maxOutputs == 1) return;
//         var index = machine.outputs.IndexOf(machine.currentOutput);
//         machine.currentOutput = (index < machine.outputs.Count - 1) ? machine.outputs[++index] : machine.outputs[0];
//     }
//
//     
//     //> FIXED CALCULATION INTERVAL
//     virtual protected void FixedUpdate()
//     {
//         if (machine.sleeping) return;
//         if (machine.ticks >= machine.sleepThreshold) machine.sleeping = true;
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
//         var firstMachine = Factory.Spawn("Testing", currentMachine, Vector3.zero);
//         var secondMachine = Factory.Spawn("Testing", currentMachine, Vector3.one * 2);
//
//         var conveyor = Factory.Spawn("Testing", currentConveyor, Vector3.one);
//         
//         firstMachine.ConnectOutput(conveyor);
//         conveyor.ConnectInput(firstMachine);
//         conveyor.ConnectOutput(secondMachine);
//         secondMachine.ConnectInput(conveyor);
//
//     }
// }