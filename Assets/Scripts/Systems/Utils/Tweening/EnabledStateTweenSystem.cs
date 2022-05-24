using Unity.Entities;

namespace DOTSTemplate.Tweening
{
    [UpdateInGroup(typeof(TweenSystemGroup))]
    public partial class EnabledStateTweenSystem : SystemBase
    {
        private EntityCommandBufferSystem entityCommandBufferSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }
        
        protected override void OnUpdate()
        {
            var commands = entityCommandBufferSystem.CreateCommandBuffer();
            Dependency = Entities
                .WithNone<Delay>()
                .ForEach((in TweenProgress progress, in Target tween, in EnabledStateTween enabledState) =>
                {
                    if (progress.TargetDestroyed) return;
                    if (progress.NormalizedTime >= 1)
                    {
                        if (enabledState.State)
                        {
                            commands.RemoveComponent<Disabled>(tween.Entity);
                        }
                        else
                        {
                            commands.AddComponent<Disabled>(tween.Entity);
                        }
                    }
                }).Schedule(Dependency);
            entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}