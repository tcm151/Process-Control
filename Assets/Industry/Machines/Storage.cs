using System.Threading.Tasks;
using UnityEngine;
using ProcessControl.Jobs;
using ProcessControl.Tools;
using ProcessControl.Graphs;
using ProcessControl.Industry;
#pragma warning disable 108,114

public class Storage : Node, IBuildable
{
    private Conveyor input;
    private Conveyor output;

    private readonly Inventory inventory = new Inventory(16, 16 * 64);

    override public IO Input => input;
    override public IO Output => output;

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        renderer.color = disabledColor;
    }

    public async Task Build(float buildTime)
    {
        var time = 0f;
        while ((time += Time.deltaTime) < buildTime) await Task.Yield();
        renderer.color = enabledColor;
        enabled = true;
    }
        
    public async Task Deconstruct(float deconstructionTime)
    {
        var time = 0f;
        while ((time += Time.deltaTime) < deconstructionTime) await Task.Yield();
        Destroy(this);
    }

    
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
        var container = ItemFactory.Instance.SpawnItem(resource, this.position);
        return container;

    }

    override public bool CanDeposit(Item item) => !inventory.Full;
    override public void Deposit(Container container)
    {
        inventory.Deposit(container.item);
        Destroy(container);
    }
}