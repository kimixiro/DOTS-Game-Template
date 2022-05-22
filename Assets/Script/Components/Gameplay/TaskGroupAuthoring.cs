using Unity.Entities;

namespace DOTSTemplate
{
    public class TaskGroupAuthoring : TaskAuthoring, IConvertGameObjectToEntity
    {
        public GroupCompletionStrategy CompletionStrategy;
        public TaskAuthoring[] Tasks;
        
        public override void Convert(Entity entity, EntityManager dstManager,
            GameObjectConversionSystem conversionSystem)
        {
            base.Convert(entity, dstManager, conversionSystem);
            dstManager.AddComponents(entity, new ComponentTypes(
                typeof(TaskGroup), 
                typeof(ChildLink)));
            dstManager.SetComponentData(entity, new TaskGroup
            {
                CompletionStrategy = CompletionStrategy
            });
            
            foreach (var taskAuthoring in Tasks)
            {
                var taskEntity = conversionSystem.GetPrimaryEntity(taskAuthoring);
                dstManager.AddComponentData(taskEntity, new ParentLink
                {
                    Entity = entity
                });
            }
            var tasksBuffer = dstManager.GetBuffer<ChildLink>(entity);
            foreach (var taskAuthoring in Tasks)
            {
                var taskEntity = conversionSystem.GetPrimaryEntity(taskAuthoring);
                tasksBuffer.Add(new ChildLink
                {
                    Entity = taskEntity
                });
            }
        }
    }
    
    public struct TaskGroup : IComponentData
    {
        public GroupCompletionStrategy CompletionStrategy;
    }

    public enum GroupCompletionStrategy
    {
        Sequence,
        Parallel
    }
}