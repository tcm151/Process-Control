using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProcessControl.Industry;
using ProcessControl.Pathfinding;
using ProcessControl.Tools;
using UnityEngine;
using Random = UnityEngine.Random;


namespace ProcessControl.Jobs
{
    public class Worker : Agent, IWorker, IInventory
    {
        [Header("Roaming")]
        public float roamingInterval = 2.5f;
        public float roamingDistance = 5f;
        
        //> EVENTS
        public event Action onJobCompleted;
        
        internal Order CurrentOrder;
        private CancellationTokenSource roamingCancellation;

        public int stackSize = 16;
        public int inventorySlots = 4;
        internal Inventory inventory;
        
        //> TAKE A NEW JOB
        public void TakeJob(Order newOrder)
        {
            CurrentOrder = newOrder;
            if (CurrentOrder.location is {}) currentPath = AStar.FindPath(transform.position, CurrentOrder.location);
            // if (CurrentOrder.requiredItems.Any(itemAmount => !inventory.Contains(itemAmount)))
            // {
            //     CurrentOrder.requiredItems.ForEach(
            //         i =>
            //         {
            //             var matchingContainers = ItemFactory.FindItemByClosest(transform.position, i);
            //             if (matchingContainers.Count < i.amount) Debug.Log("NOT ENOUGH ITEMS...");
            //         });
            // }
        }
        
        //> COMPLETE ACTIVE JOB OR ROAM IF IDLE
        public async void CompleteJob()
        {
            if (CurrentOrder is null)
            { 
                Roam();
                return;
            }
            
            await CurrentOrder.order();
            CurrentOrder.complete = true;
            onJobCompleted?.Invoke();
        }

        //> INITIALIZATION
        public bool Contains(ItemAmount itemAmount) => inventory.Contains(itemAmount);
        public ItemAmount Withdraw(ItemAmount itemAmount) => inventory.Withdraw(itemAmount.item, itemAmount.amount);
        public void Deposit(ItemAmount itemAmount) => inventory.Deposit(itemAmount.item, itemAmount.amount);
        
        override protected void Awake()
        {
            base.Awake();
            inventory = new Inventory(inventorySlots, stackSize);
            onReachedDestination += CompleteJob;
        }

        //> ROAM WHEN ENTERING PLAYMODE
        private void Start() => Roam();

        //> ROAM AROUND RANDOMLY
        private async void Roam()
        {
            roamingCancellation = new CancellationTokenSource();
            var time = 0f;
            while ((time += Time.deltaTime) < roamingInterval)
            {
                if (roamingCancellation.IsCancellationRequested) return;
                await Task.Yield();
            }
            if (CurrentOrder is {complete: false}) return;
            currentPath = AStar.FindPath(transform.position, transform.position + Random.insideUnitCircle.ToVector3() * roamingDistance);
        }

        //> CLEAN UP & DESTROY
        private void OnDestroy() => CurrentOrder = null;

        //> CANCEL ROAMING ON QUIT
        private void OnApplicationQuit() => roamingCancellation.Cancel();
        
    }
}