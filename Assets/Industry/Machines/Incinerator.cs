
namespace ProcessControl.Industry.Machines
{
    public class Incinerator : Machine
    {
        public override void Deposit(Container container)
        {
            Destroy(container);
        }
    }
}