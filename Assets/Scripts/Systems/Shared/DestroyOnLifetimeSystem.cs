using Unity.Entities;

namespace DOTSTemplate
{
    public partial class DestroyOnLifetimeSystem : SystemBase
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
            
            Dependency = Entities.ForEach((Entity entity, ref Lifetime lifetime) =>
            {
                lifetime.Time -= dt;
                
                if (lifetime.Time < 0)
                    commands.DestroyEntity(entity);
                
            }).Schedule(Dependency);
            
            entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}