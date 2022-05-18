using Unity.Entities;

namespace DOTSTemplate
{
    public static class EntityExtensions
    {
        public static void Link(this EntityManager entityManager, Entity ownerEntity, Entity entity)
        {
            var buffer = entityManager.AddBuffer<LinkedEntityGroup>(ownerEntity);
            if (buffer.IsEmpty) buffer.Add(ownerEntity);
            buffer.Add(entity);
        }
    }
}