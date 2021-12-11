using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ProcessControl.Jobs;
using ProcessControl.Tools;
using ProcessControl.Graphs;
using ProcessControl.Industry;

#pragma warning disable 108,114

public class Storage : Node, IO, Buildable, IInventory
{
    public Recipe recipe => schematic.recipe;

    private Conveyor input;
    private Conveyor output;

    [SerializeField] private Inventory inventory = new Inventory(16, 16 * 64);

    virtual public IO Input => input;
    virtual public IO Output => output;

    // private void Awake()
    // {
    //     renderer = GetComponent<SpriteRenderer>();
    //     renderer.color = disabledColor;
    // }

    public Task Deliver(Stack itemAmounts)
    {
        return Task.CompletedTask;
    }
    
    public async Task Build(float buildTime)
    {
        var time = 0f;
        while ((time += Time.deltaTime) < buildTime) await Task.Yield();
        var enabledColor = renderer.color;
        enabledColor.a = EnabledAlpha;
        renderer.color = enabledColor;
        enabled = true;
    }
        
    public async Task Disassemble(float deconstructionTime)
    {
        var time = 0f;
        while ((time += Time.deltaTime) < deconstructionTime) await Task.Yield();
        Destroy(this);
    }

    
    virtual public bool ConnectInput(IO newInput)
    {
        if (input is { }) return false;
        input = newInput as Conveyor;
        return true;
    }

    virtual public bool DisconnectInput(IO oldInput)
    {
        if (input != oldInput as Conveyor) return false;
        input = null;
        return true;
    }

    virtual public bool ConnectOutput(IO newOutput)
    {
        if (output is { }) return false;
        output = newOutput as Conveyor;
        return true;
    }

    virtual public bool DisconnectOutput(IO oldOutput)
    {
        if (output != oldOutput as Conveyor) return false;
        output = null;
        return true;
    }

    virtual public bool CanWithdraw() => !inventory.Empty;
    virtual public Container Withdraw()
    {
        var resource = inventory.Withdraw();
        if (resource is null) Debug.Log("IInventory empty.");
        var container = ItemFactory.SpawnContainer(resource, position);
        return container;
    }

    virtual public bool CanDeposit(Item item) => !inventory.Full;
    virtual public void Deposit(Container container)
    {
        inventory.Deposit(container.item);
        ItemFactory.DisposeContainer(container);
    }

    public bool Contains(Stack stack) => inventory.Contains(stack);
    public Stack Withdraw(Stack stack) => inventory.Withdraw(stack.item, stack.amount);
    public void Deposit(Stack stack) => inventory.Deposit(stack.item, stack.amount);
}