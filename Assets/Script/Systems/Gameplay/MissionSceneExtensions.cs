using DOTSTemplate.Tweening;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace DOTSTemplate
{
    public static class MissionSceneExtensions
    {
        private const float ActivationDuration = 0.5f;
        private const float DeactivationDuration = 0.5f;
        private const float Height = 5;
        
        public static void ActivateScene(this EntityManager entityManager, Entity sceneEntity)
        {
            var currentActiveSceneEntities = entityManager.CreateEntityQuery(
                ComponentType.ReadWrite<MissionScene>(), 
                ComponentType.ReadOnly<Active>()).ToEntityArray(Allocator.Temp);
            
            foreach (var entity in currentActiveSceneEntities)
            {
                if (entity == sceneEntity) continue;
                DeactivateScene(entityManager, entity);
            }
            
            entityManager.AddComponent<Active>(sceneEntity);
            
            var scene = entityManager.GetComponentData<MissionScene>(sceneEntity);
            ref var objects = ref scene.Data.Value.Objects;
            for (int i = 0; i < objects.Length; i++)
            {
                ActivateSceneObject(entityManager, objects[i]);
            }
        }

        private static void ActivateSceneObject(EntityManager entityManager, Entity objectEntity)
        {
            entityManager.RemoveComponent<Disabled>(objectEntity);
            if (entityManager.HasComponent<PhysicsVelocity>(objectEntity))
            {
                entityManager.SetComponentData(objectEntity, default(PhysicsVelocity));
            }
            
            entityManager.AddComponents(objectEntity, 
                new ComponentTypes(
                    typeof(Translation),
                    typeof(Rotation),
                    typeof(NonUniformScale)
                ));
            
            var missionObject = entityManager.GetComponentData<MissionSceneObject>(objectEntity);
            entityManager.TweenScale(objectEntity, missionObject.Scale, ActivationDuration)
                .From(0)
                .WithEasing(EasingType.ExponentialEaseOut);
            
            entityManager.TweenTranslation(objectEntity, missionObject.Position, ActivationDuration)
                .From(missionObject.Position + new float3(0, Height, 0))
                .WithEasing(EasingType.ExponentialEaseOut);
            
            entityManager.SetComponentData(objectEntity, new Rotation {Value = missionObject.Rotation});
        }

        public static void DeactivateScene(this EntityManager entityManager, Entity sceneEntity)
        {
            entityManager.RemoveComponent<Active>(sceneEntity);
            
            var scene = entityManager.GetComponentData<MissionScene>(sceneEntity);
            ref var objects = ref scene.Data.Value.Objects;
            for (int i = 0; i < objects.Length; i++)
            {
                DeactivateSceneObject(entityManager, objects[i]);
            }
        }

        private static void DeactivateSceneObject(EntityManager entityManager, Entity objectEntity)
        {
            var translation = entityManager.GetComponentData<Translation>(objectEntity);
            
            entityManager.TweenScale(objectEntity, 0, DeactivationDuration)
                .WithEasing(EasingType.ExponentialEaseIn);
            
            entityManager.TweenTranslation(objectEntity,
                    translation.Value + new float3(0, Height, 0),
                    DeactivationDuration)
                .WithEasing(EasingType.ExponentialEaseOut);
            
            entityManager.TweenEnabledState(objectEntity, false).WithDelay(DeactivationDuration);
        }
    }
}