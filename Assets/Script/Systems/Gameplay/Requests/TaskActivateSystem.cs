using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace DOTSTemplate
{
    [UpdateInGroup(typeof(EventsSystemGroup))]
    public partial class TaskActivateSystem : SystemBase
    {
        private EntityQuery requests;
        
        protected override void OnUpdate()
        {
            Entities
                .WithStoreEntityQueryInField(ref requests)
                .ForEach((in ActivateTaskRequest activeTaskRequest) =>
                {
                    
                    ActivateTask(EntityManager, activeTaskRequest.Entity, 
                        activeTaskRequest.Player);

                }).WithStructuralChanges().Run();
            EntityManager.DestroyEntity(requests);
        }

        public static void ActivateTask(EntityManager entityManager, 
            Entity taskEntity, Entity player)
        {
            Debug.Log("Task activated: " + entityManager.GetName(taskEntity));
            
            entityManager.SetEnabled(taskEntity, true);
            entityManager.RemoveComponent<Completed>(taskEntity);
            entityManager.AddComponents(taskEntity, new ComponentTypes(
                ComponentType.ReadWrite<StateChanged>(),
                ComponentType.ReadWrite<AttachedPlayer>()
            ));
            
            entityManager.SetComponentData(taskEntity, new AttachedPlayer { Entity = player });

            if (entityManager.HasComponent<TaskGroup>(taskEntity))
            {
                var taskGroup = entityManager.GetComponentData<TaskGroup>(taskEntity);
                var children = entityManager.GetBuffer<ChildLink>(taskEntity);

                if (!children.IsEmpty)
                {
                    switch (taskGroup.CompletionStrategy)
                    {
                        case GroupCompletionStrategy.Sequence:
                        {
                            ActivateTask(entityManager, children[0].Entity, player);
                            break;
                        }
                        case GroupCompletionStrategy.Parallel:
                        {
                            using var childrenCopy = children.ToNativeArray(Allocator.Temp);
                            foreach (var child in childrenCopy)
                            {
                                ActivateTask(entityManager, child.Entity, player);
                            }
                            break;
                        }
                    }
                }
            }
            
            entityManager.UpdateIndicators(taskEntity);
        }
    }
}