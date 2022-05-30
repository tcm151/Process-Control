

namespace ProcessControl.Industry.Conveyors
{
    public class Merger : Junction
    {
        public override void Deposit(Container container)
        {
            base.Deposit(container);
            NextInput();
            
            if (!Input.CanWithdraw()) NextInput();
            if (!Input.CanWithdraw()) NextInput();
            
        }
    }
}