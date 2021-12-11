using System;
using System.Linq;
using System.Collections.Generic;
using ProcessControl.Jobs;
using UnityEngine;
using UnityEngine.Serialization;


namespace ProcessControl.Industry
{
    [Serializable] public class Inventory
    {
        //> CONSTRUCTOR
        public Inventory(int slots, int stackSize)
        {
            this.slots = slots;
            this.stackSize = stackSize;

            onModified += () =>
            {
                var empties = inventory.Where(i => i.amount <= 0).ToList();
                empties.ForEach(e => inventory.Remove(e));
            };
        }
        
        public event Action onModified;

        private readonly int slots;
        private readonly int stackSize;
        [SerializeField] private List<Stack> inventory = new List<Stack>();
        
        public int Count => inventory.Sum(i => i.amount);
        public bool Full => Count >= slots * stackSize;
        public bool Empty => inventory.Count == 0;
        
        public void Clear()
            => inventory.Clear();
        
        public bool Contains(Item match, int amount = 1)
            => inventory.FirstOrDefault(s => s.item == match && s.amount >= amount) is {};
        
        public bool Contains(Stack stack)
            => Contains(stack.item, stack.amount);
        
        public IReadOnlyList<Stack> GetItems()
            => inventory.AsReadOnly();
        
        public bool CanDeposit(Item item)
        {
            if (inventory.Count < slots) return true;
            if (inventory.Any(s => s.item == item && s.amount < stackSize)) return true;
            return false;
        }

        public bool CanDeposit(Stack stack)
        {
            if (inventory.Count < slots) return true;
            if (inventory.Any(s => s.item == stack.item && s.amount + stack.amount <= stackSize)) return true;
            return false;
        }
        
        public void Deposit(Item newItem, int amount = 1)
        {
            if (Contains(newItem))
            {
                // Debug.Log("IInventory has item match");
                var item = inventory.FirstOrDefault(s => s.item == newItem && s.amount < stackSize);
                if (item is { })
                {
                    // Debug.Log("Adding to match.");
                    item.amount += amount;
                    onModified?.Invoke();
                    return;
                }
            }
            
            if (inventory.Count < slots)
            {
                // Debug.Log("Free slot adding new item");
                inventory.Add(new Stack
                {
                    item = newItem,
                    amount = amount,
                });
                onModified?.Invoke();
            }
            // else Debug.Log("Neither...");
        }

        public void Deposit(Stack stack)
        {
            if (Contains(stack.item))
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

        public Item Withdraw()
        {
            var stack = inventory.FirstOrDefault(s => s.amount >= 1);
            if (stack is null) return null;
            stack.amount--;
            onModified?.Invoke();
            return stack.item;
        }

        public Stack WithdrawFirst()
        {
            var stack = inventory.FirstOrDefault(s => s.amount >= 1);
            if (stack is null) return null;
            stack.amount--;
            onModified?.Invoke();
            return new Stack
            {
                item = stack.item,
                amount = 1,
            };
        }
        
        public Item Withdraw(Item match)
        {
            var stack = inventory.FirstOrDefault(s => s.item == match && s.amount >= 1);
            if (stack is null) return null;
            stack.amount--;
            onModified?.Invoke();
            return stack.item;
        }
        
        
        //@ not in use...
        public Stack Withdraw(Item match, int amount)
        {
            // if (!inventory.Any(i => i.amount >= amount)) return default;
            var stack = inventory.FirstOrDefault(s => s.item == match && s.amount >= amount);
            if (stack is null) return null;
            stack.amount -= amount;
            onModified?.Invoke();
            return new Stack
            {
                item = stack.item,
                amount = stack.amount,
            };
        }
        
        public Stack Withdraw(Stack match)
        {
            // if (!inventory.Any(i => i.amount >= amount)) return default;
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