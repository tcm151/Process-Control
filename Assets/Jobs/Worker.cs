using System;
using System.Collections.Generic;
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
        
        public int stackSize = 16;
        public int inventorySlots = 4;
        [SerializeField] internal Inventory inventory;
        
        //> EVENTS
        public event Action onJobCompleted;
        public event Action onOrderCompleted;
        
        [SerializeField] internal Job currentJob;
        [SerializeField] internal Order currentOrder;
        private CancellationTokenSource roamingCancellation;

        // private Action currentAction;
        // public void DoAction() => currentAction.Invoke();
        
        //> INITIALIZATION
        override protected void Awake()
        {
            base.Awake();
            inventory = new Inventory(inventorySlots, stackSize);
            // onReachedDestination += CompleteOrder;
            onOrderCompleted += DoJob;
            onReachedDestination += CompleteOrder;
        }

        //> ROAM WHEN ENTERING PLAYMODE
        private void Start() => Roam();
        
        //> TAKE A NEW ORDER
        // public void TakeJob(Order newOrder)
        // {
        //     currentOrder = newOrder;
        //     // currentAction = CompleteOrder;
        //     if (currentOrder.location is {}) currentPath = AStar.FindPath(position, currentOrder.location);
        // }

        //> TAKE A NEW JOB
        public void TakeJob(Job newJob)
        {
            currentJob = newJob;
            currentJob.activeWorker = this;
            DoJob();
        }

        // public async void GatherItems(List<ItemAmount> itemAmounts)
        // {
        //     for (int i = 0; i < itemAmounts.Count; )
        //     {
        //         if (!inventory.Contains(itemAmounts[i]))
        //         {
        //             var activeContainer = ItemFactory.FindItemByClosest(position, itemAmounts[i]);
        //             currentPath = AStar.FindPath(position, activeContainer.position);
        //             currentAction = () =>
        //             {
        //                 inventory.Deposit(activeContainer.item);
        //                 ItemFactory.DisposeContainer(activeContainer);
        //             };
        //             
        //             await Task.Yield();
        //         }
        //         else i++;
        //     }
        // }

        public void DoJob()
        {
            Debug.Log("Doing job.");
            
            if (currentJob.orders.TrueForAll(o => o.complete))
            {
                Debug.Log("FULL JOB COMPLETE...");
                currentJob.complete = true;
                onJobCompleted?.Invoke();
                return;
            }

            
            currentOrder = currentJob.orders.First(o => !o.complete);
            // currentAction = CompleteOrder;
            currentPath = AStar.FindPath(position, currentOrder.location);
            Debug.Log($"path is {currentPath.Count} nodes long");
            Debug.Log("starting next order...");
        }

        //> COMPLETE ACTIVE JOB OR ROAM IF IDLE
        public async void CompleteOrder()
        {
            if (currentOrder is null)
            { 
                Roam();
                Debug.Log("CURRENT ORDER IS NULL");
                return;
            }
            
            await currentOrder.action();
            currentOrder.complete = true;
            Debug.Log("single order complete...");
            onOrderCompleted?.Invoke();
        }

        public bool Contains(ItemAmount itemAmount) => inventory.Contains(itemAmount);
        public ItemAmount Withdraw(ItemAmount itemAmount) => inventory.Withdraw(itemAmount.item, itemAmount.amount);
        public void Deposit(ItemAmount itemAmount) => inventory.Deposit(itemAmount.item, itemAmount.amount);

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
            if (currentOrder is {complete: false}) return;
            currentPath = AStar.FindPath(transform.position, transform.position + Random.insideUnitCircle.ToVector3() * roamingDistance);
        }

        //> CLEAN UP & DESTROY
        private void OnDestroy() => currentOrder = null;

        //> CANCEL ROAMING ON QUIT
        private void OnApplicationQuit() => roamingCancellation.Cancel();
        
    }
}