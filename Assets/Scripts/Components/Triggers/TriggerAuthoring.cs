using Unity.Entities;
using UnityEngine;

namespace DOTSTemplate
{
    public class TriggerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public bool IsReactive;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponents(entity,
                IsReactive
                    ? new ComponentTypes(typeof(Trigger), typeof(Reactive))
                    : new ComponentTypes(typeof(Trigger)));
        }
    }
    
    public struct Trigger : IComponentData { }
    
    public struct Reactive : IComponentData { }
    
    public struct Triggered : IComponentData { }
}