using System;
using System.Linq;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using ProcessControl.Industry.Resources;
using ProcessControl.Tools;
using UnityEngine;
using UnityEngine.Rendering;


namespace ProcessControl.Industry.Machines
{
    public class Inventory
    {
        public Inventory(int stackSize)
        {
            this.stackSize = stackSize;
        }

        private readonly int slots;
        private readonly int stackSize;
        // private readonly List<Item> items = new List<Item>();
        public readonly Dictionary<Item, int> items = new Dictionary<Item, int>();

        public int Count => items.Sum(i => i.Value);
        public bool Full => items.Count >= stackSize;
        public bool Empty => items.Count == 0;
        public List<Item> Items => items.Keys.ToList();

        public List<KeyValuePair<Item, int>> GetItems() => items.ToList();

        public event Action onModified;
        
        public void Clear() => items.Clear();
        public bool Has(Item match, int amount) => items.ContainsKey(match) && items[match] >= amount;

        public void Deposit(Item newItem)
        {
            if (items.ContainsKey(newItem)) items[newItem]++;
            else items.Add(newItem, 1);
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
        
        public (Item, int) Withdraw(int amount)
        {
            if (!items.Any(i => i.Value >= amount)) return default;
            var item = items.FirstOrDefault(i => i.Value >= amount).Key;
            if (item is { })
            {
                items[item] -= amount;
                onModified?.Invoke();
            }
            return (item, amount);
        }
    }

}