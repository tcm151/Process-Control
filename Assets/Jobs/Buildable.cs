using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProcessControl.Industry;


namespace ProcessControl.Jobs
{
    public interface Buildable
    {
        public Recipe recipe {get;}
        
        public Task Deliver(Stack stack, float deliveryTime);
        public Task Build(float buildTime);
        public Task Disassemble(float deconstructionTime);
    }
}