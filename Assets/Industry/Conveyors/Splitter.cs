using ProcessControl.Industry.Resources;


namespace ProcessControl.Industry.Conveyors
{
    public class Splitter : MultiJunction
    {
        override public Entity Withdraw()
        {
            var resource = base.Withdraw();
            NextOutput();
            return resource;
        }
    }                     
}