

namespace ProcessControl.Industry.Conveyors
{
    public class Splitter : Junction
    {
        override public Container Withdraw()
        {
            var resource = base.Withdraw();
            NextOutput();
            return resource;
        }
    }                     
}