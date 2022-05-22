using Unity.Entities;
using Unity.Transforms;

namespace DOTSTemplate.Tasks
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class PathTaskSystem : SystemBase
    {
        private EventBuilder<CompleteTaskRequest> completeTaskEvent;
        private EntityCommandBufferSystem entityCommandBufferSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            completeTaskEvent = new EventBuilder<CompleteTaskRequest>(EntityManager);
            entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var completeTaskRequest = this.completeTaskEvent;
            var commands = entityCommandBufferSystem.CreateCommandBuffer();

            Dependency = Entities.ForEach((Entity entity, ref Progress progress,
                in PathTask task) =>
            {
                
                ref var polyline = ref task.Path.Value;
                
                var (_, closestPosition) = polyline.GetClosestPoint(progress.Value);
                progress.Value = closestPosition;
                
                if (progress.Value >= polyline.Length)
                {
                    completeTaskRequest.Raise(commands, new CompleteTaskRequest(entity));
                }
                
            }).Schedule(Dependency);

            entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
    
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class PathTaskActivateSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<StateChanged, PathTask>().ForEach((ref Progress progress) =>
            {
                progress.Value = 0.0f;
            }).Run();
        }
    }
}