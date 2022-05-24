using System.Runtime.CompilerServices;
using Unity.Entities;

namespace DOTSTemplate.Tweening
{
    public static class TweenExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Entity CreateTweenEntity<T>(this EntityManager entityManager, 
            Entity target, float duration, T tweenComponent)
            where T : struct, IComponentData
        {
            var entity = entityManager.CreateEntity(
                ComponentType.ReadWrite<Target>(),
                ComponentType.ReadWrite<Duration>(),
                ComponentType.ReadWrite<TweenProgress>(),
                ComponentType.ReadWrite<T>());
            
            entityManager.SetComponentData(entity, new Target {Entity = target});
            entityManager.SetComponentData(entity, new Duration {Value = duration});
            entityManager.SetComponentData(entity, tweenComponent);
            
            return entity;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void SetDelay(this EntityManager entityManager, Entity target, float delay)
        {
            if (target == Entity.Null || delay <= 0) return;
            entityManager.AddComponentData(target, new Delay {Value = delay});
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void SetEasing(this EntityManager entityManager, Entity target, EasingType easingType)
        {
            if (target == Entity.Null || easingType == EasingType.Linear) return;
            entityManager.AddComponentData(target, new Easing {Type = easingType});
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void SetLoops(this EntityManager entityManager, Entity target, int count, LoopingType type)
        {
            if (target == Entity.Null || count == 0) return;
            entityManager.AddComponentData(target, new Loop
            {
                Count = count,
                Type = type
            });
        }
    }
}