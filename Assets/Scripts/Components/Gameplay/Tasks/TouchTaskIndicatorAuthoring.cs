using Unity.Entities;
using UnityEngine;

namespace DOTSTemplate
{
    public class TouchTaskIndicatorAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public float VerticalOffset = 0.5f;
        public float VerticalOffsetDuration = 0.5f;
        public float RotationDuration = 0.5f;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new TouchTaskIndicator
            {
                RotationDuration = RotationDuration,
                VerticalOffset = VerticalOffset,
                VerticalOffsetDuration = VerticalOffsetDuration
            });
        }
    }
    
    public struct TouchTaskIndicator : IComponentData
    {
        public float VerticalOffset;
        public float VerticalOffsetDuration;
        public float RotationDuration;
    }
}