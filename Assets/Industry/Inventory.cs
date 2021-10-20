using System;
using System.Linq;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using ProcessControl.Industry.Resources;
using ProcessControl.Tools;


namespace ProcessControl.Industry.Machines
{
    public class Inventory
    {
        public Inventory(int maxItems)
        {
            this.maxItems = maxItems;
        }

        private readonly int maxItems;
        private readonly List<Container> items = new List<Container>();


        public int Count => items.Count;
        public bool Full => items.Count >= maxItems;
        public bool Empty => items.Count == 0;
        public Container FirstItem => items[0];

        public event Action onModified;
        
        public void Clear() => items.ForEach(Object.Destroy);
        public bool Has(Item match, int amount) => items.Count(e => e.item == match) > 2f;

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

    }
}