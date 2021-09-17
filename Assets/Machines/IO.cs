
namespace ProcessControl.Machines
{
    public interface IO
    {
        public void ConnectInput(Edge input);
        public void ConnectOutput(Edge output);

        public void Deposit(Resource resource);
        public Resource Withdraw();
    }
}