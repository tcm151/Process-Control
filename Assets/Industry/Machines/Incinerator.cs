﻿using ProcessControl.Industry.Resources;


namespace ProcessControl.Industry.Machines
{
    public class Incinerator : Machine
    {
        override public void Deposit(Entity entity)
        {
            Destroy(entity);
        }
    }
}