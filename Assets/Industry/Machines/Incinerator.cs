namespace ProcessControl.Machines
{
    class Incinerator : Machine
    {
        override public void Deposit(Resource resource)
        {
            Destroy(resource);
        }
    }
}