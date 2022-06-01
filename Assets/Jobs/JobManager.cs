using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using ProcessControl.Tools;
using ProcessControl.Industry;
using ProcessControl.Procedural;
using Stack = ProcessControl.Industry.Stack;


namespace ProcessControl.Jobs
{
    public class JobManager : Service
    {
        public static Action<Job> QueueJob;
        // public static Action<List<Job>> QueueJobs;

        [Header("Jobs")]
        [SerializeField] private List<Job> openJobs = new List<Job>();
        [SerializeField] private List<Job> takenJobs = new List<Job>();
        [SerializeField] private List<Job> completedJobs = new List<Job>();

        [Header("Workers")]
        [SerializeField] private List<Worker> openWorkers = new List<Worker>();
        [SerializeField] private List<Worker> busyWorkers = new List<Worker>();

        //> INITIALIZATION
        protected override void Awake()
        {
            base.Awake();
            
            // program events
            QueueJob += (job) => openJobs.Add(job);
            // QueueJobs += (jobList) => jobList.ForEach(j => openJobs.Add(j));

            // collect workers
            openWorkers = FindObjectsOfType<Worker>().ToList();
            
            // setup protocol when workers finish their job
            openWorkers.ForEach(worker =>
            {
                worker.onJobCompleted += () =>
                {
                    busyWorkers.Remove(worker);
                    openWorkers.Add(worker);

                    var job = worker.currentJob;
                    takenJobs.Remove(job);
                    completedJobs.Add(job);
                    job.activeWorker = null;
                    worker.currentJob = Job.EmptyJob;
                    worker.currentOrder = Job.EmptyJob.orders[0];
                };
            });
        }

        //> ASSIGN JOBS
        private void FixedUpdate()
        {
            // no jobs or no workers
            if (openJobs.Count < 1 || openWorkers.Count < 1) return;

            // assign the job closest to the first available worker
            var worker = openWorkers.TakeAndRemoveFirst();
            var closestJob = openJobs.Where(job => job.prerequisite is {complete: true} || job.prerequisite is null)
                                     .OrderBy(job => Vector3.Distance(worker.position, job.orders.First(o => !o.complete).location))
                                     .FirstOrDefault();

            if (closestJob is null)
            {
                Debug.Log("One or more open jobs cannot be completed...");
                return;
            }

            worker.TakeJob(closestJob);
            busyWorkers.Add(worker);
            openJobs.Remove(closestJob);
            takenJobs.Add(closestJob);
        }

        public static void QueueEdgeConstruction(Cell firstCell, Cell secondCell, Recipe recipe, Buildable buildable, List<Inventory> inventories)
        {
            bool cannotBuild = false;
            // var matchingEntities = new List<Entity>();
            // var requiredContainers = new List<Container>();
            var matchingInventories = new List<(Inventory inventory, Stack itemAmount)>();
            recipe.inputItems.ForEach(itemAmount =>
            {
                // var matchingContainers = ItemFactory.FindItemsByClosest(selectedNode.position, i);
                // if (matchingContainers.Count < i.amount)
                // {
                // Debug.Log("NOT ENOUGH FOR RECIPE ITEMS...");
                // cannotBuild = true;
                // return;
                // }
                
                // matchingContainers.ForEach(c => requiredContainers.Add(c));
                // var matchingMachines = builtMachines.Where(m => m.inputInventory.Contains(i) || m.outputInventory.Contains(i)).ToList();
                // matchingMachines.ForEach(m => matchingInventories.Add(m));
                // Debug.Log($"Matching machines length: {matchingMachines.Count}");

                var bestMatch = inventories.Where(inv => inv.Contains(itemAmount))
                                           .OrderBy(inv => Vector3.Distance((firstCell.position + secondCell.position) / 2f, inv.parent.position))
                                           .FirstOrDefault();

                if (bestMatch is null) cannotBuild = true;
               
                matchingInventories.Add((bestMatch, itemAmount));
            });

            if (cannotBuild)
            {
                Debug.Log($"Unable to build {recipe.name}");
                return;
            }
            
            var collectionJob = new Job
            {
                description = $"Collect resources for {recipe.name}",
            };

            matchingInventories.ForEach(match =>
            {
                collectionJob.orders.Add(new Order
                {
                    description = $"gather {match.itemAmount.amount} {match.itemAmount.item}(s) for {recipe.name}",
                    location = match.inventory.parent.position,
                    action = () =>
                    {
                        match.inventory.Withdraw(match.itemAmount);
                        collectionJob.activeWorker.Deposit(match.itemAmount);
                        return Task.CompletedTask;
                    },
                });
            });
            QueueJob(collectionJob);
            
            var deliveryJob = new Job
            {
                description = $"deliver items to {recipe.name}",
                prerequisite = collectionJob,
            };

            // deliveryJob.orders.Add
            // (
            //     new Order
            //     {
            //         description = $"deliver items to {conveyor.name}",
            //         location = conveyor.Center,
            //         action = () =>
            //         {
            //             var deliveryItems = new List<Stack>();
            //             recipe.inputItems.ForEach
            //             (
            //                 itemAmount =>
            //                 {
            //                     var withdrawnItems = deliveryJob.activeWorker.Withdraw(itemAmount);
            //                     deliveryItems.Add(withdrawnItems);
            //                 }
            //             );
            //             return buildable.Deliver(deliveryItems);
            //         },
            //     }
            // );
            
            recipe.inputItems.ForEach(stack =>
            {
                deliveryJob.orders.Add(new Order
                {
                    description = $"deliver {stack.amount} {stack.item}(s) to {recipe.name}",
                    location = (firstCell.position + secondCell.position) / 2f,
                    action = () =>
                    {
                        deliveryJob.activeWorker.Withdraw(stack);
                        buildable.Deliver(stack, 0.25f);
                        return Task.CompletedTask;
                    },
                });
            });
            
            QueueJob(deliveryJob);
            
            QueueJob(new Job
            {
                description = $"build a {recipe.name}",
                prerequisite = deliveryJob,
                orders =
                {
                    new Order
                    {
                        description = $"build a {recipe.name}",
                        location = (firstCell.position + secondCell.position) / 2f,
                        action = () => buildable.Build(1),
                    },
                },
            });
        }

        public static void QueueNodeConstruction(Cell cell, Recipe recipe, Buildable buildable, List<Inventory> inventories)
        {
            bool cannotBuild = false;
            var matchingInventories = new List<(Inventory inventory, Stack stack)>();
            recipe.inputItems.ForEach(itemAmount =>
            {
                var bestMatch = inventories.Where(inv => inv.Contains(itemAmount))
                                           .OrderBy(inv => Vector3.Distance(cell.position, inv.parent.position))
                                           .FirstOrDefault();

                if (bestMatch is null) cannotBuild = true;

                matchingInventories.Add((bestMatch, itemAmount));
            });

            if (cannotBuild)
            {
                Debug.Log($"Unable to build {recipe.name}");
                return;
            }

            var collectionJob = new Job
            {
                description = $"Gather resources for {recipe.name}",
            };
            matchingInventories.ForEach(match =>
            {
                (Inventory inventory, Stack stack) = match;
                collectionJob.orders.Add(new Order
                {
                    description = $"gather {stack.amount} {stack.item.name}(s) for {recipe.name}",
                    location = inventory.parent.position,
                    action = () =>
                    {
                        inventory.Withdraw(stack);
                        collectionJob.activeWorker.Deposit(stack);
                        return Task.CompletedTask;
                    },
                });
            });
            QueueJob(collectionJob);

            var deliveryJob = new Job
            {
                description = $"deliver items to {recipe.name}",
                prerequisite = collectionJob,
            };
            recipe.inputItems.ForEach(stack =>
            {
                deliveryJob.orders.Add(new Order
                {
                    description = $"deliver {stack.amount} {stack.item.name}(s) to {recipe.name}",
                    location = cell.position,
                    action = () =>
                    {
                        deliveryJob.activeWorker.Withdraw(stack);
                        buildable.Deliver(stack, 0.25f);
                        return Task.CompletedTask;
                    },
                });
            });
            QueueJob(deliveryJob);

            var constructionJob = new Job
            {
                description = $"build a {recipe.name}",
                prerequisite = deliveryJob,
                orders = { new Order
                {
                    description = $"build a {recipe.name}",
                    location = cell.position,
                    action = () => buildable.Build(1),
                }},
            };
            QueueJob(constructionJob);
        }
    }
}