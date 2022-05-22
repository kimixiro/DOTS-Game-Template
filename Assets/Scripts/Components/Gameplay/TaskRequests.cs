using Unity.Entities;

namespace DOTSTemplate
{
    public struct ActivateTaskRequest : IComponentData
    {
        public Entity Entity;
        public Entity Player;
        

        public ActivateTaskRequest(Entity entity, Entity player)
        {
            Entity = entity;
            Player = player;
        }
    }
    
    public struct CompleteTaskRequest : IComponentData
    {
        public Entity Entity;

        public CompleteTaskRequest(Entity entity)
        {
            Entity = entity;
        }
    }
    
    public struct DisableTaskRequest : IComponentData
    {
        public Entity Entity;

        public DisableTaskRequest(Entity entity)
        {
            Entity = entity;
        }
    }
}