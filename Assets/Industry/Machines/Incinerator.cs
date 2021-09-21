using ProcessControl.Industry.Resources;


namespace ProcessControl.Industry.Machines
{
    class Incinerator : Machine
    {
        override public void Deposit(Resource resource)
        {
            Destroy(resource);
        }
    }
}