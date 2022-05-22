using Unity.Entities;
using UnityEngine;

namespace DOTSTemplate
{
    public class PlayerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponents(entity, new ComponentTypes(
                typeof(Player)
            ));
        }
    }
    
    public struct Player : IComponentData
    {
    }

    public struct AttachedPlayer : IComponentData
    {
        public Entity Entity;
    }
}