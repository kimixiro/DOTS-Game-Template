using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;

namespace DOTSTemplate
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(StepPhysicsWorld))]
    public partial class CollectCollisionsSystem : SystemBase
    {
        private BuildPhysicsWorld buildPhysicsWorld;
        private StepPhysicsWorld stepPhysicsWorld;

        internal NativeHashMap<EntityPair, CollisionData> Collisions;
        
        public JobHandle OutputDependency => Dependency;

        protected override void OnCreate()
        {
            base.OnCreate();
            buildPhysicsWorld = World.GetExistingSystem<BuildPhysicsWorld>();
            stepPhysicsWorld = World.GetExistingSystem<StepPhysicsWorld>();
            
            Collisions = new NativeHashMap<EntityPair, CollisionData>(128, Allocator.Persistent);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Collisions.Dispose();
        }

        protected override void OnUpdate()
        {
            var physicsWorld = buildPhysicsWorld.PhysicsWorld;
            Collisions.Clear();
            
            var job = new CollectCollisionDataJob
            {
                World = physicsWorld,
                CollisionsData = Collisions
            };

            Dependency = job.Schedule(stepPhysicsWorld.Simulation, Dependency);
            buildPhysicsWorld.AddInputDependencyToComplete(Dependency);
        }

        [BurstCompile]
        private struct CollectCollisionDataJob : ICollisionEventsJob
        {
            [ReadOnly]
            public PhysicsWorld World;

            public NativeHashMap<EntityPair, CollisionData> CollisionsData;
            
            public void Execute(CollisionEvent collisionEvent)
            {
                var details = collisionEvent.CalculateDetails(ref World);
                var pair = new EntityPair(collisionEvent.EntityA, collisionEvent.EntityB);
                var collisionData = new CollisionData
                {
                    Impulse = details.EstimatedImpulse,
                    Normal = collisionEvent.Normal,
                    AverageContactPoint = details.AverageContactPointPosition
                };
                var maxPointCount = math.min(details.EstimatedContactPointPositions.Length,
                    collisionData.ContactPoints.Capacity);

                for (int i = 0; i < maxPointCount; i++)
                {
                    collisionData.ContactPoints.Add(details.EstimatedContactPointPositions[i]);
                }
                
                CollisionsData[pair] = collisionData;
            }
        }

    }
}