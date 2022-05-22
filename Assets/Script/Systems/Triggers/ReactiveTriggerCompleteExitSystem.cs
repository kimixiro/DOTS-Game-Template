using DOTSTemplate.Contacts;
using Unity.Entities;

namespace DOTSTemplate
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(TriggerSystem))]
    [UpdateBefore(typeof(ReactiveTriggerExitSystem))]
    public partial class ReactiveTriggerCompleteExitSystem : SystemBase
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
            
            Entities.WithAll<Trigger, Exit>().ForEach((Entity entity) =>
            {
                commands.RemoveComponent<Triggered>(entity);
                commands.RemoveComponent<Exit>(entity);
            }).Run();
        }
    }
}