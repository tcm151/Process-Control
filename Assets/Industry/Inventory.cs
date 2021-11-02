using System;
using System.Linq;
using System.Collections.Generic;


namespace ProcessControl.Industry.Machines
{
    public class Inventory
    {
        public Inventory(int slots, int stackSize)
        {
            this.slots = slots;
            this.stackSize = stackSize;

            onModified += () =>
            {
                var empties = items.Where(i => i.Value <= 0).Select(i => i.Key).ToList();
                empties.ForEach(e => items.Remove(e));
            };
        }

        private readonly int slots;
        private readonly int stackSize;
        private readonly Dictionary<Item, int> items = new Dictionary<Item, int>();

        public int Count => items.Sum(i => i.Value);
        public bool Full => Count >= slots * stackSize;
        public bool Empty => items.Count == 0;
        public List<Item> AllItems => items.Keys.ToList();


        public event Action onModified;
        
        public void Clear() => items.Clear();
        public bool Contains(Item match, int amount = 1) => items.ContainsKey(match) && items[match] >= amount;

        public List<KeyValuePair<Item, int>> GetItems() => items.ToList();

        public bool CanDeposit(Item item)
        {
            if (items.Count < slots) return true;
            if (items.ContainsKey(item) && items[item] < stackSize) return true;
            return false;
        }
        
        public void Deposit(Item newItem, int amount = 1)
        {
            if (Contains(newItem) && items[newItem] < stackSize) items[newItem] += amount;
            else if (items.Count < slots) items.Add(newItem, amount);
            onModified?.Invoke();
        }

        public Item Withdraw()
        {
            var item = (!Empty) ? items.FirstOrDefault(i => i.Value >= 1).Key : default;
            if (item is { })
            {
                items[item]--;
                onModified?.Invoke();
            }
            return item;
        }
        
        public Item Withdraw(Item match)
        {
            var item = (!Empty) ? items.FirstOrDefault(i => i.Key == match && i.Value >= 1).Key : default;
            if (item is { })
            {
                items[item]--;
                onModified?.Invoke();
            }
            return item;
        }
        
        //@ not in use...
        public (Item, int) Withdraw(Item match, int amount)
        {
            if (!items.Any(i => i.Value >= amount)) return default;
            var item = items.FirstOrDefault(i => i.Key == match && i.Value >= amount).Key;
            if (item is { })
            {
                items[item] -= amount;
                onModified?.Invoke();
            }
            return (item, amount);
        }
    }

}