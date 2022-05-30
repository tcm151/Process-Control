using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ProcessControl.Jobs;
using ProcessControl.Tools;
using ProcessControl.Graphs;
using ProcessControl.Industry;

#pragma warning disable 108,114

public class Storage : Node, IO, Buildable // HasInventory
{
    public Recipe recipe => schematic.recipe;

    private IO input;
    private IO output;

    [SerializeField] private Inventory inventory;
    public Inventory InputInventory => inventory;
    public Inventory OutputInventory => inventory;

    public IO Input
    {
        get => input;
        set => input = value;
    }

    public IO Output
    {
        get => output;
        set => output = value;
    }

    protected override void Awake()
    {
        base.Awake();
        inventory = new Inventory(16, 16 * 64, this.transform);
    }

    // public event Action onAllItemsDelivered;
    
    public async Task Deliver(Stack itemAmounts, float deliveryTime)
    {
        await Alerp.Delay(deliveryTime);
        // return Task.CompletedTask;
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
        input = newInput;
        return true;
    }

    virtual public bool DisconnectInput(IO oldInput)
    {
        if (input != oldInput) return false;
        input = null;
        return true;
    }

    virtual public bool ConnectOutput(IO newOutput)
    {
        if (output is { }) return false;
        output = newOutput;
        return true;
    }

    virtual public bool DisconnectOutput(IO oldOutput)
    {
        if (output != oldOutput) return false;
        output = null;
        return true;
    }

    virtual public bool CanWithdraw() => !inventory.Empty;
    virtual public Container Withdraw()
    {
        var stack = inventory.Withdraw();
        if (stack is null) Debug.Log("Inventory empty.");
        var container = ItemFactory.SpawnContainer(stack.item, position);
        return container;
    }

    virtual public bool CanDeposit(Item item) => !inventory.Full;
    virtual public void Deposit(Container container)
    {
        inventory.Deposit(container.stack);
        ItemFactory.DisposeContainer(container);
    }

    public bool Contains(Stack stack) => inventory.Contains(stack);
    public Stack Withdraw(Stack stack) => inventory.Withdraw(stack);
    public void Deposit(Stack stack) => inventory.Deposit(stack);
}