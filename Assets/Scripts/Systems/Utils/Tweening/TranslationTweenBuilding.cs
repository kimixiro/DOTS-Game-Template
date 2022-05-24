using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTSTemplate.Tweening
{
    public static class TranslationTweenBuilding
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
                entity = entityManager.CreateTweenEntity(target, duration, new TranslationTween
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
                var tween = entityManager.GetComponentData<TranslationTween>(entity);
                if (isOffset)
                {
                    tween.EndValue += tween.StartValue;
                    entityManager.SetComponentData(entity, tween);
                    offset = true;
                }
                else
                {
                    tween.EndValue -= tween.StartValue;
                    entityManager.SetComponentData(entity, tween);
                    offset = false;
                }

                return this;
            }
            
            public TweenBuilder From(float3 value)
            {
                if (entity == Entity.Null) return this;
                var tween = entityManager.GetComponentData<TranslationTween>(entity);
                entityManager.SetComponentData(entity, new TranslationTween
                {
                    StartValue = value,
                    EndValue = offset ? value + tween.EndValue : tween.EndValue
                });
                return this;
            }
        }

        public static TweenBuilder TweenTranslation(
            this EntityManager entityManager, Entity target, float3 endValue, float duration)
        {
            if (!entityManager.HasComponent<Translation>(target)) return default;
            var translation = entityManager.GetComponentData<Translation>(target);
            return new TweenBuilder(entityManager, target, duration, translation.Value, endValue);
        }
    }
}