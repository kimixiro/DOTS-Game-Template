using Unity.Entities;

namespace DOTSTemplate.Tweening
{
    [UpdateInGroup(typeof(TweenSystemGroup), OrderFirst = true)]
    public partial class TweenProgressSystem : SystemBase
    {
        private EntityCommandBufferSystem entityCommandBufferSystem;
        private EntityQueryMask matchAnyQueryMask;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
            matchAnyQueryMask = EntityManager.GetEntityQueryMask(EntityManager.UniversalQuery);
        }
        
        protected override void OnUpdate()
        {
            var dt = Time.DeltaTime;
            var commands = entityCommandBufferSystem.CreateCommandBuffer();
            var matchAny = matchAnyQueryMask;
            
            Dependency = Entities
                .WithNone<Delay, Loop>()
                .ForEach((Entity entity, ref TweenProgress progress,
                    in Duration duration, in Target target) =>
                {
                    if (!matchAny.Matches(target.Entity))
                    {
                        progress.TargetDestroyed = true;
                        commands.DestroyEntity(entity);
                        return;
                    }
                    
                    progress.Time += dt;
                    progress.NormalizedTime = progress.Time / duration.Value;
                    
                    if (progress.NormalizedTime > 1)
                    {
                        progress.NormalizedTime = 1;
                        commands.DestroyEntity(entity);
                    }
                }).Schedule(Dependency);
            
            entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}