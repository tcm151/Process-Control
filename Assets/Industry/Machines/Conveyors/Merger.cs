using System;
using ProcessControl.Tools;
using ProcessControl.Industry.Machines;
using ProcessControl.Industry.Resources;


namespace ProcessControl.Industry.Conveyors
{
    public class Merger : MultiJunction
    {
        override public void Deposit(Resource resource)
        {
            base.Deposit(resource);
            NextInput();
            
            if (!Input.CanWithdraw) NextInput();
            if (!Input.CanWithdraw) NextInput();
            
        }
    }
}