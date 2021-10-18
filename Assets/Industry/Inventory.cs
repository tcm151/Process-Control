using System;
using System.Collections.Generic;
using ProcessControl.Tools;


namespace ProcessControl.Industry.Machines
{
    public class Inventory<T>
    {
        public Inventory(int slots, int maxItems)
        {
            this.slots = slots;
            this.maxItems = maxItems;
        }

        private readonly int slots;
        private readonly int maxItems;

        private readonly List<T> items = new List<T>();

        public bool Empty => items.Count == 0;
        public bool Full => items.Count >= maxItems;

        public event Action onModified;
        
        public void Deposit(T newItem)
        {
            items.Add(newItem);
            onModified?.Invoke();
        }

        public T Withdraw()
        {
            var item = (!Empty) ? items.TakeFirst() : default;
            if (item is {}) onModified?.Invoke();
            return item;
        }
    }
}