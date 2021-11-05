

namespace ProcessControl.Industry.Conveyors
{
    public class Merger : MultiJunction
    {
        override public void Deposit(Container container)
        {
            base.Deposit(container);
            NextInput();
            
            if (!Input.CanWithdraw()) NextInput();
            if (!Input.CanWithdraw()) NextInput();
            
        }
    }
}