namespace ProcessControl.Industry.Machines
{
    public class HydraulicPress : Machine
    {
        public float speed;

        virtual protected void FixedUpdate()
        {
            if ((++ticks % (TicksPerMinute / speed)) == 0)
            {
                ticks = 0;
                if (inputInventory.Count == 0) return;
                
                
                EngagePress();
            }
        }

        private void EngagePress()
        {
            if (currentRecipe.inputItems.TrueForAll(recipeItem => inputInventory.Contains(recipeItem)))
            {
                currentRecipe.inputItems.ForEach(i => inputInventory.Withdraw(i));
                currentRecipe.outputItems.ForEach(i => outputInventory.Deposit(i));
            }
        }
    }
}