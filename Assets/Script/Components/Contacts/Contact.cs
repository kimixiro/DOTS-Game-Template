using Unity.Entities;

namespace DOTSTemplate.Contacts
{
    public struct Contact : IComponentData
    {
        public EntityPair EntityPair;
        public CollisionData CollisionData;
    }
    
    public struct Enter : IComponentData {}
    
    public struct Exit : IComponentData {}
}