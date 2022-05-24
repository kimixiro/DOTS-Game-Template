using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTSTemplate.Tweening
{
    public static class ScaleTweenBuilding
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
                entity = entityManager.CreateTweenEntity(target, duration, new ScaleTween
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
            
            public TweenBuilder WithEasing(EasingType type)
            {
                entityManager.SetEasing(entity, type);
                return this;
            }
            
            public TweenBuilder SetOffset(bool isOffset)
            {
                if (entity == Entity.Null || offset == isOffset) return this;
                var tween = entityManager.GetComponentData<ScaleTween>(entity);
                if (isOffset)
                {
                    entityManager.SetComponentData(entity, new ScaleTween
                    {
                        StartValue = tween.StartValue,
                        EndValue = tween.StartValue + tween.EndValue
                    });
                    offset = true;
                }
                else
                {
                    entityManager.SetComponentData(entity, new ScaleTween
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
                if (entity == Entity.Null) return this;
                var tween = entityManager.GetComponentData<ScaleTween>(entity);
                entityManager.SetComponentData(entity, new ScaleTween
                {
                    StartValue = value,
                    EndValue = offset ? value + tween.EndValue : tween.EndValue
                });
                return this;
            }
        }
        
        public static TweenBuilder TweenScale(
            this EntityManager entityManager, Entity target, float3 endValue, float duration)
        {
            if (!entityManager.HasComponent<NonUniformScale>(target)) return default;
            var scale = entityManager.GetComponentData<NonUniformScale>(target);
            return new TweenBuilder(entityManager, target, duration, scale.Value, endValue);
        }
    }
}