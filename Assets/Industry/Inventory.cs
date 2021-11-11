using System;
using System.Linq;
using System.Collections.Generic;


namespace ProcessControl.Industry
{
    public class Inventory
    {
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

        private readonly int slots;
        private readonly int stackSize;
        // private readonly Dictionary<Item, int> items = new Dictionary<Item, int>();
        private readonly List<ItemAmount> items = new List<ItemAmount>();
        
        // public int Count => items.Sum(i => i.Value);
        public int Count => items.Sum(i => i.amount);
        public bool Full => Count >= slots * stackSize;
        public bool Empty => items.Count == 0;


        public event Action onModified;
        
        public void Clear() => items.Clear();
        // public bool Contains(Item match, int amount = 1) => items.ContainsKey(match) && items[match] >= amount;
        public bool Contains(Item match, int amount = 1) => items.FirstOrDefault(i => i.item == match && i.amount >= amount) is {};
        public bool Contains(ItemAmount itemAmount) => Contains(itemAmount.item, itemAmount.amount);
        
        // public List<KeyValuePair<Item, int>> GetItems() => items.ToList();
        public IReadOnlyList<ItemAmount> GetItems() => items.AsReadOnly();

        public bool CanDeposit(Item item)
        {
            if (items.Count < slots) return true;
            // if (items.ContainsKey(item) && items[item] < stackSize) return true;
            if (items.FirstOrDefault(i => i.item == item && i.amount < stackSize) is {}) return true;
            return false;
        }
        
        public void Deposit(Item newItem, int amount = 1)
        {
            // if (Contains(newItem) && Count < MaxItems) items[newItem] += amount;
            if (Contains(newItem))
            {
                var item = items.FirstOrDefault(i => i.item == newItem && i.amount < stackSize);
                if (item is { })
                {
                    item.amount += amount;
                    onModified?.Invoke();
                    return;
                }
            }
            
            if (items.Count < slots)
            {
                items.Add(new ItemAmount
                {
                    item = newItem,
                    amount = amount,
                });
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
        
        public Item Withdraw(Item match)
        {
            var i = items.FirstOrDefault(i => i.item == match && i.amount >= 1);
            if (i is null) return null;
            i.amount--;
            onModified?.Invoke();
            return i.item;
        }
        
        //@ not in use...
        public ItemAmount Withdraw(Item match, int amount)
        {
            // if (!items.Any(i => i.amount >= amount)) return default;
            var i = items.FirstOrDefault(i => i.item == match && i.amount >= amount);
            if (i is null) return null;
            i.amount -= amount;
            onModified?.Invoke();
            return new ItemAmount
            {
                item = i.item,
                amount = i.amount,
            };
        }
    }

}