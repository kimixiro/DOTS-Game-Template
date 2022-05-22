using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace DOTSTemplate
{
    [UpdateInGroup(typeof(EventsSystemGroup))]
    public partial class MissionCancelSystem : SystemBase
    {
        private EntityQuery requests;

        protected override void OnUpdate()
        {
            Entities
                .WithStoreEntityQueryInField(ref requests)
                .ForEach((in CancelMissionRequest cancelMissionRequest) =>
                {
                    CancelMission(EntityManager, cancelMissionRequest.Entity);
                }).WithStructuralChanges().Run();
            EntityManager.DestroyEntity(requests);
        }

        public static void CancelMission(EntityManager entityManager, Entity missionEntity)
        {
            Debug.Log("Mission canceled: " + entityManager.GetName(missionEntity));

            var missionsInactiveMissionsQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<Mission>(),
                ComponentType.Exclude<Active>()
            );

            using var otherMissions = missionsInactiveMissionsQuery.ToEntityArray(Allocator.Temp);
            foreach (var otherMission in otherMissions)
            {
                entityManager.SetEnabled(otherMission, true);
            }

            entityManager.RemoveComponent(missionEntity, new ComponentTypes(
                ComponentType.ReadWrite<Active>(),
                ComponentType.ReadWrite<AttachedPlayer>()
                ));

            TaskDisableSystem.DisableTask(entityManager,
                entityManager.GetComponentData<Mission>(missionEntity).RootTask);
        }
    }
}