using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DOTSTemplate.Tweening
{
    public static class RotationTweenBuilding
    {
        public struct TweenBuilder
        {
            private EntityManager entityManager;
            private readonly Entity entity;
            private bool offset;

            internal TweenBuilder(EntityManager entityManager, Entity target,
                float duration, float3 start, float3 end)
            {
                this.entityManager = entityManager;
                offset = false;
                entity = entityManager.CreateTweenEntity(target, duration, new RotationTween
                {
                    StartValue = start,
                    EndValue = end
                });
            }
            
            public TweenBuilder WithDelay(float delay)
            {
                entityManager.SetDelay(entity, delay);
                return this;
            }
            
            public TweenBuilder Looped(int count, LoopingType type)
            {
                entityManager.SetLoops(entity, count, type);
                return this;
            }
            
            public TweenBuilder WithEasing(EasingType easingType)
            {
                entityManager.SetEasing(entity, easingType);
                return this;
            }
            
            public TweenBuilder SetOffset(bool isOffset)
            {
                if (entity == Entity.Null || offset == isOffset) return this;
                var tween = entityManager.GetComponentData<RotationTween>(entity);
                if (isOffset)
                {
                    entityManager.SetComponentData(entity, new RotationTween
                    {
                        StartValue = tween.StartValue,
                        EndValue = tween.StartValue + tween.EndValue
                    });
                    offset = true;
                }
                else
                {
                    entityManager.SetComponentData(entity, new RotationTween
                    {
                        StartValue = tween.StartValue,
                        EndValue = tween.EndValue - tween.StartValue
                    });
                    offset = false;
                }

                return this;
            }
            
            public TweenBuilder From(float3 value)
            {
                var tween = entityManager.GetComponentData<RotationTween>(entity);
                entityManager.SetComponentData(entity, new RotationTween
                {
                    StartValue = value,
                    EndValue = offset ? value + tween.EndValue : tween.EndValue
                });
                return this;
            }
        }
        
        public static TweenBuilder TweenRotation(
            this EntityManager entityManager, Entity target, float3 endValue, float duration)
        {
            if (!entityManager.HasComponent<Rotation>(target)) return default;
            var rotation = entityManager.GetComponentData<Rotation>(target);
            return new TweenBuilder(entityManager, target, duration, math.radians((
                (Quaternion) rotation.Value).eulerAngles), endValue);
        }
    }
}