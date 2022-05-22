using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace DOTSTemplate
{
    [UpdateInGroup(typeof(EventsSystemGroup))]
    public partial class TaskDisableSystem : SystemBase
    {
        private EntityQuery requests;
        
        protected override void OnUpdate()
        {
            Entities
                .WithStoreEntityQueryInField(ref requests)
                .ForEach((in DisableTaskRequest disableTaskRequest) =>
                {
                    DisableTask(EntityManager, disableTaskRequest.Entity);
                }).WithStructuralChanges().Run();
            EntityManager.DestroyEntity(requests);
        }

        public static void DisableTask(EntityManager entityManager, Entity taskEntity)
        {
            Debug.Log("Task disabled: " + entityManager.GetName(taskEntity));
            
            entityManager.SetEnabled(taskEntity, false);
            entityManager.AddComponent<StateChanged>(taskEntity);

            entityManager.RemoveComponent(taskEntity, new ComponentTypes(
                ComponentType.ReadWrite<Completed>(),
                ComponentType.ReadWrite<AttachedPlayer>()
                                                                       
            ));
            
            if (entityManager.HasComponent<TaskGroup>(taskEntity))
            {
                var children = entityManager.GetBuffer<ChildLink>(taskEntity);

                if (!children.IsEmpty)
                {
                    using var childrenCopy = children.ToNativeArray(Allocator.Temp);
                    foreach (var child in childrenCopy)
                    {
                        DisableTask(entityManager, child.Entity);
                    }
                }
            }
            
            entityManager.UpdateIndicators(taskEntity);
        }

    }
}