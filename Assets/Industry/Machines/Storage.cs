using ProcessControl;
using ProcessControl.Graphs;
using ProcessControl.Industry;
using ProcessControl.Industry.Conveyors;
using ProcessControl.Industry.Machines;
using ProcessControl.Industry.Resources;
using ProcessControl.Tools;
using UnityEngine;

public class Storage : Node
{
    private Conveyor input;
    private Conveyor output;

    private Inventory inventory = new Inventory(16, 16 * 64);

    override public IO Input => input;
    override public IO Output => output;

    override public bool ConnectInput(IO newInput)
    {
        if (input is { }) return false;
        input = newInput as Conveyor;
        return true;
    }

    override public bool DisconnectInput(IO oldInput)
    {
        if (input != oldInput as Conveyor) return false;
        input = null;
        return true;
    }

    override public bool ConnectOutput(IO newOutput)
    {
        if (output is { }) return false;
        output = newOutput as Conveyor;
        return true;
    }

    override public bool DisconnectOutput(IO oldOutput)
    {
        if (output != oldOutput as Conveyor) return false;
        output = null;
        return true;
    }

    override public bool CanWithdraw() => !inventory.Empty;
    override public Container Withdraw()
    {
        var resource = inventory.Withdraw(null);
        if (resource is null) Debug.Log("Inventory empty.");
        var container = ItemFactory.Instance.SpawnItem(resource, this.Position);
        return container;

    }

    override public bool CanDeposit(Item item) => !inventory.Full;
    override public void Deposit(Container container)
    {
        inventory.Deposit(container.item);
        Destroy(container);
    }
}