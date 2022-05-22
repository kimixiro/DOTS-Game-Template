using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace DOTSTemplate
{
    public static class TaskExtensions
    {
        public static void UpdateIndicators(this EntityManager entityManager, Entity task)
        {
            if (!entityManager.HasComponent<TaskVisualStateEntities>(task)
                || !entityManager.HasComponent<TaskVisualState>(task)) return;

            var taskVisualStateEntities = entityManager.GetComponentData<TaskVisualStateEntities>(task);
            var taskVisualState = entityManager.GetComponentData<TaskVisualState>(task);
            
            ref var activeIndicators = ref taskVisualStateEntities.Indicators;
            foreach (var taskVisualStateEntity in activeIndicators)
            {
                entityManager.DestroyEntity(taskVisualStateEntity);
            }
            activeIndicators.Clear();

            if (entityManager.HasComponent<Completed>(task))
            {
                ref var completedPrefabs =
                    ref taskVisualState.CompleteIndicators;
                // Completed
                InstantiateIndicators(entityManager, task,
                    ref completedPrefabs,
                    ref activeIndicators);
            }
            else if (!entityManager.HasComponent<Disabled>(task))
            {
                ref var activePrefabs =
                    ref taskVisualState.ActiveIndicators;
                // Active
                InstantiateIndicators(entityManager, task,
                    ref activePrefabs,
                    ref activeIndicators);
            }

            entityManager.SetComponentData(task, taskVisualStateEntities);
        }

        private static void InstantiateIndicators(EntityManager entityManager, Entity task,
            ref List<IndicatorPrefabDefinition> prefabs,
            ref List<Entity> activeIndicators)
        {
            foreach (var indicatorPrefabDefinition in prefabs)
            {
                if (indicatorPrefabDefinition.Prefab == Entity.Null) continue;
                var e = entityManager.Instantiate(indicatorPrefabDefinition.Prefab);
                
                var localToWorld = entityManager.GetComponentData<LocalToWorld>(task);
                entityManager.AddComponents(e, new ComponentTypes(
                    ComponentType.ReadWrite<Translation>(),
                    ComponentType.ReadWrite<Rotation>(),
                    ComponentType.ReadWrite<AttachedTask>()
                ));
                entityManager.SetComponentData(e, new Translation {Value = localToWorld.Position});
                entityManager.SetComponentData(e, new Rotation {Value = localToWorld.Rotation});
                entityManager.AddComponentData(e, new AttachedTask {Entity = task});
                
                if (!indicatorPrefabDefinition.SeparatedLifecycle)
                {
                    activeIndicators.Add(e);
                }
            }
        }
    }
}