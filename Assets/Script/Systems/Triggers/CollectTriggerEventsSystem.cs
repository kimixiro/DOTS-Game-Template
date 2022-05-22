using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;

namespace DOTSTemplate
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(StepPhysicsWorld))]
    public partial class CollectTriggerEventsSystem : SystemBase
    {
        private BuildPhysicsWorld buildPhysicsWorld;
        private StepPhysicsWorld stepPhysicsWorld;
        
        internal NativeHashSet<TriggerEvent> TriggerEvents;

        public JobHandle OutputDependency => Dependency;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            TriggerEvents = new NativeHashSet<TriggerEvent>(32, Allocator.Persistent);
            
            buildPhysicsWorld = World.GetExistingSystem<BuildPhysicsWorld>();
            stepPhysicsWorld = World.GetExistingSystem<StepPhysicsWorld>();
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            TriggerEvents.Dispose();
        }
        
        protected override void OnUpdate()
        {
            TriggerEvents.Clear();
            
            var job = new TriggerEventsJob
            {
                TriggerEvents = TriggerEvents,
                Triggers = GetComponentDataFromEntity<Trigger>()
            };
            Dependency = job.Schedule(stepPhysicsWorld.Simulation, Dependency);
            
            buildPhysicsWorld.AddInputDependencyToComplete(Dependency);
        }
        
        private struct TriggerEventsJob : ITriggerEventsJob
        {
            [ReadOnly]
            public ComponentDataFromEntity<Trigger> Triggers;

            public NativeHashSet<TriggerEvent> TriggerEvents;
            
            public void Execute(Unity.Physics.TriggerEvent triggerEvent)
            {
                if (Triggers.HasComponent(triggerEvent.EntityA))
                {
                    TriggerEvents.Add(new TriggerEvent(triggerEvent.EntityA, triggerEvent.EntityB));
                }
                if (Triggers.HasComponent(triggerEvent.EntityB))
                {
                    TriggerEvents.Add(new TriggerEvent(triggerEvent.EntityB, triggerEvent.EntityA));
                }
            }
        }
    }
}