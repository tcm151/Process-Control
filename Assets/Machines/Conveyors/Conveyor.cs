using System.Linq;
using ProcessControl.Machines;


namespace ProcessControl.Machines
{
    public class Conveyor : Edge
    {
        public const int TicksPerSecond = 64;
        public const int ItemsPerSecond = 8;

        protected void FixedUpdate()
        {
            
            if (++edge.ticks % (TicksPerSecond / ItemsPerSecond) == 0)
            {
                if (Full) return;
                
                edge.ticks = 0;
                if (edge.output && edge.inventory.Count >= 1)
                {
                    if (edge.output.Full) return;
                    edge.output.Deposit(Withdraw());
                }
            }
        }
    }
}
