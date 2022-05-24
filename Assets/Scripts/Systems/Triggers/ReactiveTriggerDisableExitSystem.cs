using DOTSTemplate;
using Unity.Entities;

namespace DOTSTemplate
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(TriggerSystem))]
    public partial class ReactiveTriggerDisableExitSystem : SystemBase
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
            
            Entities
                .WithAll<Trigger, Triggered, Disabled>()
                .WithEntityQueryOptions(EntityQueryOptions.IncludeDisabled)
                .ForEach((Entity entity) =>
                {
                    commands.RemoveComponent<Triggered>(entity);
                    commands.RemoveComponent<Enter>(entity);
                    commands.RemoveComponent<Exit>(entity);
                }).Run();
        }
    }
}