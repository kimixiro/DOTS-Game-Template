using Unity.Entities;
using UnityEngine;

namespace DOTSTemplate
{
    public class LifetimeAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public float Lifetime = 5;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponents(entity, new ComponentTypes(
                ComponentType.ReadWrite<Lifetime>(),
                ComponentType.ReadWrite<DestroyOnLifetime>()
                ));
            
            dstManager.SetComponentData(entity, new Lifetime
            {
                Time = Lifetime
            });
        }
    }
    
    public struct DestroyOnLifetime : IComponentData {}
}