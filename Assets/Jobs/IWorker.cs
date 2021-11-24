namespace ProcessControl.Jobs
{
    public interface IWorker
    {
        // public void TakeJob(Order newOrder);
        public void TakeJob(Job newJob);
        public void CompleteOrder();
    }
}