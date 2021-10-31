using System.Threading.Tasks;


namespace ProcessControl.Jobs
{
    public interface IBuildable
    {
        public Task Build(int buildTime);
        public Task Deconstruct(int deconstructionTime);
    }
}