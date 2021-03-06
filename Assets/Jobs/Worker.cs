using System;
using System.Linq;
using System.Threading;
using UnityEngine;
using ProcessControl.Industry;
using ProcessControl.Pathfinding;


namespace ProcessControl.Jobs
{
    public class Worker : Agent, IWorker // HasInventory
    {
        [Header("HasInventory")]
        public int stackSize = 16;
        public int inventorySlots = 4;
        [SerializeField] internal Inventory inventory;

        [Header("Job")]
        [SerializeField] internal Job currentJob;
        [SerializeField] internal Order currentOrder;

        //> PROPERTIES
        protected override bool Idle => (currentJob is null) || (currentJob.complete);
        
        //> EVENTS
        public event Action onJobCompleted;
        public event Action onOrderCompleted;
        
        //> INVENTORY
        public bool Contains(Stack stack) => inventory.Contains(stack);
        public void Deposit(Stack stack) => inventory.Deposit(stack);
        public Stack Withdraw(Stack stack) => inventory.Withdraw(stack);

        // private Action currentAction;
        // public void DoAction() => currentAction.Invoke();
        
        //> INITIALIZATION
        protected override void Awake()
        {
            base.Awake();
            
            currentJob = new Job { complete = true };
            currentOrder = new Order { complete = true };
            
            inventory = new Inventory(inventorySlots, stackSize, this.transform);
            
            onOrderCompleted += DoJob;
            onReachedDestination += DoOrder;
        }

        //> TAKE A NEW JOB
        public void TakeJob(Job newJob)
        {
            currentJob = newJob;
            currentJob.activeWorker = this;
            
            CancelAction();
            
            DoJob();
            // Debug.Log("Job Taken Successfully.");
        }
        
        //> CHECK STATUS ON CURRENT JOB
        private async void DoJob()
        {
            // Debug.Log("Doing job.");
            
            //- all orders in job are completed
            if (currentJob.orders.TrueForAll(o => o.complete))
            {
                // Debug.Log("Job Complete.");
                currentJob.complete = true;
                onJobCompleted?.Invoke();
                Roam();
                return;
            }

            // get closest incomplete order
            currentOrder = currentJob.orders.Where(o => !o.complete)
                                            .OrderBy(o => Vector3.Distance(position, o.location))
                                            .First();
            
            currentPath = await AStar.FindPath_Async(position, currentOrder.location);
            // Debug.Log($"path is {currentPath.Count} nodes long");
            // Debug.Log($"Starting: {currentOrder.description}");
        }

        //> COMPLETE ACTIVE JOB OR ROAM IF IDLE
        public async void DoOrder()
        {
            // Debug.Log("Destination Reached.");
            if (Idle)
            {
                // Debug.Log("~IDLE~");
                Roam();
                return;
            }

            await currentOrder.action();
            currentOrder.complete = true;
            onOrderCompleted?.Invoke();
        }

        //> CLEAN UP & DESTROY
        private void OnDestroy()
        {
            currentJob = null;
            currentOrder = null;
        }
    }
}