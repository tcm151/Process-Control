using System;


namespace ProcessControl.Industry
{
    [Serializable] public class Stack
    {
        public Item item;
        public int amount = 1;
        
        // protected bool Equals(Stack other) => (stack == other.stack) && (amount == other.amount);
    }

    [Serializable] public class Stack<T>
    {
        public T item;
        public int amount;
    }
}