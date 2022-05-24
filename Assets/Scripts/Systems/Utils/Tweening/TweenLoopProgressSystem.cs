using Unity.Entities;

namespace DOTSTemplate.Tweening
{
    [UpdateInGroup(typeof(TweenSystemGroup), OrderFirst = true)]
    public partial class TweenLoopProgressSystem : SystemBase
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
                .WithNone<Delay>()
                .ForEach((Entity entity, ref TweenProgress progress, ref Loop loop, 
                    in Duration duration, in Target target) =>
                {
                    if (!matchAny.Matches(target.Entity))
                    {
                        progress.TargetDestroyed = true;
                        commands.DestroyEntity(entity);
                        return;
                    }

                    progress.Time += dt;
                    
                    var timeInLoop = progress.Time % duration.Value;
                    var loopIndex = (int) (progress.Time / duration.Value);
                    
                    progress.NormalizedTime = timeInLoop / duration.Value;
                    
                    if (loop.Count >= 0 && loopIndex >= loop.Count)
                    {
                        progress.NormalizedTime = 1;
                        commands.DestroyEntity(entity);
                    }
                    
                    if (loop.Type == LoopingType.Yoyo && loopIndex % 2 == 1)
                    {
                        progress.NormalizedTime = 1 - progress.NormalizedTime;
                    }
                    
                }).Schedule(Dependency);

            entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}