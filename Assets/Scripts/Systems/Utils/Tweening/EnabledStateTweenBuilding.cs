using Unity.Entities;

namespace DOTSTemplate.Tweening
{
    public static class EnabledStateTweenBuilding
    {
        public struct TweenBuilder
        {
            private EntityManager entityManager;
            private readonly Entity entity;
            
            public Entity Entity => entity;

            internal TweenBuilder(EntityManager entityManager, Entity target, bool value)
            {
                this.entityManager = entityManager;
                entity = entityManager.CreateTweenEntity(target, 0, new EnabledStateTween
                {
                    State = value
                });
            }

            public TweenBuilder WithDelay(float delay)
            {
                entityManager.SetDelay(entity, delay);
                return this;
            }
        }
        
        public static TweenBuilder TweenEnabledState(this EntityManager entityManager, Entity target, bool state)
        {
            return new TweenBuilder(entityManager, target, state);
        }
    }
}