using DOTSTemplate.Contacts;
using Unity.Entities;

namespace DOTSTemplate
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(TriggerSystem))]
    [UpdateBefore(typeof(ReactiveTriggerEnterSystem))]
    public partial class ReactiveTriggerCompleteEnterSystem : SystemBase
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
            
            Entities.WithAll<Trigger, Enter>().ForEach((Entity entity) =>
            {
                commands.RemoveComponent<Enter>(entity);
            }).Run();
        }
    }
}