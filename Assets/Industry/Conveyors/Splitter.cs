// namespace ProcessControl.Machines
// {
//     class Splitter : ConveyorNode
//     {
//         override public Conveyor Output => nextOutput;
//
//         override public void ConnectOutput(Conveyor outputs)
//         {
//             if (transportNode.outputs.Contains(outputs)) return;
//             transportNode.outputs.Add(outputs);
//         }
//
//         override public void Deposit(Resource resource)
//         {
//             nextOutput.Deposit(resource);
//             var currentIndex = transportNode.outputs.IndexOf(nextOutput) + 1;
//             if (currentIndex >= transportNode.outputs.Count) currentIndex = 0;
//             nextOutput = transportNode.outputs[currentIndex];
//         }
//         
//         private Conveyor nextOutput;
//         
//         
//     }
// }