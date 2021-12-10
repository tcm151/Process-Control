using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ProcessControl.Tools;
using ProcessControl.Industry;
using ProcessControl.Procedural;
using Stack = ProcessControl.Industry.Stack;


namespace ProcessControl.Jobs
{
    public class JobManager : MonoBehaviour
    {
        public static Action<Job> QueueJob;
        public static Action<List<Job>> QueueJobs;
        
        [Header("Jobs")]
        [SerializeField] private List<Job> openJobs = new List<Job>();
        [SerializeField] private List<Job> takenJobs = new List<Job>();
        [SerializeField] private List<Job> completedJobs = new List<Job>();
        
        [Header("Workers")]
        [SerializeField] private List<Worker> openWorkers = new List<Worker>();
        [SerializeField] private List<Worker> busyWorkers = new List<Worker>();

        //> INITIALIZATION
        private void Awake()
        {
            // program events
            QueueJob += (job) => openJobs.Add(job);
            QueueJobs += (jobList) => jobList.ForEach(j => openJobs.Add(j));
            
            // collect workers
            openWorkers = FindObjectsOfType<Worker>().ToList();
            openWorkers.ForEach(worker => 
            {
                // setup protocol when workers finish their job
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
            var closestJob = openJobs.Where(j => j.prerequisite is null || j.prerequisite is {complete: true})
                                     .OrderBy(j => Vector3.Distance(worker.position, j.orders.First(o => !o.complete).location))
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

        public static void QueueConstructionJob(Cell cell, Recipe recipe, Buildable buildable, List<HasInventory> inventories)
        {
            bool cannotBuild = false;
            // var matchingEntities = new List<Entity>();
            // var requiredContainers = new List<Container>();
            var matchingInventories = new List<(HasInventory inventory, Stack stack)>();
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
                                           .OrderBy(inv => Vector3.Distance(cell.position, inv.position))
                                           .FirstOrDefault();

                if (bestMatch is null) cannotBuild = true;
               
                matchingInventories.Add((bestMatch, itemAmount));
            });

            if (cannotBuild)
            {
                Debug.Log($"Unable to build {recipe.name}");
                return;
            }
            
            // if (node is Buildable buildable)
            // {
                // if (queueJob && queueJobGlobal)
                // {


                    var collectJob = new Job
                    {
                        description = $"Collect resources for {recipe.name}",
                    };

                    matchingInventories.ForEach
                    (
                        match =>
                        {
                            collectJob.orders.Add
                            (
                                new Order
                                {
                                    description = $"gather {match.stack.amount} {match.stack.item}(s) for {recipe.name}",
                                    location = match.inventory.position,
                                    action = () =>
                                    {
                                        match.inventory.Withdraw(match.stack);
                                        collectJob.activeWorker.Deposit(match.stack);
                                        return Task.CompletedTask;
                                    },
                                }
                            );
                        }
                    );

                    // requiredContainers.ForEach(
                    //     c =>
                    //     {
                    //         // if (e is Container c)
                    //         {
                    //             collectJob.orders.Add(new Order
                    //             {
                    //                 description = $"gather {c.item.name} for {nodeRecipe.name}",
                    //                 location = c.position,
                    //                 action = () =>
                    //                 {
                    //                     collectJob.activeWorker.Deposit(new Stack{item = c.item, amount = 1});
                    //                     ItemFactory.DisposeContainer(c);
                    //                     return Task.CompletedTask;
                    //                 },
                    //             });
                    //         }
                    //         // if (e is Machine m)
                    //         // {
                    //         //     collectJob.orders.Add(new Order
                    //         //     {
                    //         //         description = $"gather {"temp"} for {nodeRecipe.name}",
                    //         //         location = m.position,
                    //         //         action = () =>
                    //         //         {
                    //         //             collectJob.activeWorker.Deposit(new Stack{item = c.item, amount = 1});
                    //         //             ItemFactory.DisposeContainer(c);
                    //         //             return Task.CompletedTask;
                    //         //         },
                    //         //     });
                    //         // }
                    //     });

                    JobManager.QueueJob(collectJob);


                    var deliveryJob = new Job
                    {
                        description = $"deliver items to {recipe.name}",
                        prerequisite = collectJob,
                    };

                    deliveryJob.orders.Add
                    (
                        new Order
                        {
                            description = $"deliver items to {recipe.name}",
                            location = cell.position,
                            action = () =>
                            {
                                var deliveryItems = new List<Stack>();
                                recipe.inputItems.ForEach
                                (
                                    itemAmount =>
                                    {
                                        var withdrawnItems = deliveryJob.activeWorker.Withdraw(itemAmount);
                                        deliveryItems.Add(withdrawnItems);
                                    }
                                );
                                return buildable.DeliverItems(deliveryItems);
                            },
                        }
                    );
                    JobManager.QueueJob(deliveryJob);

                    var constructionJob = new Job
                    {
                        description = $"build a {recipe.name}",
                        prerequisite = deliveryJob,
                        orders =
                        {
                            new Order
                            {
                                description = $"build a {recipe.name}",
                                // prerequisite = deliveryJob,
                                location = cell.position,
                                action = () => buildable.Build(1),
                            },
                        },
                    };
                    JobManager.QueueJob(constructionJob);
                    // JobManager.QueueJob(new ConstructionJob(cell.position, buildable, 1));
                }
                // else buildable.Build(0);
            // }
        // }
    }
    
}

