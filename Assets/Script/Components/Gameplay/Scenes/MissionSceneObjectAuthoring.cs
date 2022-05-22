using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTSTemplate
{
    public class MissionSceneObjectAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponents(entity, new ComponentTypes(
                ComponentType.ReadWrite<MissionSceneObject>(),
                ComponentType.ReadWrite<Disabled>()
            ));

            var thisTransform = transform;
            var position = thisTransform.position;
            var rotation = thisTransform.rotation;
            var localScale = thisTransform.localScale;
            dstManager.SetComponentData(entity, new MissionSceneObject
            {
                Position = position,
                Rotation = rotation,
                Scale = localScale
            });
        }
    }

    public struct MissionSceneObject : IComponentData
    {
        public float3 Position;
        public quaternion Rotation;
        public float3 Scale;
    }
}