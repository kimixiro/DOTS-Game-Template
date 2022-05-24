using Unity.Entities;
using UnityEngine;

namespace DOTSTemplate
{
    [UpdateInGroup(typeof(EventsSystemGroup))]
    public partial class TaskCompleteSystem : SystemBase
    {
        private EntityQuery requests;

        protected override void OnUpdate()
        {
            Entities
                .WithStoreEntityQueryInField(ref requests)
                .ForEach((in CompleteTaskRequest completeTaskRequest) =>
                {
                    CompleteTask(EntityManager, completeTaskRequest.Entity);
                }).WithStructuralChanges().Run();
            EntityManager.DestroyEntity(requests);
        }

        public static void CompleteTask(EntityManager entityManager, Entity taskEntity)
        {
            Debug.Log("Task completed: " + entityManager.GetName(taskEntity));
            
            entityManager.SetEnabled(taskEntity, true);
            entityManager.AddComponent<Completed>(taskEntity);
            entityManager.AddComponent<StateChanged>(taskEntity);

            if (entityManager.HasComponent<ParentLink>(taskEntity))
            {
                var parentEntity = entityManager.GetComponentData<ParentLink>(taskEntity).Entity;
                if (entityManager.HasComponent<Mission>(parentEntity))
                {
                    var completeMissionBuilder = new EventBuilder<CompleteMissionRequest>(entityManager);
                    completeMissionBuilder.Raise(entityManager, new CompleteMissionRequest( parentEntity));
                }
                else if (entityManager.HasComponent<TaskGroup>(parentEntity))
                {
                    var player = entityManager.GetComponentData<AttachedPlayer>(parentEntity).Entity;
                    var parentTaskGroup = entityManager.GetComponentData<TaskGroup>(parentEntity);
                    var parentSubTasks = entityManager.GetBuffer<ChildLink>(parentEntity);
                    switch (parentTaskGroup.CompletionStrategy)
                    {
                        case GroupCompletionStrategy.Sequence:
                            for (var index = 0; index < parentSubTasks.Length; index++)
                            {
                                var subTask = parentSubTasks[index];
                                if (subTask.Entity == taskEntity)
                                {
                                    // Activate next
                                    if (index < parentSubTasks.Length - 1)
                                    {
                                        TaskActivateSystem.ActivateTask(entityManager,
                                            parentSubTasks[index + 1].Entity, player);
                                    }
                                    else
                                    {
                                        CompleteTask(entityManager, parentEntity);
                                    }

                                    break;
                                }
                            }
                            break;
                        case GroupCompletionStrategy.Parallel:
                            var allCompleted = true;
                            foreach (var subTask in parentSubTasks)
                            {
                                if (!entityManager.HasComponent<Completed>(subTask.Entity))
                                {
                                    allCompleted = false;
                                    break;
                                }
                            }

                            if (allCompleted)
                                CompleteTask(entityManager, parentEntity);
                            break;
                    }
                }
            }
            
            entityManager.UpdateIndicators(taskEntity);
        }

    }
}