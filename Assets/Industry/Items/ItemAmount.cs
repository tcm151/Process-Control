using System;


namespace ProcessControl.Industry
{
    [Serializable] public class ItemAmount
    {
        public Item item;
        public int amount = 1;
    }
}