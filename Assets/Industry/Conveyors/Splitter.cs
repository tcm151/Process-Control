

namespace ProcessControl.Industry.Conveyors
{
    public class Splitter : MultiJunction
    {
        override public Container Withdraw()
        {
            var resource = base.Withdraw();
            NextOutput();
            return resource;
        }
    }                     
}