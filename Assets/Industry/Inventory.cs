using System;
using System.Collections.Generic;
using System.Linq;
using ProcessControl.Industry.Resources;
using ProcessControl.Tools;
using UnityEngine;
using Object = UnityEngine.Object;


namespace ProcessControl.Industry.Machines
{
    public class Inventory
    {
        public Inventory(int slots, int maxItems)
        {
            this.slots = slots;
            this.maxItems = maxItems;
        }

        public int Count => items.Count;

        private readonly int slots;
        private readonly int maxItems;

        private readonly List<Container> items = new List<Container>();

        public bool Empty => items.Count == 0;
        public bool Full => items.Count >= maxItems;

        public event Action onModified;

        public Container FirstItem => items[0];
        
        public bool Has(Item match, int amount) => items.Count(e => e.item == match) > 2f;

        // items.Count()
        public void Deposit(Container newItem)
        {
            items.Add(newItem);
            onModified?.Invoke();
        }

        public Container Withdraw()
        {
            var withdrawnItem = (!Empty) ? items.TakeFirst() : default;
            if (withdrawnItem is {}) onModified?.Invoke();
            return withdrawnItem;
        }

        public List<Container> Withdraw(int amount)
        {
            if (items.Count < amount) return default;
            var withdrawnItems = items.TakeRange(0, amount - 1);
            onModified?.Invoke();
            return withdrawnItems;
        }

        public void Clear() => items.ForEach(Object.Destroy);
    }
}