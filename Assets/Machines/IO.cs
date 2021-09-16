using ProcessControl.Conveyors;


namespace ProcessControl.Machines
{
    public interface IO
    {
        public void ConnectInput(Node node);
        public void ConnectOutput(Node node);
    }
}