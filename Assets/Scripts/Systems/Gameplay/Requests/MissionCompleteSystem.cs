using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace DOTSTemplate
{
    [UpdateInGroup(typeof(EventsSystemGroup))]
    public partial class MissionCompleteSystem : SystemBase
    {
        private EntityQuery requests;

        protected override void OnUpdate()
        {
            Entities
                .WithStoreEntityQueryInField(ref requests)
                .ForEach((in CompleteMissionRequest completeMissionRequest) =>
                {
                    CompleteMission(EntityManager, completeMissionRequest.Entity);
                }).WithStructuralChanges().Run();
            EntityManager.DestroyEntity(requests);
        }

        public static void CompleteMission(EntityManager entityManager, Entity missionEntity)
        {
            Debug.Log("Mission completed: " + entityManager.GetName(missionEntity));
            
            var missionsInactiveMissionsQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadWrite<Mission>(),
                ComponentType.Exclude<Active>(),
                ComponentType.ReadWrite<Disabled>()
            );
            // Enable other missions
            using var otherMissions = missionsInactiveMissionsQuery.ToEntityArray(Allocator.Temp);
            foreach (var otherMission in otherMissions)
            {
                entityManager.SetEnabled(otherMission, true);
            }
            entityManager.RemoveComponent(missionEntity, new ComponentTypes(
                ComponentType.ReadWrite<Active>(),
                ComponentType.ReadWrite<AttachedPlayer>()
            ));
            
            var mission = entityManager.GetComponentData<Mission>(missionEntity);
            TaskDisableSystem.DisableTask(entityManager, mission.RootTask);
        }

    }
}