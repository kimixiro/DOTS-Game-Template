using Unity.Entities;

namespace DOTSTemplate.Tasks
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class PathTaskIndicatorSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((PathTaskIndicator indicator, in AttachedTask attachedTask) =>
            {
                var isEnabled = !HasComponent<Completed>(attachedTask.Entity);
                indicator.SetEnabled(isEnabled);
                if (!isEnabled) return;

                var pathTask = GetComponent<PathTask>(attachedTask.Entity);
                var progress = GetComponent<Progress>(attachedTask.Entity);
                indicator.SetProgress(progress.Value, pathTask.Path.Value.Length);
            }).WithoutBurst().Run();
        }
    }
}