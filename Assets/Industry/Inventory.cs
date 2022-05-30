using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


namespace ProcessControl.Industry
{
    [Serializable] public class Inventory
    {
        //> CONSTRUCTOR
        public Inventory(int slots, int stackSize, Transform parent)
        {
            this.slots = slots;
            this.stackSize = stackSize;
            this.parent = parent;
            
            onModified += () =>
            {
                var empties = inventory.Where(i => i.amount <= 0).ToList();
                empties.ForEach(e => inventory.Remove(e));
            };
        }

        public Transform parent;
        public event Action onModified;

        private readonly int slots;
        private readonly int stackSize;
        [SerializeField] private List<Stack> inventory = new List<Stack>();
        
        public int Count => inventory.Sum(i => i.amount);
        public bool Full => Count >= slots * stackSize;
        public bool Empty => inventory.Count == 0;
        
        public void Clear()
            => inventory.Clear();
        
        public IReadOnlyList<Stack> GetItems()
            => inventory.AsReadOnly();
        
        public bool Contains(Stack stack)
            => inventory.Any(s => s.item == stack.item && s.amount >= stack.amount);

        public bool Contains(List<Stack> stacks)
            => stacks.TrueForAll(stack => inventory.Contains(stack));
        
        public bool CanDeposit(Stack stack)
            => inventory.Count < slots || inventory.Any(s => s.item == stack.item && s.amount + stack.amount <= stackSize);

        public void Deposit(Stack stack)
        {
            if (Contains(stack))
            {
                var item = inventory.FirstOrDefault(s => s.item == stack.item && s.amount + stack.amount <= stackSize);
                if (item is { })
                {
                    item.amount += item.amount;
                    onModified?.Invoke();
                    return;
                }
            }
            
            if (inventory.Count < slots)
            {
                inventory.Add(stack);
                onModified?.Invoke();
            }
        }

        public Stack Withdraw()
        {
            var stack = inventory.FirstOrDefault(s => s.amount >= 1);
            if (stack is null) return null;
            stack.amount--;
            onModified?.Invoke();
            return new Stack { item = stack.item, amount = 1};
        }

        public Stack Withdraw(Stack match)
        {
            // if (!containers.Any(i => i.amount >= amount)) return default;
            var stack = inventory.FirstOrDefault(s => s.item == match.item && s.amount >= match.amount);
            if (stack is null) return null;
            stack.amount -= match.amount;
            onModified?.Invoke();
            return new Stack
            {
                item = stack.item,
                amount = stack.amount,
            };
        }
    }

}