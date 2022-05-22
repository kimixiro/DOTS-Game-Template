using Unity.Entities;
using UnityEngine;

namespace DOTSTemplate
{
    [RequireComponent(typeof(TriggerAuthoring))]
    public class TouchTaskAuthoring : TaskAuthoring, IConvertGameObjectToEntity
    {
        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            base.Convert(entity, dstManager, conversionSystem);
            dstManager.AddComponentData(entity, new TouchTask());
        }
    }
    
    public struct TouchTask : IComponentData
    {
        
    }
}