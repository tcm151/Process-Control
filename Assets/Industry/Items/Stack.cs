using System;


namespace ProcessControl.Industry
{
    [Serializable] public class Stack
    {
        public Item item;
        public int amount = 1;
    }

    [Serializable] public class Amount<T>
    {
        public T item;
        public int amount;
    }
}