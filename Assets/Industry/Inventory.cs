using System;
using System.Linq;
using System.Collections.Generic;
using ProcessControl.Jobs;
using UnityEngine;


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
                var empties = items.Where(i => i.amount <= 0).ToList();
                empties.ForEach(e => items.Remove(e));
            };
        }
        
        public event Action onModified;

        private readonly int slots;
        private readonly int stackSize;
        [SerializeField] private List<Stack> items = new List<Stack>();
        
        public int Count => items.Sum(i => i.amount);
        public bool Full => Count >= slots * stackSize;
        public bool Empty => items.Count == 0;
        
        public void Clear() => items.Clear();
        public bool Contains(Item match, int amount = 1) => items.FirstOrDefault(i => i.item == match && i.amount >= amount) is {};
        public bool Contains(Stack stack) => Contains(stack.item, stack.amount);
        public IReadOnlyList<Stack> GetItems() => items.AsReadOnly();
        
        public bool CanDeposit(Item item)
        {
            if (items.Count < slots) return true;
            if (items.FirstOrDefault(i => i.item == item && i.amount < stackSize) is {}) return true;
            return false;
        }

        public bool CanDeposit(Stack stack)
        {
            if (items.Count < slots) return true;
            if (items.FirstOrDefault(i => i.item == stack.item && i.amount + stack.amount <= stackSize) is { }) return true;
            return false;
        }
        
        public void Deposit(Item newItem, int amount = 1)
        {
            if (Contains(newItem))
            {
                // Debug.Log("Inventory has item stack");
                var item = items.FirstOrDefault(i => i.item == newItem && i.amount < stackSize);
                if (item is { })
                {
                    // Debug.Log("Adding to stack.");
                    item.amount += amount;
                    onModified?.Invoke();
                    return;
                }
            }
            
            if (items.Count < slots)
            {
                // Debug.Log("Free slot adding new item");
                items.Add(new Stack
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
                var item = items.FirstOrDefault(i => i.item == stack.item && i.amount + stack.amount <= stackSize);
                if (item is { })
                {
                    item.amount += item.amount;
                    onModified?.Invoke();
                    return;
                }
            }
            
            if (items.Count < slots)
            {
                items.Add(stack);
                onModified?.Invoke();
            }
        }

        public Item Withdraw()
        {
            var i = items.FirstOrDefault(i => i.amount >= 1);
            if (i is null) return null;
            i.amount--;
            onModified?.Invoke();
            return i.item;
        }

        public Stack WithdrawFirst()
        {
            var i = items.FirstOrDefault(i => i.amount >= 1);
            if (i is null) return null;
            i.amount--;
            onModified?.Invoke();
            return new Stack
            {
                item = i.item,
                amount = 1,
            };
        }
        
        public Item Withdraw(Item match)
        {
            var i = items.FirstOrDefault(i => i.item == match && i.amount >= 1);
            if (i is null) return null;
            i.amount--;
            onModified?.Invoke();
            return i.item;
        }
        
        
        //@ not in use...
        public Stack Withdraw(Item match, int amount)
        {
            // if (!items.Any(i => i.amount >= amount)) return default;
            var i = items.FirstOrDefault(i => i.item == match && i.amount >= amount);
            if (i is null) return null;
            i.amount -= amount;
            onModified?.Invoke();
            return new Stack
            {
                item = i.item,
                amount = i.amount,
            };
        }
        
        public Stack Withdraw(Stack stack)
        {
            // if (!items.Any(i => i.amount >= amount)) return default;
            var i = items.FirstOrDefault(i => i.item == stack.item && i.amount >= stack.amount);
            if (i is null) return null;
            i.amount -= stack.amount;
            onModified?.Invoke();
            return new Stack
            {
                item = i.item,
                amount = i.amount,
            };
        }
    }

}