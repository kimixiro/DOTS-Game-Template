using Unity.Entities;

namespace DOTSTemplate.Tweening
{
    [UpdateInGroup(typeof(TweenSystemGroup), OrderFirst = true)]
    public partial class TweenDelaySystem : SystemBase
    {
        private EntityCommandBufferSystem entityCommandBufferSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }
        
        protected override void OnUpdate()
        {
            var dt = Time.DeltaTime;
            var commands = entityCommandBufferSystem.CreateCommandBuffer();
            Dependency = Entities
                .ForEach((Entity entity, ref TweenProgress progress, in Delay delay) =>
                {
                    
                    progress.Time += dt;
                    if (progress.Time >= delay.Value)
                    {
                        progress.Time -= delay.Value;
                        commands.RemoveComponent<Delay>(entity);
                    }
                    
                }).Schedule(Dependency);
            
            entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}