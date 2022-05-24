using Unity.Entities;

namespace DOTSTemplate
{
    public struct ActiveMissionRequest : IComponentData
    {
        public Entity Entity;
        public Entity Player;

        public ActiveMissionRequest(Entity entity, Entity player)
        {
            Entity = entity;
            Player = player;
        }
    }
    
    public struct CompleteMissionRequest : IComponentData
    {
        public Entity Entity;

        public CompleteMissionRequest(Entity entity)
        {
            Entity = entity;
        }
    }
    
    public struct CancelMissionRequest : IComponentData
    {
        public Entity Entity;

        public CancelMissionRequest(Entity entity)
        {
            Entity = entity;
        }
    }
}