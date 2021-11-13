using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ProcessControl.Jobs;
using ProcessControl.Tools;
using ProcessControl.Graphs;
using ProcessControl.Industry;
#pragma warning disable 108,114

public class Storage : Node, IBuildable, IInventory
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

    public Task DeliverItems(List<ItemAmount> itemAmounts)
    {
        return Task.CompletedTask;
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
        var resource = inventory.Withdraw();
        if (resource is null) Debug.Log("Inventory empty.");
        var container = ItemFactory.Instance.SpawnContainer(resource, position);
        return container;
    }

    override public bool CanDeposit(Item item) => !inventory.Full;
    override public void Deposit(Container container)
    {
        inventory.Deposit(container.item);
        ItemFactory.Instance.DisposeContainer(container);
    }

    public bool Contains(ItemAmount itemAmount) => inventory.Contains(itemAmount);
    public ItemAmount Withdraw(ItemAmount itemAmount) => inventory.Withdraw(itemAmount.item, itemAmount.amount);
    public void Deposit(ItemAmount itemAmount) => inventory.Deposit(itemAmount.item, itemAmount.amount);
}