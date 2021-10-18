using ProcessControl.Industry.Resources;


namespace ProcessControl.Industry.Conveyors
{
    public class Merger : MultiJunction
    {
        override public void Deposit(Entity entity)
        {
            base.Deposit(entity);
            NextInput();
            
            if (!Input.CanWithdraw) NextInput();
            if (!Input.CanWithdraw) NextInput();
            
        }
    }
}