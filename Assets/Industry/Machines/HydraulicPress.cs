namespace ProcessControl.Industry.Machines
{
    public class HydraulicPress : Machine
    {
        public float speed;

        override protected void FixedUpdate()
        {
            base.FixedUpdate();

            if ((++ticks % (TicksPerMinute / speed)) == 0)
            {
                ticks = 0;
                if (inputInventory.Count == 0) return;
                
                
                EngagePress();
            }
        }

        private void EngagePress()
        {
            if (currentRecipe.inputItems.TrueForAll(recipeItem => inputInventory.Contains(recipeItem.item, recipeItem.amount)))
            {
                currentRecipe.inputItems.ForEach(i => inputInventory.Withdraw(i.item, i.amount));
                currentRecipe.outputItems.ForEach(r => outputInventory.Deposit(r.item, r.amount));
            }
        }
    }
}