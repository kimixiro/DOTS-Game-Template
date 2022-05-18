using Unity.Entities;

namespace DOTSTemplate
{
    public struct EventBuilder<T> where T : struct, IComponentData
    {
        private readonly EntityArchetype entityArchetype;
        
        public EventBuilder(EntityManager entityManager)
        {
            entityArchetype = entityManager.CreateArchetype(typeof(T));
        }
        
        public void Raise(EntityManager entityManager, T data)
        {
            var requestEntity = entityManager.CreateEntity(entityArchetype);
            entityManager.SetComponentData(requestEntity, data);
        }
        
        public void Raise(EntityCommandBuffer commandBuffer, T data)
        {
            var requestEntity = commandBuffer.CreateEntity(entityArchetype);
            commandBuffer.SetComponent(requestEntity, data);
        }
    }
    
    public static class EventBuilderExtensions
    {
        public static EventBuilder<T> CreateEventBuilder<T>(this EntityManager entityManager)
            where T : struct, IComponentData
        {
            return new EventBuilder<T>(entityManager);
        }
    }
}