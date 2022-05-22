using Unity.Entities;
using UnityEngine;

namespace DOTSTemplate
{
    public class MissionAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public TaskAuthoring Task;
        public MissionSceneAuthoring Scene;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponents(entity, new ComponentTypes(
                typeof(Mission)
            ));

            var taskEntity = conversionSystem.GetPrimaryEntity(Task);
            dstManager.AddComponentData(taskEntity, new ParentLink
            {
                Entity = entity
            });
            dstManager.SetComponentData(entity, new Mission
            {
                RootTask = taskEntity,
                Scene = Scene != null ? conversionSystem.GetPrimaryEntity(Scene) : Entity.Null
            });
        }
    }
    
    public struct Mission : IComponentData
    {
        public Entity RootTask;
        public Entity Scene;
    }
}