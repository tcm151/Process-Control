

namespace ProcessControl.Industry.Conveyors
{
    public class Splitter : Junction
    {
        public override Container Withdraw()
        {
            var resource = base.Withdraw();
            NextOutput();
            return resource;
        }
    }                     
}